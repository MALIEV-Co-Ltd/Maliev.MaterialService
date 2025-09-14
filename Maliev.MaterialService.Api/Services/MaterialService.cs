using Maliev.MaterialService.Api.Constants;
using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.DbContexts;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Services;

public class MaterialService : IMaterialService
{
    private readonly MaterialDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MaterialService> _logger;
    private readonly CacheOptions _cacheOptions;

    public MaterialService(
        MaterialDbContext context,
        IMemoryCache cache,
        ILogger<MaterialService> logger,
        CacheOptions cacheOptions)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    public async Task<IEnumerable<Material>> GetAllMaterialsAsync(bool includeInactive = false)
    {
        var cacheKey = string.Format(MaterialServiceCacheKeys.AllMaterials, includeInactive);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            _logger.LogDebug("Retrieved {Count} materials from cache", cachedMaterials!.Count());
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        var materials = await query
            .OrderBy(m => m.MaterialGroup.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached {Count} materials", materials.Count);

        return materials;
    }

    public async Task<Material?> GetMaterialByIdAsync(int id)
    {
        var cacheKey = string.Format(MaterialServiceCacheKeys.MaterialById, id);

        if (_cache.TryGetValue(cacheKey, out Material? cachedMaterial))
        {
            _logger.LogDebug("Retrieved material {Id} from cache", id);
            return cachedMaterial;
        }

        var material = await _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .Include(m => m.MaterialProperties)
                .ThenInclude(mp => mp.PropertySubtype)
                    .ThenInclude(ps => ps.PropertyType)
            .Include(m => m.MaterialStandards)
                .ThenInclude(ms => ms.StandardType)
            .Include(m => m.ProcessCompatibilities)
                .ThenInclude(mpc => mpc.Process)
            .Include(m => m.MaterialColors)
                .ThenInclude(mc => mc.Color)
            .Include(m => m.MaterialSurfaceFinishes)
                .ThenInclude(msf => msf.SurfaceFinish)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (material != null)
        {
            _cache.Set(cacheKey, material, _cacheOptions.LongExpiration);
            _logger.LogDebug("Retrieved and cached material {Id}", id);
        }

        return material;
    }

    public async Task<IEnumerable<Material>> GetMaterialsByGroupIdAsync(int groupId, bool includeInactive = false)
    {
        var cacheKey = string.Format(MaterialServiceCacheKeys.MaterialsByGroupId, groupId, includeInactive);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Where(m => m.MaterialGroupId == groupId);

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        var materials = await query
            .OrderBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, _cacheOptions.DefaultExpiration);
        return materials;
    }

    public async Task<IEnumerable<Material>> GetMaterialsByFamilyIdAsync(int familyId, bool includeInactive = false)
    {
        var cacheKey = string.Format(MaterialServiceCacheKeys.MaterialsByFamilyId, familyId, includeInactive);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Where(m => m.MaterialGroup.MaterialFamilyId == familyId);

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        var materials = await query
            .OrderBy(m => m.MaterialGroup.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, _cacheOptions.DefaultExpiration);
        return materials;
    }

