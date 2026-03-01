using Maliev.MaterialService.Application.DTOs.Bulk;
using Maliev.MaterialService.Application.DTOs.Materials;

namespace Maliev.MaterialService.Application.Services;

/// <summary>
/// Service for handling bulk operations on materials.
/// </summary>
public interface IBulkMaterialService
{
    /// <summary>
    /// Imports multiple materials in a single batch.
    /// </summary>
    /// <param name="request">The bulk import request containing materials data.</param>
    /// <param name="userId">ID of the user performing the import.</param>
    /// <returns>Response detailing success and failure counts.</returns>
    Task<BulkImportResponse> BulkImportMaterialsAsync(BulkImportRequest request, string userId);

    /// <summary>
    /// Exports all active materials.
    /// </summary>
    /// <returns>Collection of all materials.</returns>
    Task<IEnumerable<MaterialResponse>> BulkExportMaterialsAsync();
}
