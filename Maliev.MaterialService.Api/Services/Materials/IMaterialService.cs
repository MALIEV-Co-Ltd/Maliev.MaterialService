using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <returns>Paged result of materials</returns>
    Task<PagedResult<MaterialResponse>> GetMaterialsAsync(
        int page = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? sortBy = null,
        bool sortDescending = false,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        Guid? supplierId = null);
}

/// <summary>
/// Generic paged result container
/// </summary>
/// <typeparam name="T">Type of items</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Collection of items for the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
