using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

public interface IMaterialService
{
    Task<IEnumerable<Material>> GetAllMaterialsAsync(bool includeInactive = false);
    Task<PagedResult<Material>> GetAllMaterialsPagedAsync(PaginationParameters pagination, bool includeInactive = false);
    Task<Material?> GetMaterialByIdAsync(int id);
    Task<IEnumerable<Material>> GetMaterialsByGroupIdAsync(int groupId, bool includeInactive = false);
    Task<PagedResult<Material>> GetMaterialsByGroupIdPagedAsync(int groupId, PaginationParameters pagination, bool includeInactive = false);
    Task<IEnumerable<Material>> GetMaterialsByFamilyIdAsync(int familyId, bool includeInactive = false);
    Task<IEnumerable<Material>> SearchMaterialsAsync(string searchTerm, bool includeInactive = false);
    Task<IEnumerable<Material>> GetMaterialsByProcessCompatibilityAsync(int processId, bool includeInactive = false);
    Task<Material> CreateMaterialAsync(Material material);
    Task<Material> UpdateMaterialAsync(Material material);
    Task DeleteMaterialAsync(int id);
    Task<bool> MaterialExistsAsync(int id);

    // Material properties
    Task<IEnumerable<MaterialProperty>> GetMaterialPropertiesAsync(int materialId);
    Task<MaterialProperty> AddMaterialPropertyAsync(MaterialProperty property);
    Task UpdateMaterialPropertyAsync(MaterialProperty property);
    Task DeleteMaterialPropertyAsync(int propertyId);

    // Material standards
    Task<IEnumerable<MaterialStandard>> GetMaterialStandardsAsync(int materialId);
    Task<MaterialStandard> AddMaterialStandardAsync(MaterialStandard standard);
    Task UpdateMaterialStandardAsync(MaterialStandard standard);
    Task DeleteMaterialStandardAsync(int standardId);

    // Process compatibility
    Task<IEnumerable<MaterialProcessCompatibility>> GetMaterialProcessCompatibilityAsync(int materialId);
    Task<MaterialProcessCompatibility> AddProcessCompatibilityAsync(MaterialProcessCompatibility compatibility);
    Task UpdateProcessCompatibilityAsync(MaterialProcessCompatibility compatibility);
    Task DeleteProcessCompatibilityAsync(int compatibilityId);
}