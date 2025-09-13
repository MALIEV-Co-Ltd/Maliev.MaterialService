using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.DbContexts;
using Maliev.MaterialService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Maliev.MaterialService.Api.Services;

public class MaterialSearchService : IMaterialSearchService
{
    private readonly MaterialDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<MaterialSearchService> _logger;
    private readonly CacheOptions _cacheOptions;

    public MaterialSearchService(
        MaterialDbContext context,
        IMemoryCache cache,
        ILogger<MaterialSearchService> logger,
        CacheOptions cacheOptions)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions;
    }

    public async Task<IEnumerable<Material>> SearchAsync(string query, MaterialSearchFilters? filters = null)
    {
        var cacheKey = GenerateSearchCacheKey(query, filters);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var materialQuery = _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .Include(m => m.ProcessCompatibilities)
                .ThenInclude(mpc => mpc.Process)
            .AsQueryable();

        // Apply text search
        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim().ToLowerInvariant();
            materialQuery = materialQuery.Where(m =>
                m.Name.ToLower().Contains(normalizedQuery) ||
                (m.Description != null && m.Description.ToLower().Contains(normalizedQuery)) ||
                (m.MaterialNumber != null && m.MaterialNumber.ToLower().Contains(normalizedQuery)) ||
                (m.ManufacturerReference != null && m.ManufacturerReference.ToLower().Contains(normalizedQuery)) ||
                m.MaterialGroup.Name.ToLower().Contains(normalizedQuery) ||
                m.MaterialGroup.MaterialFamily.Name.ToLower().Contains(normalizedQuery));
        }

        // Apply filters
        if (filters != null)
        {
            materialQuery = ApplyFilters(materialQuery, filters);
        }

        var results = await materialQuery
            .OrderBy(m => m.MaterialGroup.SortOrder)
            .ThenBy(m => m.Name)
            .ToListAsync();

        // Cache for shorter duration for search results
        _cache.Set(cacheKey, results, TimeSpan.FromMinutes(15));
        _logger.LogDebug("Search '{Query}' returned {Count} results", query, results.Count);

        return results;
    }

    public async Task<IEnumerable<Material>> GetByPropertyRangeAsync(int propertySubtypeId, decimal? minValue, decimal? maxValue)
    {
        var cacheKey = $"materials_property_{propertySubtypeId}_{minValue}_{maxValue}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Include(m => m.MaterialProperties)
            .Where(m => m.IsActive &&
                       m.MaterialProperties.Any(mp => mp.PropertySubtypeId == propertySubtypeId));

        if (minValue.HasValue)
        {
            query = query.Where(m => m.MaterialProperties
                .Any(mp => mp.PropertySubtypeId == propertySubtypeId &&
                          mp.MinValue >= minValue.Value));
        }

        if (maxValue.HasValue)
        {
            query = query.Where(m => m.MaterialProperties
                .Any(mp => mp.PropertySubtypeId == propertySubtypeId &&
                          mp.MaxValue <= maxValue.Value));
        }

        var results = await query
            .OrderBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, results, _cacheOptions.DefaultExpiration);
        return results;
    }

    public async Task<IEnumerable<Material>> GetByStandardAsync(string standardType, string standardValue)
    {
        var cacheKey = $"materials_standard_{standardType}_{standardValue}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var results = await _context.Materials
            .Include(m => m.MaterialGroup)
            .Include(m => m.MaterialStandards)
                .ThenInclude(ms => ms.StandardType)
            .Where(m => m.IsActive &&
                       m.MaterialStandards.Any(ms =>
                           ms.StandardType.Code.ToLower() == standardType.ToLower() &&
                           ms.StandardValue.ToLower() == standardValue.ToLower()))
            .OrderBy(m => m.Name)
            .ToListAsync();

        _cache.Set(cacheKey, results, _cacheOptions.DefaultExpiration);
        return results;
    }

    public async Task<IEnumerable<Material>> GetByDensityRangeAsync(decimal? minDensity, decimal? maxDensity)
    {
        var cacheKey = $"materials_density_{minDensity}_{maxDensity}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Where(m => m.IsActive && m.DensityKilogramPerCubicMeter.HasValue);

        if (minDensity.HasValue)
        {
            query = query.Where(m => m.DensityKilogramPerCubicMeter >= minDensity.Value);
        }

        if (maxDensity.HasValue)
        {
            query = query.Where(m => m.DensityKilogramPerCubicMeter <= maxDensity.Value);
        }

        var results = await query
            .OrderBy(m => m.DensityKilogramPerCubicMeter)
            .ToListAsync();

        _cache.Set(cacheKey, results, _cacheOptions.DefaultExpiration);
        return results;
    }

    public async Task<IEnumerable<Material>> GetByTensileStrengthRangeAsync(decimal? minStrength, decimal? maxStrength)
    {
        var cacheKey = $"materials_tensile_{minStrength}_{maxStrength}";

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
            .Where(m => m.IsActive);

        if (minStrength.HasValue)
        {
            query = query.Where(m =>
                (m.TensileStrengthUltimateGigaPascal.HasValue && m.TensileStrengthUltimateGigaPascal * 1000 >= minStrength.Value) ||
                (m.TensileStrengthYieldMegaPascal.HasValue && m.TensileStrengthYieldMegaPascal >= minStrength.Value));
        }

        if (maxStrength.HasValue)
        {
            query = query.Where(m =>
                (m.TensileStrengthUltimateGigaPascal.HasValue && m.TensileStrengthUltimateGigaPascal * 1000 <= maxStrength.Value) ||
                (m.TensileStrengthYieldMegaPascal.HasValue && m.TensileStrengthYieldMegaPascal <= maxStrength.Value));
        }

        var results = await query
            .OrderByDescending(m => m.TensileStrengthUltimateGigaPascal)
            .ThenByDescending(m => m.TensileStrengthYieldMegaPascal)
            .ToListAsync();

        _cache.Set(cacheKey, results, _cacheOptions.DefaultExpiration);
        return results;
    }

    public async Task<IEnumerable<Material>> GetRecommendationsAsync(MaterialRecommendationRequest request)
    {
        var cacheKey = GenerateRecommendationCacheKey(request);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<Material>? cachedResults))
        {
            return cachedResults!;
        }

        var query = _context.Materials
            .Include(m => m.MaterialGroup)
                .ThenInclude(mg => mg.MaterialFamily)
            .Include(m => m.ProcessCompatibilities)
                .ThenInclude(mpc => mpc.Process)
            .Where(m => m.IsActive);

        // Filter by process compatibility
        if (request.ProcessIds.Any())
        {
            query = query.Where(m => m.ProcessCompatibilities
                .Any(mpc => request.ProcessIds.Contains(mpc.ProcessId) &&
                           mpc.CompatibilityLevel >= 3)); // At least moderate compatibility
        }

        // Apply constraints
        if (request.MaxDensity.HasValue)
        {
            query = query.Where(m => !m.DensityKilogramPerCubicMeter.HasValue ||
                                    m.DensityKilogramPerCubicMeter <= request.MaxDensity.Value);
        }

        if (request.MinTensileStrength.HasValue)
        {
            query = query.Where(m =>
                (m.TensileStrengthUltimateGigaPascal.HasValue && m.TensileStrengthUltimateGigaPascal * 1000 >= request.MinTensileStrength.Value) ||
                (m.TensileStrengthYieldMegaPascal.HasValue && m.TensileStrengthYieldMegaPascal >= request.MinTensileStrength.Value));
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(m => !m.PricePerKilogram.HasValue ||
                                    m.PricePerKilogram <= request.MaxPrice.Value);
        }

        // Apply application-based filtering if specified
        if (!string.IsNullOrWhiteSpace(request.Application))
        {
            var normalizedApp = request.Application.ToLowerInvariant();
            query = query.Where(m =>
                (m.Description != null && m.Description.ToLower().Contains(normalizedApp)) ||
                (m.MaterialGroup.TypicalApplications != null && m.MaterialGroup.TypicalApplications.ToLower().Contains(normalizedApp)));
        }

        var results = await query
            .Take(request.MaxResults * 2) // Get more for scoring
            .ToListAsync();

        // Score and rank materials
        var scoredResults = results.Select(m => new
        {
            Material = m,
            Score = CalculateRecommendationScore(m, request)
        })
        .OrderByDescending(x => x.Score)
        .Take(request.MaxResults)
        .Select(x => x.Material)
        .ToList();

        _cache.Set(cacheKey, scoredResults, TimeSpan.FromMinutes(30));
        _logger.LogInformation("Generated {Count} material recommendations", scoredResults.Count);

        return scoredResults;
    }

    private static IQueryable<Material> ApplyFilters(IQueryable<Material> query, MaterialSearchFilters filters)
    {
        if (!filters.IncludeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        if (filters.MaterialFamilyId.HasValue)
        {
            query = query.Where(m => m.MaterialGroup.MaterialFamilyId == filters.MaterialFamilyId.Value);
        }

        if (filters.MaterialGroupId.HasValue)
        {
            query = query.Where(m => m.MaterialGroupId == filters.MaterialGroupId.Value);
        }

        if (filters.ProcessIds?.Any() == true)
        {
            query = query.Where(m => m.ProcessCompatibilities
                .Any(mpc => filters.ProcessIds.Contains(mpc.ProcessId)));
        }

        if (filters.MinDensity.HasValue)
        {
            query = query.Where(m => m.DensityKilogramPerCubicMeter >= filters.MinDensity.Value);
        }

        if (filters.MaxDensity.HasValue)
        {
            query = query.Where(m => m.DensityKilogramPerCubicMeter <= filters.MaxDensity.Value);
        }

        if (filters.MinTensileStrength.HasValue)
        {
            query = query.Where(m =>
                (m.TensileStrengthUltimateGigaPascal.HasValue && m.TensileStrengthUltimateGigaPascal * 1000 >= filters.MinTensileStrength.Value) ||
                (m.TensileStrengthYieldMegaPascal.HasValue && m.TensileStrengthYieldMegaPascal >= filters.MinTensileStrength.Value));
        }

        if (filters.MaxTensileStrength.HasValue)
        {
            query = query.Where(m =>
                (m.TensileStrengthUltimateGigaPascal.HasValue && m.TensileStrengthUltimateGigaPascal * 1000 <= filters.MaxTensileStrength.Value) ||
                (m.TensileStrengthYieldMegaPascal.HasValue && m.TensileStrengthYieldMegaPascal <= filters.MaxTensileStrength.Value));
        }

        if (filters.MinPrice.HasValue)
        {
            query = query.Where(m => m.PricePerKilogram >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(m => m.PricePerKilogram <= filters.MaxPrice.Value);
        }

        return query;
    }

    private static double CalculateRecommendationScore(Material material, MaterialRecommendationRequest request)
    {
        double score = 0.0;

        // Base score for process compatibility
        if (request.ProcessIds.Any())
        {
            var maxCompatibility = material.ProcessCompatibilities
                .Where(mpc => request.ProcessIds.Contains(mpc.ProcessId))
                .Select(mpc => mpc.CompatibilityLevel)
                .DefaultIfEmpty(0)
                .Max();

            score += maxCompatibility * 20; // Max 100 points for perfect compatibility
        }

        // Density preference (lighter is often better for many applications)
        if (request.MaxDensity.HasValue && material.DensityKilogramPerCubicMeter.HasValue)
        {
            var densityRatio = (double)material.DensityKilogramPerCubicMeter.Value / (double)request.MaxDensity.Value;
            if (densityRatio <= 1.0)
            {
                score += (1.0 - densityRatio) * 20; // Up to 20 points for being lighter
            }
        }

        // Strength bonus
        if (request.MinTensileStrength.HasValue)
        {
            var materialStrength = (double)(material.TensileStrengthUltimateGigaPascal * 1000 ??
                                           material.TensileStrengthYieldMegaPascal ?? 0);
            if (materialStrength >= (double)request.MinTensileStrength.Value)
            {
                var strengthRatio = materialStrength / (double)request.MinTensileStrength.Value;
                score += Math.Min(strengthRatio - 1.0, 0.5) * 20; // Up to 10 points for exceeding requirements
            }
        }

        // Price efficiency (lower price is better)
        if (request.MaxPrice.HasValue && material.PricePerKilogram.HasValue)
        {
            var priceRatio = (double)material.PricePerKilogram.Value / (double)request.MaxPrice.Value;
            if (priceRatio <= 1.0)
            {
                score += (1.0 - priceRatio) * 15; // Up to 15 points for being cheaper
            }
        }

        // Availability bonus (materials with complete data are preferred)
        if (material.DensityKilogramPerCubicMeter.HasValue) score += 2;
        if (material.TensileStrengthUltimateGigaPascal.HasValue) score += 2;
        if (material.TensileStrengthYieldMegaPascal.HasValue) score += 2;
        if (material.PricePerKilogram.HasValue) score += 2;

        return score;
    }

    private static string GenerateSearchCacheKey(string query, MaterialSearchFilters? filters)
    {
        var key = $"search_{query ?? ""}";

        if (filters != null)
        {
            key += $"_{filters.MaterialFamilyId}_{filters.MaterialGroupId}_{filters.MinDensity}_{filters.MaxDensity}" +
                   $"_{filters.MinTensileStrength}_{filters.MaxTensileStrength}_{filters.MinPrice}_{filters.MaxPrice}" +
                   $"_{filters.IncludeInactive}_{string.Join(",", filters.ProcessIds ?? new List<int>())}";
        }

        return key.Length > 250 ? $"search_{key.GetHashCode()}" : key;
    }

    private static string GenerateRecommendationCacheKey(MaterialRecommendationRequest request)
    {
        return $"recommend_{string.Join(",", request.ProcessIds)}_{request.MaxDensity}_{request.MinTensileStrength}" +
               $"_{request.MaxPrice}_{request.Application}_{request.MaxResults}";
    }
}