    public async Task<IEnumerable<Material>> SearchMaterialsAsync(string searchTerm, bool includeInactive = false)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllMaterialsAsync(includeInactive);
        }

        var normalizedSearchTerm = searchTerm.Trim().ToLowerInvariant();
        var cacheKey = string.Format(MaterialServiceCacheKeys.MaterialSearch, normalizedSearchTerm, includeInactive);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .Where(m =>
                m.Name.ToLower().Contains(normalizedSearchTerm) ||
                (m.Description != null && m.Description.ToLower().Contains(normalizedSearchTerm)) ||
                (m.MaterialNumber != null && m.MaterialNumber.ToLower().Contains(normalizedSearchTerm)) ||
                (m.ManufacturerReference != null && m.ManufacturerReference.ToLower().Contains(normalizedSearchTerm)) ||
                m.MaterialGroup.Name.ToLower().Contains(normalizedSearchTerm));

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        var materials = await query
            .OrderBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, TimeSpan.FromMinutes(15)); // Shorter cache for search results
        return materials;
    }

    public async Task<IEnumerable<Material>> GetMaterialsByProcessCompatibilityAsync(int processId, bool includeInactive = false)
    {
        var cacheKey = $"materials_process_{processId}_{includeInactive}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Include(m => m.ProcessCompatibilities)
            .Where(m => m.ProcessCompatibilities.Any(mpc => mpc.ProcessId == processId));

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        var materials = await query
            .OrderByDescending(m => m.ProcessCompatibilities
                .Where(mpc => mpc.ProcessId == processId)
                .Max(mpc => mpc.CompatibilityLevel))
            .ThenBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, _cacheOptions.DefaultExpiration);
        return materials;
    }

    public async Task<Material> CreateMaterialAsync(Material material)
    {
        if (await _context.Materials.AnyAsync(m => m.Name == material.Name && m.MaterialGroupId == material.MaterialGroupId))
        {
            throw new InvalidOperationException($"A material with name '{material.Name}' already exists in this material group.");
        }

        material.CreatedDate = DateTime.UtcNow;
        material.ModifiedDate = DateTime.UtcNow;

        _context.Materials.Add(material);
        await _context.SaveChangesAsync();

        // Clear relevant caches
        ClearMaterialCaches();

        _logger.LogInformation("Created material {Id}: {Name}", material.Id, material.Name);
        return material;
    }

    public async Task<Material> UpdateMaterialAsync(Material material)
    {
        var existingMaterial = await _context.Materials.FindAsync(material.Id);
        if (existingMaterial == null)
        {
            throw new KeyNotFoundException($"Material with ID {material.Id} not found.");
        }

        // Check for duplicate names (excluding current material)
        if (await _context.Materials.AnyAsync(m => m.Name == material.Name &&
                                                   m.MaterialGroupId == material.MaterialGroupId &&
                                                   m.Id != material.Id))
        {
            throw new InvalidOperationException($"A material with name '{material.Name}' already exists in this material group.");
        }

        // Update properties
        _context.Entry(existingMaterial).CurrentValues.SetValues(material);
        existingMaterial.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Clear relevant caches
        ClearMaterialCaches();
        _cache.Remove($"material_{material.Id}");

        _logger.LogInformation("Updated material {Id}: {Name}", material.Id, material.Name);
        return existingMaterial;
    }

    public async Task DeleteMaterialAsync(int id)
    {
        var material = await _context.Materials.FindAsync(id);
        if (material == null)
        {
            throw new KeyNotFoundException($"Material with ID {id} not found.");
        }

        _context.Materials.Remove(material);
        await _context.SaveChangesAsync();

        // Clear relevant caches
        ClearMaterialCaches();
        _cache.Remove($"material_{id}");

        _logger.LogInformation("Deleted material {Id}: {Name}", id, material.Name);
    }

    public async Task<bool> MaterialExistsAsync(int id)
    {
        return await _context.Materials.AnyAsync(m => m.Id == id);
    }

    // Material Properties
    public async Task<IEnumerable<MaterialProperty>> GetMaterialPropertiesAsync(int materialId)
    {
        var cacheKey = $"material_properties_{materialId}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialProperty>? cachedProperties))
        {
            return cachedProperties!;
        }

        var properties = await _context.MaterialProperties
            .Include(mp => mp.PropertySubtype)
                .ThenInclude(ps => ps.PropertyType)
            .Where(mp => mp.MaterialId == materialId)
            .OrderBy(mp => mp.PropertySubtype.PropertyType.Category)
            .ThenBy(mp => mp.PropertySubtype.SortOrder)
            .ToListAsync();

        _cache.Set(cacheKey, properties, _cacheOptions.DefaultExpiration);
        return properties;
    }

    public async Task<MaterialProperty> AddMaterialPropertyAsync(MaterialProperty property)
    {
        property.CreatedDate = DateTime.UtcNow;
        property.ModifiedDate = DateTime.UtcNow;

        _context.MaterialProperties.Add(property);
        await _context.SaveChangesAsync();

        _cache.Remove($"material_properties_{property.MaterialId}");
        _cache.Remove($"material_{property.MaterialId}");

        return property;
    }

    public async Task UpdateMaterialPropertyAsync(MaterialProperty property)
    {
        var existing = await _context.MaterialProperties.FindAsync(property.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Material property with ID {property.Id} not found.");
        }

        _context.Entry(existing).CurrentValues.SetValues(property);
        existing.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _cache.Remove($"material_properties_{property.MaterialId}");
        _cache.Remove($"material_{property.MaterialId}");
    }

    public async Task DeleteMaterialPropertyAsync(int propertyId)
    {
        var property = await _context.MaterialProperties.FindAsync(propertyId);
        if (property == null)
        {
            throw new KeyNotFoundException($"Material property with ID {propertyId} not found.");
        }

        var materialId = property.MaterialId;
        _context.MaterialProperties.Remove(property);
        await _context.SaveChangesAsync();

        _cache.Remove($"material_properties_{materialId}");
        _cache.Remove($"material_{materialId}");
    }

    // Material Standards
    public async Task<IEnumerable<MaterialStandard>> GetMaterialStandardsAsync(int materialId)
    {
        return await _context.MaterialStandards
            .Include(ms => ms.StandardType)
            .Where(ms => ms.MaterialId == materialId)
            .OrderBy(ms => ms.StandardType.Code)
            .ToListAsync();
    }

    public async Task<MaterialStandard> AddMaterialStandardAsync(MaterialStandard standard)
    {
        standard.CreatedDate = DateTime.UtcNow;
        standard.ModifiedDate = DateTime.UtcNow;

        _context.MaterialStandards.Add(standard);
        await _context.SaveChangesAsync();

        _cache.Remove($"material_{standard.MaterialId}");
        return standard;
    }

    public async Task UpdateMaterialStandardAsync(MaterialStandard standard)
    {
        var existing = await _context.MaterialStandards.FindAsync(standard.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Material standard with ID {standard.Id} not found.");
        }

        _context.Entry(existing).CurrentValues.SetValues(standard);
        existing.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _cache.Remove($"material_{standard.MaterialId}");
    }

    public async Task DeleteMaterialStandardAsync(int standardId)
    {
        var standard = await _context.MaterialStandards.FindAsync(standardId);
        if (standard == null)
        {
            throw new KeyNotFoundException($"Material standard with ID {standardId} not found.");
        }

        var materialId = standard.MaterialId;
        _context.MaterialStandards.Remove(standard);
        await _context.SaveChangesAsync();

        _cache.Remove($"material_{materialId}");
    }

    // Process Compatibility
    public async Task<IEnumerable<MaterialProcessCompatibility>> GetMaterialProcessCompatibilityAsync(int materialId)
    {
        return await _context.MaterialProcessCompatibilities
            .Include(mpc => mpc.Process)
                .ThenInclude(p => p.Category)
            .Where(mpc => mpc.MaterialId == materialId)
            .OrderBy(mpc => mpc.Process.Category.SortOrder)
            .ThenBy(mpc => mpc.Process.SortOrder)
            .ToListAsync();
    }

    public async Task<MaterialProcessCompatibility> AddProcessCompatibilityAsync(MaterialProcessCompatibility compatibility)
    {
        compatibility.CreatedDate = DateTime.UtcNow;
        compatibility.ModifiedDate = DateTime.UtcNow;

        _context.MaterialProcessCompatibilities.Add(compatibility);
        await _context.SaveChangesAsync();

        ClearProcessCompatibilityCaches(compatibility.MaterialId, compatibility.ProcessId);
        return compatibility;
    }

    public async Task UpdateProcessCompatibilityAsync(MaterialProcessCompatibility compatibility)
    {
        var existing = await _context.MaterialProcessCompatibilities.FindAsync(compatibility.Id);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Process compatibility with ID {compatibility.Id} not found.");
        }

        _context.Entry(existing).CurrentValues.SetValues(compatibility);
        existing.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        ClearProcessCompatibilityCaches(compatibility.MaterialId, compatibility.ProcessId);
    }

    public async Task DeleteProcessCompatibilityAsync(int compatibilityId)
    {
        var compatibility = await _context.MaterialProcessCompatibilities.FindAsync(compatibilityId);
        if (compatibility == null)
        {
            throw new KeyNotFoundException($"Process compatibility with ID {compatibilityId} not found.");
        }

        var materialId = compatibility.MaterialId;
        var processId = compatibility.ProcessId;

        _context.MaterialProcessCompatibilities.Remove(compatibility);
        await _context.SaveChangesAsync();

        ClearProcessCompatibilityCaches(materialId, processId);
    }

    private void ClearMaterialCaches()
    {
        // This is a simple approach - in production, you might want to use cache tags or patterns
        var cacheKeys = new[]
        {
            "materials_all_true",
            "materials_all_false"
        };

        foreach (var key in cacheKeys)
        {
            _cache.Remove(key);
        }

        _logger.LogDebug("Cleared material caches");
    }

    private void ClearProcessCompatibilityCaches(int materialId, int processId)
    {
        _cache.Remove($"material_{materialId}");
        _cache.Remove($"materials_process_{processId}_true");
        _cache.Remove($"materials_process_{processId}_false");
    }
}