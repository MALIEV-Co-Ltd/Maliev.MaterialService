using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;
using System.Threading;

namespace Maliev.MaterialService.Api.Services;

public interface IMaterialService
{
    Task<IEnumerable<Material>> GetAllMaterialsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<PagedResult<Material>> GetAllMaterialsPagedAsync(PaginationParameters pagination, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Material?> GetMaterialByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Material>> GetMaterialsByGroupIdAsync(int groupId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<PagedResult<Material>> GetMaterialsByGroupIdPagedAsync(int groupId, PaginationParameters pagination, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Material>> GetMaterialsByFamilyIdAsync(int familyId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<PagedResult<Material>> GetMaterialsByFamilyIdPagedAsync(int familyId, PaginationParameters pagination, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Material>> SearchMaterialsAsync(string searchTerm, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<PagedResult<Material>> SearchMaterialsPagedAsync(string searchTerm, PaginationParameters pagination, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<IEnumerable<Material>> GetMaterialsByProcessCompatibilityAsync(int processId, bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Material> CreateMaterialAsync(Material material, CancellationToken cancellationToken = default);
    Task<Material> UpdateMaterialAsync(Material material, CancellationToken cancellationToken = default);
    Task DeleteMaterialAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> MaterialExistsAsync(int id, CancellationToken cancellationToken = default);

    // Material properties
    Task<IEnumerable<MaterialProperty>> GetMaterialPropertiesAsync(int materialId, CancellationToken cancellationToken = default);
    Task<MaterialProperty> AddMaterialPropertyAsync(MaterialProperty property, CancellationToken cancellationToken = default);
    Task UpdateMaterialPropertyAsync(MaterialProperty property, CancellationToken cancellationToken = default);
    Task DeleteMaterialPropertyAsync(int propertyId, CancellationToken cancellationToken = default);

    // Material standards
    Task<IEnumerable<MaterialStandard>> GetMaterialStandardsAsync(int materialId, CancellationToken cancellationToken = default);
    Task<MaterialStandard> AddMaterialStandardAsync(MaterialStandard standard, CancellationToken cancellationToken = default);
    Task UpdateMaterialStandardAsync(MaterialStandard standard, CancellationToken cancellationToken = default);
    Task DeleteMaterialStandardAsync(int standardId, CancellationToken cancellationToken = default);

    // Process compatibility
    Task<IEnumerable<MaterialProcessCompatibility>> GetMaterialProcessCompatibilityAsync(int materialId, CancellationToken cancellationToken = default);
    Task<MaterialProcessCompatibility> AddProcessCompatibilityAsync(MaterialProcessCompatibility compatibility, CancellationToken cancellationToken = default);
    Task UpdateProcessCompatibilityAsync(MaterialProcessCompatibility compatibility, CancellationToken cancellationToken = default);
    Task DeleteProcessCompatibilityAsync(int compatibilityId, CancellationToken cancellationToken = default);
}