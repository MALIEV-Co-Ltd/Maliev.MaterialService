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
            // Always run: fix any process rows whose Name was incorrectly set to the Code value.
            await EnsureProcessNamesAsync(context, logger);

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

        if (addedColors + addedProperties + addedColorLinks + addedPropertyLinks > 0)
        {
            logger.LogInformation(
                "Seeded material detail reference data. Colors={Colors}, properties={Properties}, colorLinks={ColorLinks}, propertyLinks={PropertyLinks}.",
                addedColors,
                addedProperties,
                addedColorLinks,
                addedPropertyLinks);
        }
    }
}
