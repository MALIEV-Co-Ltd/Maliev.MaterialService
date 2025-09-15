using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Configurations;

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    [Required]
    public GlobalPolicyOptions GlobalPolicy { get; set; } = new();

    [Required]
    public MaterialsPolicyOptions MaterialsPolicy { get; set; } = new();
}

public class GlobalPolicyOptions
{
    [Range(1, 10000)]
    public int PermitLimit { get; set; } = 1000;

    [Required]
    [RegularExpression(@"^\d{2}:\d{2}:\d{2}$")]
    public string Window { get; set; } = "00:01:00";

    [Range(0, 1000)]
    public int QueueLimit { get; set; } = 100;

    [Range(1, 100)]
    public int SegmentsPerWindow { get; set; } = 2;
}

public class MaterialsPolicyOptions
{
    [Range(1, 1000)]
    public int PermitLimit { get; set; } = 100;

    [Required]
    [RegularExpression(@"^\d{2}:\d{2}:\d{2}$")]
    public string Window { get; set; } = "00:01:00";

    [Range(0, 500)]
    public int QueueLimit { get; set; } = 50;

    [Range(1, 100)]
    public int SegmentsPerWindow { get; set; } = 2;
}