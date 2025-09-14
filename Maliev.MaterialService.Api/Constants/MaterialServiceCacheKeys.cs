namespace Maliev.MaterialService.Api.Constants;

/// <summary>
/// Constants for cache keys used in the MaterialService.
/// </summary>
public static class MaterialServiceCacheKeys
{
    /// <summary>
    /// Cache key for all materials.
    /// Format: materials_all_{includeInactive}
    /// </summary>
    public const string AllMaterials = "materials_all_{0}";

    /// <summary>
    /// Cache key for materials by group ID.
    /// Format: materials_group_{groupId}_{includeInactive}
    /// </summary>
    public const string MaterialsByGroupId = "materials_group_{0}_{1}";

    /// <summary>
    /// Cache key for materials by family ID.
    /// Format: materials_family_{familyId}_{includeInactive}
    /// </summary>
    public const string MaterialsByFamilyId = "materials_family_{0}_{1}";

    /// <summary>
    /// Cache key for a single material by ID.
    /// Format: material_{id}
    /// </summary>
    public const string MaterialById = "material_{0}";

    /// <summary>
    /// Cache key for material search results.
    /// Format: materials_search_{searchTerm}_{includeInactive}
    /// </summary>
    public const string MaterialSearch = "materials_search_{0}_{1}";
}