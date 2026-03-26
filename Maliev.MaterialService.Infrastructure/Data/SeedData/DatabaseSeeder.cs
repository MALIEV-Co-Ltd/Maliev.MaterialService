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
            if (await context.ManufacturingProcesses.AnyAsync())
            {
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
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the manufacturing catalog.");
        }
    }
}
