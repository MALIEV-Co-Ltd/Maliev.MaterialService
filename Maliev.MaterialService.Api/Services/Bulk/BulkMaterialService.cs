using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;
using Maliev.MaterialService.Api.Services.Materials;
using Maliev.MaterialService.Api.Validators.Materials;
using Microsoft.Extensions.Logging;

namespace Maliev.MaterialService.Api.Services.Bulk;

/// <summary>
/// Implementation of bulk material operations
/// </summary>
public class BulkMaterialService : IBulkMaterialService
{
    private readonly IMaterialService _materialService;
    private readonly ILogger<BulkMaterialService> _logger;
    private readonly IValidator<CreateMaterialRequest> _validator;

    /// <summary>
    /// Initializes a new instance of BulkMaterialService
    /// </summary>
    /// <param name="materialService">Material service</param>
    /// <param name="validator">Validator for material creation requests</param>
    /// <param name="logger">Logger instance</param>
    public BulkMaterialService(
        IMaterialService materialService,
        IValidator<CreateMaterialRequest> validator,
        ILogger<BulkMaterialService> logger)
    {
        _materialService = materialService;
        _validator = validator;
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
                // Validate
                var validationResult = await _validator.ValidateAsync(material);
                if (!validationResult.IsValid)
                {
                    response.FailureCount++;
                    response.Errors.Add(new BulkImportError
                    {
                        Index = i,
                        MaterialCode = material.Code,
                        Error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                    });
                    continue;
                }

                // Skip actual import if validate-only or dry-run
                if (request.ValidateOnly || request.DryRun)
                {
                    response.SuccessCount++;
                    continue;
                }

                // Import material
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
