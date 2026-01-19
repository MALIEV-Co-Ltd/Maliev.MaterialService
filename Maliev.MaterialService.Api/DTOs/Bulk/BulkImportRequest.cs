using Maliev.MaterialService.Api.DTOs.Materials;

namespace Maliev.MaterialService.Api.DTOs.Bulk;

/// <summary>
/// Request for bulk importing materials
/// </summary>
public class BulkImportRequest
{
    /// <summary>
    /// List of materials to import
    /// </summary>
    public List<CreateMaterialRequest> Materials { get; set; } = new();

    /// <summary>
    /// Whether to run in dry-run mode (no actual import)
    /// </summary>
    public bool DryRun { get; set; } = false;

    /// <summary>
    /// Whether to only validate without importing
    /// </summary>
    public bool ValidateOnly { get; set; } = false;
}

/// <summary>
/// Response from bulk import operation
/// </summary>
public class BulkImportResponse
{
    /// <summary>
    /// Total materials in request
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Successfully imported materials
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Failed imports
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// List of errors that occurred
    /// </summary>
    public List<BulkImportError> Errors { get; set; } = new();

    /// <summary>
    /// List of successfully created materials
    /// </summary>
    public List<MaterialResponse> CreatedMaterials { get; set; } = new();
}

/// <summary>
/// Error information for a failed bulk import
/// </summary>
public class BulkImportError
{
    /// <summary>
    /// Index of the material in the request
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// Material code that failed
    /// </summary>
    public string MaterialCode { get; set; } = string.Empty;

    /// <summary>
    /// Error message
    /// </summary>
    public string Error { get; set; } = string.Empty;
}
