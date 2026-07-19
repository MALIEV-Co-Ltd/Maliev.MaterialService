using Maliev.MaterialService.Application.DTOs.Bulk;
using Maliev.MaterialService.Application.DTOs.Materials;
using Maliev.MaterialService.Application.Services;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Infrastructure.Services;

/// <summary>
/// Implementation of bulk material operations.
/// </summary>
public class BulkMaterialService : IBulkMaterialService
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<BulkMaterialService> _logger;

    /// <summary>
    /// Initializes a new instance of BulkMaterialService.
    /// </summary>
    /// <param name="materialService">Material service.</param>
    /// <param name="logger">Logger instance.</param>
    public BulkMaterialService(
        IMaterialService materialService,
        ILogger<BulkMaterialService> logger)
    {
        _materialService = materialService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<BulkImportResponse> BulkImportMaterialsAsync(BulkImportRequest request, string userId)
    {
        _logger.LogInformation("Starting bulk import of {Count} materials. DryRun: {DryRun}, ValidateOnly: {ValidateOnly}",
            request.Materials.Count, request.DryRun, request.ValidateOnly);

        var response = new BulkImportResponse
        {
            TotalCount = request.Materials.Count
        };

        for (var i = 0; i < request.Materials.Count; i++)
        {
            var material = request.Materials[i];

            try
            {
                if (request.ValidateOnly || request.DryRun)
                {
                    response.SuccessCount++;
                    continue;
                }

                var created = await _materialService.CreateMaterialAsync(material, userId);
                response.SuccessCount++;
                response.CreatedMaterials.Add(created);

                _logger.LogDebug("Successfully imported material {Index}/{Total}: {Code}",
                    i + 1, request.Materials.Count, material.Code);
            }
            catch (Exception ex)
            {
                response.FailureCount++;
                response.Errors.Add(new BulkImportError
                {
                    Index = i,
                    MaterialCode = material.Code,
                    Error = ex.Message
                });

                _logger.LogWarning(ex, "Failed to import material {Index}/{Total}: {Code}",
                    i + 1, request.Materials.Count, material.Code);
            }
        }

        _logger.LogInformation("Bulk import completed. Success: {Success}, Failures: {Failures}",
            response.SuccessCount, response.FailureCount);

        return response;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<MaterialResponse>> BulkExportMaterialsAsync()
    {
        _logger.LogInformation("Starting bulk export of all materials");
        var materials = await _materialService.GetAllMaterialsAsync();
        _logger.LogInformation("Bulk export completed. Total materials: {Count}", materials.Count());
        return materials;
    }
}
