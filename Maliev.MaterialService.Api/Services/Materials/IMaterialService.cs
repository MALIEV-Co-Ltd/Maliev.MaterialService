using Maliev.MaterialService.Api.DTOs;
using Maliev.MaterialService.Api.DTOs.Materials;

namespace Maliev.MaterialService.Api.Services.Materials;

/// <summary>
/// Service for managing materials
/// </summary>
public interface IMaterialService
{
    /// <summary>
    /// Creates a new material
    /// </summary>
    /// <param name="request">Material creation request</param>
    /// <param name="userId">ID of the user creating the material</param>
    /// <returns>The created material</returns>
    Task<MaterialResponse> CreateMaterialAsync(CreateMaterialRequest request, string userId);

    /// <summary>
    /// Retrieves a material by its ID
    /// </summary>
    /// <param name="id">Material ID</param>
    /// <returns>The material if found, null otherwise</returns>
    Task<MaterialResponse?> GetMaterialByIdAsync(Guid id);

    /// <summary>
    /// Updates an existing material
    /// </summary>
    /// <param name="id">Material ID</param>
    /// <param name="request">Update request</param>
    /// <param name="userId">ID of the user updating the material</param>
    /// <returns>The updated material if found, null otherwise</returns>
    Task<MaterialResponse?> UpdateMaterialAsync(Guid id, UpdateMaterialRequest request, string userId);

    /// <summary>
    /// Deletes a material (soft delete)
    /// </summary>
    /// <param name="id">Material ID</param>
    /// <returns>True if deleted, false if not found</returns>
    Task<bool> DeleteMaterialAsync(Guid id);

    /// <summary>
    /// Retrieves all materials
    /// </summary>
    /// <returns>Collection of all materials</returns>
    Task<IEnumerable<MaterialResponse>> GetAllMaterialsAsync();

    /// <summary>
    /// Retrieves a paged list of materials with filtering and sorting
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <param name="searchTerm">Search term for name or code</param>
    /// <param name="sortBy">Field to sort by</param>
    /// <param name="sortDescending">Sort direction</param>
    /// <param name="minPrice">Minimum price filter</param>
    /// <param name="maxPrice">Maximum price filter</param>
    /// <param name="supplierId">Supplier ID filter</param>
    /// <param name="manufacturingProcess">Filter by manufacturing process name</param>
    /// <param name="color">Filter by color name</param>
    /// <param name="minTensileStrength">Minimum tensile strength filter</param>
    /// <param name="maxTensileStrength">Maximum tensile strength filter</param>
    /// <returns>Paged result of materials</returns>
    Task<PagedResult<MaterialResponse>> GetMaterialsAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Guid? supplierId = null,
        string? manufacturingProcess = null,
        string? color = null,
        decimal? minTensileStrength = null,
        decimal? maxTensileStrength = null);

    /// <summary>
    /// Gets the count of materials that reference a specific supplier
    /// </summary>
    /// <param name="supplierId">Supplier ID to check</param>
    /// <returns>Number of materials referencing the supplier</returns>
    Task<int> GetSupplierReferenceCountAsync(Guid supplierId);
}
