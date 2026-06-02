using Maliev.MaterialService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Infrastructure.Data.SeedData;

/// <summary>
/// Handles initial database seeding for the manufacturing catalog.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the manufacturing catalog (processes, materials, finishes, tolerances, config options)
    /// if the data does not already exist.
    /// </summary>
    public static async Task SeedManufacturingCatalogAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        try
        {
            // Always run: fix stale names and add new combos idempotently.
            await EnsureProcessNamesAsync(context, logger);
            await EnsureFinishNamesAsync(context, logger);
            await EnsureProcessFinishLinksAsync(context, logger);

            if (await context.ManufacturingProcesses.AnyAsync())
            {
                await EnsureMaterialDetailReferenceDataAsync(context, logger);
                logger.LogInformation("Manufacturing catalog already seeded. Skipping.");
                return;
            }

            logger.LogInformation("Seeding manufacturing catalog...");

            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var tx = await context.Database.BeginTransactionAsync();

                // 1. Processes
                var processes = ManufacturingCatalogSeedData.GetProcesses().ToList();
                await context.ManufacturingProcesses.AddRangeAsync(processes);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} manufacturing processes.", processes.Count);

                // 2. Materials
                var materials = ManufacturingCatalogSeedData.GetMaterials().ToList();
                await context.Materials.AddRangeAsync(materials);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} materials.", materials.Count);

                // 3. Surface finishes
                var finishes = ManufacturingCatalogSeedData.GetSurfaceFinishes().ToList();
                await context.SurfaceFinishes.AddRangeAsync(finishes);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} surface finishes.", finishes.Count);

                // 4. Tolerance classes
                var tolerances = ManufacturingCatalogSeedData.GetToleranceClasses().ToList();
                await context.ToleranceClasses.AddRangeAsync(tolerances);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} tolerance classes.", tolerances.Count);

                // 5. Process config options
                var configOptions = ManufacturingCatalogSeedData.GetProcessConfigOptions().ToList();
                await context.ProcessConfigOptions.AddRangeAsync(configOptions);
                await context.SaveChangesAsync();
                logger.LogInformation("Seeded {Count} process config options.", configOptions.Count);

                // 6. M2M: Process ↔ Material (via existing material_manufacturing_processes table)
                var processDict = processes.ToDictionary(p => p.Id);
                var materialDict = materials.ToDictionary(m => m.Id);
                foreach (var (processId, materialId) in ManufacturingCatalogSeedData.GetProcessMaterialLinks())
                {
                    if (processDict.TryGetValue(processId, out var process) &&
                        materialDict.TryGetValue(materialId, out var material))
                    {
                        process.Materials.Add(material);
                    }
                }
                await context.SaveChangesAsync();

                // 7. M2M: Process ↔ SurfaceFinish
                var finishDict = finishes.ToDictionary(f => f.Id);
                foreach (var (processId, finishId) in ManufacturingCatalogSeedData.GetProcessFinishLinks())
                {
                    if (processDict.TryGetValue(processId, out var process) &&
                        finishDict.TryGetValue(finishId, out var finish))
                    {
                        finish.AvailableForProcesses.Add(process);
                    }
                }
                await context.SaveChangesAsync();

                // 8. M2M: Process ↔ ToleranceClass
                var toleranceDict = tolerances.ToDictionary(t => t.Id);
                foreach (var (processId, toleranceId) in ManufacturingCatalogSeedData.GetProcessToleranceLinks())
                {
                    if (processDict.TryGetValue(processId, out var process) &&
                        toleranceDict.TryGetValue(toleranceId, out var tolerance))
                    {
                        tolerance.AvailableForProcesses.Add(process);
                    }
                }
                await context.SaveChangesAsync();

                await tx.CommitAsync();
                logger.LogInformation("Manufacturing catalog seeding complete.");
            });

            await EnsureMaterialDetailReferenceDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the manufacturing catalog.");
        }
    }

    /// <summary>
    /// Fixes any processes whose Name was accidentally stored as the Code value.
    /// Runs unconditionally on every startup so stale dev/staging databases self-heal.
    /// </summary>
    private static async Task EnsureProcessNamesAsync(MaterialDbContext context, ILogger logger)
    {
        var canonical = ManufacturingCatalogSeedData.GetProcesses()
            .ToDictionary(p => p.Code, p => p.Name);

        var staleProcesses = await context.ManufacturingProcesses
            .Where(p => canonical.Keys.Contains(p.Code) && p.Name == p.Code)
            .ToListAsync();

        if (staleProcesses.Count == 0) return;

        foreach (var process in staleProcesses)
        {
            if (canonical.TryGetValue(process.Code, out var correctName))
            {
                logger.LogInformation("Fixing process name: {Code} '{Old}' → '{New}'", process.Code, process.Name, correctName);
                process.Name = correctName;
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Fixed {Count} stale process name(s).", staleProcesses.Count);
    }

    /// <summary>
    /// Fixes surface finish names that were saved with stale values (e.g. "Dyed (SLA)" → "Dyed").
    /// </summary>
    private static async Task EnsureFinishNamesAsync(MaterialDbContext context, ILogger logger)
    {
        var canonical = ManufacturingCatalogSeedData.GetSurfaceFinishes()
            .ToDictionary(f => f.Code, f => f.Name);
        var canonicalCodes = canonical.Keys.ToList();

        var staleFinishes = await context.SurfaceFinishes
            .Where(f => canonicalCodes.Contains(f.Code))
            .ToListAsync();

        staleFinishes = staleFinishes
            .Where(f => canonical.TryGetValue(f.Code, out var correctName) && correctName != f.Name)
            .ToList();

        if (staleFinishes.Count == 0)
        {
            return;
        }

        foreach (var finish in staleFinishes)
        {
            if (canonical.TryGetValue(finish.Code, out var correctName))
            {
                logger.LogInformation("Fixing finish name: {Code} '{Old}' → '{New}'", finish.Code, finish.Name, correctName);
                finish.Name = correctName;
            }
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Fixed {Count} stale finish name(s).", staleFinishes.Count);
    }

    /// <summary>
    /// Adds any process–finish links that are in the seed data but missing from the DB.
    /// Safe to run repeatedly (idempotent).
    /// </summary>
    private static async Task EnsureProcessFinishLinksAsync(MaterialDbContext context, ILogger logger)
    {
        var seedLinks = ManufacturingCatalogSeedData.GetProcessFinishLinks().ToList();
        var processIds = seedLinks.Select(l => l.ProcessId).Distinct().ToList();
        var finishIds = seedLinks.Select(l => l.FinishId).Distinct().ToList();

        var processes = await context.ManufacturingProcesses
            .Include(p => p.SurfaceFinishes)
            .Where(p => processIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var finishes = await context.SurfaceFinishes
            .Where(f => finishIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id);

        var added = 0;
        foreach (var (processId, finishId) in seedLinks)
        {
            if (!processes.TryGetValue(processId, out var process) || !finishes.TryGetValue(finishId, out var finish))
                continue;

            if (!process.SurfaceFinishes.Any(f => f.Id == finishId))
            {
                process.SurfaceFinishes.Add(finish);
                added++;
            }
        }

        if (added > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Added {Count} missing process–finish link(s).", added);
        }
    }

    private static async Task EnsureMaterialDetailReferenceDataAsync(MaterialDbContext context, ILogger logger)
    {
        var addedColors = 0;
        foreach (var seedColor in ManufacturingCatalogSeedData.GetColors())
        {
            var color = await context.Colors
                .FirstOrDefaultAsync(item => item.Id == seedColor.Id || item.Name == seedColor.Name);

            if (color is null)
            {
                context.Colors.Add(seedColor);
                addedColors++;
                continue;
            }

            if (color.HexCode != seedColor.HexCode)
            {
                color.HexCode = seedColor.HexCode;
            }
        }

        var addedProperties = 0;
        foreach (var seedProperty in ManufacturingCatalogSeedData.GetMechanicalProperties())
        {
            var property = await context.MechanicalProperties
                .FirstOrDefaultAsync(item => item.Id == seedProperty.Id || item.Name == seedProperty.Name);

            if (property is null)
            {
                context.MechanicalProperties.Add(seedProperty);
                addedProperties++;
                continue;
            }

            if (property.Unit != seedProperty.Unit)
            {
                property.Unit = seedProperty.Unit;
            }
        }

        await context.SaveChangesAsync();

        var addedMaterials = await EnsureLateCatalogMaterialsAsync(context, logger);
        var addedProcessMaterialLinks = await EnsureProcessMaterialLinksAsync(context, logger);

        var materialColorLinks = ManufacturingCatalogSeedData.GetMaterialColorLinks().ToList();
        var materialIds = materialColorLinks.Select(link => link.MaterialId).Distinct().ToList();
        var colorIds = materialColorLinks.Select(link => link.ColorId).Distinct().ToList();
        var materials = await context.Materials
            .Include(material => material.AvailableColors)
            .Where(material => materialIds.Contains(material.Id))
            .ToDictionaryAsync(material => material.Id);
        var colors = await context.Colors
            .Where(color => colorIds.Contains(color.Id))
            .ToDictionaryAsync(color => color.Id);

        var addedColorLinks = 0;
        foreach (var (materialId, colorId) in materialColorLinks)
        {
            if (!materials.TryGetValue(materialId, out var material) || !colors.TryGetValue(colorId, out var color))
            {
                continue;
            }

            if (material.AvailableColors.Any(existing => existing.Id == colorId))
            {
                continue;
            }

            material.AvailableColors.Add(color);
            addedColorLinks++;
        }

        var propertyLinks = ManufacturingCatalogSeedData.GetMaterialMechanicalPropertyLinks().ToList();
        var propertyMaterialIds = propertyLinks.Select(link => link.MaterialId).Distinct().ToList();
        var propertyIds = propertyLinks.Select(link => link.MechanicalPropertyId).Distinct().ToList();
        var existingMaterialIds = await context.Materials
            .Where(material => propertyMaterialIds.Contains(material.Id))
            .Select(material => material.Id)
            .ToListAsync();
        var existingPropertyIds = await context.MechanicalProperties
            .Where(property => propertyIds.Contains(property.Id))
            .Select(property => property.Id)
            .ToListAsync();
        var existingPropertyLinks = await context.MaterialMechanicalProperties
            .Where(link => propertyMaterialIds.Contains(link.MaterialId) && propertyIds.Contains(link.MechanicalPropertyId))
            .ToDictionaryAsync(link => (link.MaterialId, link.MechanicalPropertyId));

        var addedPropertyLinks = 0;
        foreach (var (materialId, propertyId, value) in propertyLinks)
        {
            if (!existingMaterialIds.Contains(materialId) || !existingPropertyIds.Contains(propertyId))
            {
                continue;
            }

            if (existingPropertyLinks.TryGetValue((materialId, propertyId), out var existingLink))
            {
                existingLink.Value = value;
                continue;
            }

            context.MaterialMechanicalProperties.Add(new()
            {
                MaterialId = materialId,
                MechanicalPropertyId = propertyId,
                Value = value
            });
            addedPropertyLinks++;
        }

        await context.SaveChangesAsync();

        // ── surface_finish_materials — idempotent ──────────────────────────────
        // Ensure SF_AS_MOLDED exists (may be new).
        var asMoldedSeed = ManufacturingCatalogSeedData.GetSurfaceFinishes()
            .First(f => f.Code == "AS_MOLDED");
        if (!await context.SurfaceFinishes.AnyAsync(f => f.Code == "AS_MOLDED"))
        {
            context.SurfaceFinishes.Add(asMoldedSeed);
            await context.SaveChangesAsync();
            logger.LogInformation("Added new surface finish: AS_MOLDED.");
        }

        var finishLinks = ManufacturingCatalogSeedData.GetMaterialSurfaceFinishLinks().ToList();
        var finishLinkMaterialIds = finishLinks.Select(l => l.MaterialId).Distinct().ToList();
        var finishLinkFinishIds = finishLinks.Select(l => l.FinishId).Distinct().ToList();

        var finishLinkMaterials = await context.Materials
            .Include(m => m.SurfaceFinishes)
            .Where(m => finishLinkMaterialIds.Contains(m.Id))
            .ToDictionaryAsync(m => m.Id);

        var finishLinkFinishes = await context.SurfaceFinishes
            .Where(f => finishLinkFinishIds.Contains(f.Id))
            .ToDictionaryAsync(f => f.Id);

        var addedFinishLinks = 0;
        foreach (var (materialId, finishId) in finishLinks)
        {
            if (!finishLinkMaterials.TryGetValue(materialId, out var material) ||
                !finishLinkFinishes.TryGetValue(finishId, out var finish))
                continue;

            if (!material.SurfaceFinishes.Any(f => f.Id == finishId))
            {
                material.SurfaceFinishes.Add(finish);
                addedFinishLinks++;
            }
        }

        // ── Injection molding tolerance classes — idempotent ──────────────────
        var imTolCodes = new[] { "IM_STD", "IM_FINE" };
        var imTolSeed = ManufacturingCatalogSeedData.GetToleranceClasses()
            .Where(t => imTolCodes.Contains(t.Code)).ToList();
        foreach (var tol in imTolSeed)
        {
            if (!await context.ToleranceClasses.AnyAsync(t => t.Code == tol.Code))
            {
                context.ToleranceClasses.Add(tol);
            }
        }
        await context.SaveChangesAsync();

        // ── Injection molding process config options — idempotent ──────────────
        var imOptKeys = new[] { "draft_angle_deg", "wall_thickness_mm", "gate_type", "surface_texture", "annual_volume" };
        var injectionProcess2 = await context.ManufacturingProcesses
            .FirstOrDefaultAsync(p => p.Code == "INJECTION_MOLD");
        if (injectionProcess2 is not null)
        {
            var existingOptKeys = await context.ProcessConfigOptions
                .Where(o => o.ManufacturingProcessId == injectionProcess2.Id)
                .Select(o => o.ConfigKey)
                .ToListAsync();
            var imOptsToAdd = ManufacturingCatalogSeedData.GetProcessConfigOptions()
                .Where(o => o.ManufacturingProcessId == ManufacturingCatalogSeedData.InjectionMoldId
                         && !existingOptKeys.Contains(o.ConfigKey))
                .ToList();
            if (imOptsToAdd.Count > 0)
            {
                await context.ProcessConfigOptions.AddRangeAsync(imOptsToAdd);
                await context.SaveChangesAsync();
                logger.LogInformation("Added {Count} injection molding config option(s).", imOptsToAdd.Count);
            }
        }

        // ── Injection molding materials — idempotent ───────────────────────────
        var imCodes = new[] { "PP_IM", "ABS_IM", "PCABS_IM", "PA66_IM", "TPE_IM" };
        var existingImCodes = await context.Materials
            .Where(m => imCodes.Contains(m.Code))
            .Select(m => m.Code)
            .ToListAsync();
        var imMaterialsToAdd = ManufacturingCatalogSeedData.GetMaterials()
            .Where(m => imCodes.Contains(m.Code) && !existingImCodes.Contains(m.Code))
            .ToList();
        if (imMaterialsToAdd.Count > 0)
        {
            await context.Materials.AddRangeAsync(imMaterialsToAdd);
            await context.SaveChangesAsync();
            logger.LogInformation("Added {Count} new injection molding material(s).", imMaterialsToAdd.Count);

            // Link to injection molding process.
            var injectionProcess = await context.ManufacturingProcesses
                .Include(p => p.Materials)
                .FirstOrDefaultAsync(p => p.Code == "INJECTION_MOLD");
            if (injectionProcess is not null)
            {
                var newImMaterials = await context.Materials
                    .Where(m => imCodes.Contains(m.Code))
                    .ToListAsync();
                foreach (var mat in newImMaterials)
                {
                    if (!injectionProcess.Materials.Any(m => m.Id == mat.Id))
                        injectionProcess.Materials.Add(mat);
                }
                await context.SaveChangesAsync();
            }
        }

        await context.SaveChangesAsync();

        if (addedColors + addedProperties + addedMaterials + addedColorLinks + addedPropertyLinks + addedFinishLinks + addedProcessMaterialLinks > 0)
        {
            logger.LogInformation(
                "Seeded material detail reference data. Colors={Colors}, properties={Properties}, materials={Materials}, colorLinks={ColorLinks}, propertyLinks={PropertyLinks}, finishLinks={FinishLinks}, processMaterialLinks={ProcessMaterialLinks}.",
                addedColors,
                addedProperties,
                addedMaterials,
                addedColorLinks,
                addedPropertyLinks,
                addedFinishLinks,
                addedProcessMaterialLinks);
        }
    }

    private static async Task<int> EnsureLateCatalogMaterialsAsync(MaterialDbContext context, ILogger logger)
    {
        var lateMaterialCodes = new[] { "ACRYLIC_CLEAR", "CLEAR_RESIN" };
        var existingCodes = await context.Materials
            .Where(material => lateMaterialCodes.Contains(material.Code))
            .Select(material => material.Code)
            .ToListAsync();
        var materialsToAdd = ManufacturingCatalogSeedData.GetMaterials()
            .Where(material => lateMaterialCodes.Contains(material.Code) && !existingCodes.Contains(material.Code))
            .ToList();

        if (materialsToAdd.Count == 0)
        {
            return 0;
        }

        await context.Materials.AddRangeAsync(materialsToAdd);
        await context.SaveChangesAsync();
        logger.LogInformation("Added {Count} late catalog material(s).", materialsToAdd.Count);
        return materialsToAdd.Count;
    }

    private static async Task<int> EnsureProcessMaterialLinksAsync(MaterialDbContext context, ILogger logger)
    {
        var seedLinks = ManufacturingCatalogSeedData.GetProcessMaterialLinks().ToList();
        var processIds = seedLinks.Select(link => link.ProcessId).Distinct().ToList();
        var materialIds = seedLinks.Select(link => link.MaterialId).Distinct().ToList();

        var processes = await context.ManufacturingProcesses
            .Include(process => process.Materials)
            .Where(process => processIds.Contains(process.Id))
            .ToDictionaryAsync(process => process.Id);

        var materials = await context.Materials
            .Where(material => materialIds.Contains(material.Id))
            .ToDictionaryAsync(material => material.Id);

        var added = 0;
        foreach (var (processId, materialId) in seedLinks)
        {
            if (!processes.TryGetValue(processId, out var process) || !materials.TryGetValue(materialId, out var material))
            {
                continue;
            }

            if (process.Materials.Any(existing => existing.Id == materialId))
            {
                continue;
            }

            process.Materials.Add(material);
            added++;
        }

        if (added > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Added {Count} missing process–material link(s).", added);
        }

        return added;
    }
}
