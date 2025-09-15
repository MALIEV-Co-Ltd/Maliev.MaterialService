using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.DbContexts;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Services;

public class ManufacturingProcessService : IManufacturingProcessService
{
    private readonly MaterialDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ManufacturingProcessService> _logger;
    private readonly CacheOptions _cacheOptions;

    public ManufacturingProcessService(
        MaterialDbContext context,
        IMemoryCache cache,
        ILogger<ManufacturingProcessService> logger,
        CacheOptions cacheOptions)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    public async Task<IEnumerable<ManufacturingProcess>> GetAllProcessesAsync()
    {
        const string cacheKey = "manufacturing_processes_all";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<ManufacturingProcess>? cachedProcesses))
        {
            return cachedProcesses!;
        }

        var processes = await _context.ManufacturingProcesses
            .Include(mp => mp.Category)
            .OrderBy(mp => mp.Category.SortOrder)
            .ThenBy(mp => mp.SortOrder)
            .ToListAsync();

        _cache.Set(cacheKey, processes, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached {Count} manufacturing processes", processes.Count);

        return processes;
    }

    public async Task<PagedResult<ManufacturingProcess>> GetAllProcessesPagedAsync(PaginationParameters pagination)
    {
        var query = _context.ManufacturingProcesses
            .Include(mp => mp.Category)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        var processes = await query
            .OrderBy(mp => mp.CategoryId)
            .ThenBy(mp => mp.SortOrder)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        _logger.LogDebug("Retrieved paginated manufacturing processes - Page {PageNumber}, Size {PageSize}, Total {TotalCount}",
            pagination.PageNumber, pagination.PageSize, totalCount);

        return new PagedResult<ManufacturingProcess>
        {
            Items = processes,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<ManufacturingProcess?> GetProcessByIdAsync(int id)
    {
        var cacheKey = $"manufacturing_process_{id}";

        if (_cache.TryGetValue(cacheKey, out ManufacturingProcess? cachedProcess))
        {
            return cachedProcess;
        }

        var process = await _context.ManufacturingProcesses
            .Include(mp => mp.Category)
            .FirstOrDefaultAsync(mp => mp.Id == id);

        if (process != null)
        {
            _cache.Set(cacheKey, process, _cacheOptions.LongExpiration);
        }

        return process;
    }

    public async Task<IEnumerable<ManufacturingProcess>> GetProcessesByCategoryIdAsync(int categoryId)
    {
        var cacheKey = $"manufacturing_processes_category_{categoryId}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<ManufacturingProcess>? cachedProcesses))
        {
            return cachedProcesses!;
        }

        var processes = await _context.ManufacturingProcesses
            .Include(mp => mp.Category)
            .Where(mp => mp.CategoryId == categoryId)
            .OrderBy(mp => mp.SortOrder)
            .ToListAsync();

        _cache.Set(cacheKey, processes, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved and cached {Count} manufacturing processes for category {CategoryId}", processes.Count, categoryId);

        return processes;
    }

    public async Task<PagedResult<ManufacturingProcess>> GetProcessesByCategoryIdPagedAsync(int categoryId, PaginationParameters pagination)
    {
        var query = _context.ManufacturingProcesses
            .Include(mp => mp.Category)
            .Where(mp => mp.CategoryId == categoryId)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

        var processes = await query
            .OrderBy(mp => mp.SortOrder)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        _logger.LogDebug("Retrieved paginated manufacturing processes for category {CategoryId} - Page {PageNumber}, Size {PageSize}, Total {TotalCount}",
            categoryId, pagination.PageNumber, pagination.PageSize, totalCount);

        return new PagedResult<ManufacturingProcess>
        {
            Items = processes,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };
    }

    public async Task<IEnumerable<ManufacturingProcessCategory>> GetAllCategoriesAsync()
    {
        const string cacheKey = "manufacturing_process_categories_all";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<ManufacturingProcessCategory>? cachedCategories))
        {
            return cachedCategories!;
        }

        var categories = await _context.ManufacturingProcessCategories
            .Include(mpc => mpc.ManufacturingProcesses)
            .OrderBy(mpc => mpc.SortOrder)
            .ToListAsync();

        _cache.Set(cacheKey, categories, _cacheOptions.LongExpiration);
        _logger.LogDebug("Retrieved and cached {Count} manufacturing process categories", categories.Count);

        return categories;
    }

    public async Task<ManufacturingProcessCategory?> GetCategoryByIdAsync(int id)
    {
        var cacheKey = $"manufacturing_process_category_{id}";

        if (_cache.TryGetValue(cacheKey, out ManufacturingProcessCategory? cachedCategory))
        {
            return cachedCategory;
        }

        var category = await _context.ManufacturingProcessCategories
            .Include(mpc => mpc.ManufacturingProcesses)
            .FirstOrDefaultAsync(mpc => mpc.Id == id);

        if (category != null)
        {
            _cache.Set(cacheKey, category, _cacheOptions.LongExpiration);
        }

        return category;
    }

    public async Task<IEnumerable<Material>> GetCompatibleMaterialsAsync(int processId, int? compatibilityLevel = null)
    {
        var cacheKey = $"compatible_materials_{processId}_{compatibilityLevel}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedMaterials))
        {
            return cachedMaterials!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .Include(m => m.ProcessCompatibilities)
            .Where(m => m.IsActive &&
                       m.ProcessCompatibilities.Any(mpc => mpc.ProcessId == processId));

        if (compatibilityLevel.HasValue)
        {
            query = query.Where(m => m.ProcessCompatibilities
                .Any(mpc => mpc.ProcessId == processId && mpc.CompatibilityLevel >= compatibilityLevel.Value));
        }

        var materials = await query
            .OrderByDescending(m => m.ProcessCompatibilities
                .Where(mpc => mpc.ProcessId == processId)
                .Max(mpc => mpc.CompatibilityLevel))
            .ThenBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, materials, _cacheOptions.DefaultExpiration);
        _logger.LogDebug("Retrieved {Count} compatible materials for process {ProcessId}", materials.Count, processId);

        return materials;
    }

    /// <summary>
    /// Creates a new manufacturing process.
    /// </summary>
    /// <param name="process">The manufacturing process to create.</param>
    /// <returns>The created manufacturing process.</returns>
    public async Task<ManufacturingProcess> CreateProcessAsync(ManufacturingProcess process)
    {
        _logger.LogInformation("Creating new manufacturing process: {Name}", process.Name);

        process.CreatedDate = DateTime.UtcNow;
        process.ModifiedDate = DateTime.UtcNow;

        _context.ManufacturingProcesses.Add(process);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created manufacturing process with ID: {Id}", process.Id);

        return process;
    }

    /// <summary>
    /// Updates an existing manufacturing process.
    /// </summary>
    /// <param name="process">The manufacturing process to update.</param>
    /// <returns>The updated manufacturing process.</returns>
    public async Task<ManufacturingProcess> UpdateProcessAsync(ManufacturingProcess process)
    {
        _logger.LogInformation("Updating manufacturing process ID: {Id}", process.Id);

        var existingProcess = await _context.ManufacturingProcesses.FindAsync(process.Id);
        if (existingProcess == null)
        {
            _logger.LogWarning("Manufacturing process not found for update with ID: {Id}", process.Id);
            throw new KeyNotFoundException($"Manufacturing process with ID {process.Id} not found");
        }

        // Update the properties
        _context.Entry(existingProcess).CurrentValues.SetValues(process);
        existingProcess.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated manufacturing process with ID: {Id}", process.Id);

        return existingProcess;
    }

    /// <summary>
    /// Deletes a manufacturing process by ID.
    /// </summary>
    /// <param name="id">The ID of the manufacturing process to delete.</param>
    public async Task DeleteProcessAsync(int id)
    {
        _logger.LogInformation("Deleting manufacturing process ID: {Id}", id);

        var process = await _context.ManufacturingProcesses.FindAsync(id);
        if (process == null)
        {
            _logger.LogWarning("Manufacturing process not found for deletion with ID: {Id}", id);
            throw new KeyNotFoundException($"Manufacturing process with ID {id} not found");
        }

        _context.ManufacturingProcesses.Remove(process);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted manufacturing process with ID: {Id}", id);
    }

    /// <summary>
    /// Checks if a manufacturing process exists by ID.
    /// </summary>
    /// <param name="id">The ID of the manufacturing process to check.</param>
    /// <returns>True if the manufacturing process exists, false otherwise.</returns>
    public async Task<bool> ProcessExistsAsync(int id)
    {
        return await _context.ManufacturingProcesses.AnyAsync(p => p.Id == id);
    }
}