namespace Maliev.MaterialService.Data.Entities;

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactInfo { get; set; }

    // Navigation properties
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
