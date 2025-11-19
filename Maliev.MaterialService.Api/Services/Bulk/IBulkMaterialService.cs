using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Maliev.MaterialService.Api.DTOs.Bulk;
using Maliev.MaterialService.Api.DTOs.Materials;

namespace Maliev.MaterialService.Api.Services.Bulk;

/// <summary>
/// Service for handling bulk operations on materials
/// </summary>
public interface IBulkMaterialService
{
    /// <summary>
    /// Imports multiple materials in a single batch
    /// </summary>
    /// <param name="request">The bulk import request containing materials data</param>
    /// <param name="userId">ID of the user performing the import</param>
    /// <returns>Response detailing success and failure counts</returns>
    Task<BulkImportResponse> BulkImportMaterialsAsync(BulkImportRequest request, string userId);

    /// <summary>
    /// Exports all materials
    /// </summary>
    /// <returns>Collection of all materials</returns>
    Task<IEnumerable<MaterialResponse>> BulkExportMaterialsAsync();
}
