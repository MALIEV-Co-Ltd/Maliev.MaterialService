using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.DbContexts;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace Maliev.MaterialService.Api.Services;

public class MaterialGroupService : IMaterialGroupService
{
    private readonly MaterialDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MaterialGroupService> _logger;
    private readonly CacheOptions _cacheOptions;

    public MaterialGroupService(
        MaterialDbContext context,
        IMemoryCache cache,
        ILogger<MaterialGroupService> logger,
        CacheOptions cacheOptions)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    public async Task<IEnumerable<MaterialGroup>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "material_groups_all";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialGroup>? cachedGroups))
        {
            return cachedGroups!;
        }

        var groups = await _context.MaterialGroups
            .Include(mg => mg.MaterialFamily)
            .OrderBy(mg => mg.MaterialFamily.SortOrder)
            .ThenBy(mg => mg.SortOrder)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, groups, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached {Count} material groups", groups.Count);

        return groups;
    }

    public async Task<PagedResult<MaterialGroup>> GetAllGroupsPagedAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var query = _context.MaterialGroups
            .Include(mg => mg.MaterialFamily)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        var groups = await query
            .OrderBy(mg => mg.MaterialFamilyId)
            .ThenBy(mg => mg.SortOrder)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved paginated material groups - Page {PageNumber}, Size {PageSize}, Total {TotalCount}",
            pagination.PageNumber, pagination.PageSize, totalCount);

        return new PagedResult<MaterialGroup>
        {
            Items = groups,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<MaterialGroup?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"material_group_{id}";

        if (_cache.TryGetValue(cacheKey, out MaterialGroup? cachedGroup))
        {
            return cachedGroup;
        }

        var group = await _context.MaterialGroups
            .Include(mg => mg.MaterialFamily)
            .Include(mg => mg.Materials.Where(m => m.IsActive))
            .FirstOrDefaultAsync(mg => mg.Id == id, cancellationToken);

        if (group != null)
        {
            _cache.Set(cacheKey, group, _cacheOptions.LongExpiration);
        }

        return group;
    }

    public async Task<IEnumerable<MaterialGroup>> GetGroupsByFamilyIdAsync(int familyId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"material_groups_family_{familyId}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialGroup>? cachedGroups))
        {
            return cachedGroups!;
        }

        var groups = await _context.MaterialGroups
            .Include(mg => mg.MaterialFamily)
            .Where(mg => mg.MaterialFamilyId == familyId)
            .OrderBy(mg => mg.SortOrder)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, groups, _cacheOptions.LongExpiration);
        return groups;
    }

    public async Task<PagedResult<MaterialGroup>> GetGroupsByFamilyIdPagedAsync(int familyId, PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var query = _context.MaterialGroups
            .Include(mg => mg.MaterialFamily)
            .Where(mg => mg.MaterialFamilyId == familyId)
            .AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        var groups = await query
            .OrderBy(mg => mg.SortOrder)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        _logger.LogDebug("Retrieved paginated material groups for family {FamilyId} - Page {PageNumber}, Size {PageSize}, Total {TotalCount}",
            familyId, pagination.PageNumber, pagination.PageSize, totalCount);

        return new PagedResult<MaterialGroup>
        {
            Items = groups,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<MaterialGroup> CreateGroupAsync(MaterialGroup group, CancellationToken cancellationToken = default)
    {
        if (await _context.MaterialGroups.AnyAsync(mg => mg.Name == group.Name && mg.MaterialFamilyId == group.MaterialFamilyId, cancellationToken))
        {
            throw new InvalidOperationException($"A material group with name '{group.Name}' already exists in this family.");
        }

        group.CreatedDate = DateTime.UtcNow;
        group.ModifiedDate = DateTime.UtcNow;

        _context.MaterialGroups.Add(group);
        await _context.SaveChangesAsync(cancellationToken);

        ClearGroupCaches();
        _logger.LogInformation("Created material group {Id}: {Name}", group.Id, group.Name);

        return group;
    }

    public async Task<MaterialGroup> UpdateGroupAsync(MaterialGroup group, CancellationToken cancellationToken = default)
    {
        var existing = await _context.MaterialGroups.FindAsync(new object[] { group.Id }, cancellationToken);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Material group with ID {group.Id} not found.");
        }

        // Check for duplicate names (excluding current group)
        if (await _context.MaterialGroups.AnyAsync(mg => mg.Name == group.Name &&
                                                          mg.MaterialFamilyId == group.MaterialFamilyId &&
                                                          mg.Id != group.Id, cancellationToken))
        {
            throw new InvalidOperationException($"A material group with name '{group.Name}' already exists in this family.");
        }

        _context.Entry(existing).CurrentValues.SetValues(group);
        existing.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        ClearGroupCaches();
        _cache.Remove($"material_group_{group.Id}");

        _logger.LogInformation("Updated material group {Id}: {Name}", group.Id, group.Name);
        return existing;
    }

    public async Task DeleteGroupAsync(int id, CancellationToken cancellationToken = default)
    {
        var group = await _context.MaterialGroups
            .Include(mg => mg.Materials)
            .FirstOrDefaultAsync(mg => mg.Id == id, cancellationToken);

        if (group == null)
        {
            throw new KeyNotFoundException($"Material group with ID {id} not found.");
        }

        if (group.Materials.Any())
        {
            throw new InvalidOperationException($"Cannot delete material group '{group.Name}' because it contains {group.Materials.Count} materials. Please move or delete these materials first.");
        }

        _context.MaterialGroups.Remove(group);
        await _context.SaveChangesAsync(cancellationToken);

        ClearGroupCaches();
        _cache.Remove($"material_group_{id}");

        _logger.LogInformation("Deleted material group {Id}: {Name}", id, group.Name);
    }

    public async Task<bool> GroupExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.MaterialGroups.AnyAsync(mg => mg.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MaterialFamily>> GetAllFamiliesAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "material_families_all";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<MaterialFamily>? cachedFamilies))
        {
            return cachedFamilies!;
        }

        var families = await _context.MaterialFamilies
            .OrderBy(mf => mf.SortOrder)
            .ToListAsync(cancellationToken);

        _cache.Set(cacheKey, families, _cacheOptions.LongExpiration);
        _logger.LogDebug("Retrieved and cached {Count} material families", families.Count);

        return families;
    }

    public async Task<MaterialFamily?> GetFamilyByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"material_family_{id}";

        if (_cache.TryGetValue(cacheKey, out MaterialFamily? cachedFamily))
        {
            return cachedFamily;
        }

        var family = await _context.MaterialFamilies
            .Include(mf => mf.MaterialGroups)
            .FirstOrDefaultAsync(mf => mf.Id == id, cancellationToken);

        if (family != null)
        {
            _cache.Set(cacheKey, family, _cacheOptions.LongExpiration);
        }

        return family;
    }

    private void ClearGroupCaches()
    {
        var cacheKeys = new[]
        {
            "material_groups_all"
        };

        foreach (var key in cacheKeys)
        {
            _cache.Remove(key);
        }

        // Also clear family-specific caches
        for (int i = 1; i <= 10; i++) // Assuming max 10 families for now
        {
            _cache.Remove($"material_groups_family_{i}");
        }

        _logger.LogDebug("Cleared material group caches");
    }
}