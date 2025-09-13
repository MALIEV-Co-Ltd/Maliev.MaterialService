using Maliev.MaterialService.Api.Models;
using Maliev.MaterialService.Data.Entities;

namespace Maliev.MaterialService.Api.Services;

public interface IMaterialSearchService
{
    Task<IEnumerable<Material>> SearchAsync(string query, MaterialSearchFilters? filters = null);
    Task<IEnumerable<Material>> GetByPropertyRangeAsync(int propertySubtypeId, decimal? minValue, decimal? maxValue);
    Task<IEnumerable<Material>> GetByStandardAsync(string standardType, string standardValue);
    Task<IEnumerable<Material>> GetByDensityRangeAsync(decimal? minDensity, decimal? maxDensity);
    Task<IEnumerable<Material>> GetByTensileStrengthRangeAsync(decimal? minStrength, decimal? maxStrength);
    Task<IEnumerable<Material>> GetRecommendationsAsync(MaterialRecommendationRequest request);
}