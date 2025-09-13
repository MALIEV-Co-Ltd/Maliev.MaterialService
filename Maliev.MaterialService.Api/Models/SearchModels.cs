using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Models;

public class MaterialSearchFilters
{
    public int? MaterialFamilyId { get; set; }
    public int? MaterialGroupId { get; set; }
    public List<int>? ProcessIds { get; set; }
    public decimal? MinDensity { get; set; }
    public decimal? MaxDensity { get; set; }
    public decimal? MinTensileStrength { get; set; }
    public decimal? MaxTensileStrength { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool IncludeInactive { get; set; } = false;
}

public class MaterialRecommendationRequest
{
    [Required]
    public required List<int> ProcessIds { get; set; }

    public decimal? MaxDensity { get; set; }
    public decimal? MinTensileStrength { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Application { get; set; }

    [Range(1, 50)]
    public int MaxResults { get; set; } = 10;
}