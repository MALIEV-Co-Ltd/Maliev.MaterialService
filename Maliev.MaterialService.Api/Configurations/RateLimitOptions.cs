namespace Maliev.MaterialService.Api.Configurations;

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public GlobalPolicyOptions GlobalPolicy { get; set; } = new();
    public MaterialsPolicyOptions MaterialsPolicy { get; set; } = new();
}

public class GlobalPolicyOptions
{
    public int PermitLimit { get; set; } = 1000;
    public string Window { get; set; } = "00:01:00";
    public int QueueLimit { get; set; } = 100;
    public int SegmentsPerWindow { get; set; } = 2;
}

public class MaterialsPolicyOptions
{
    public int PermitLimit { get; set; } = 100;
    public string Window { get; set; } = "00:01:00";
    public int QueueLimit { get; set; } = 50;
    public int SegmentsPerWindow { get; set; } = 2;
}