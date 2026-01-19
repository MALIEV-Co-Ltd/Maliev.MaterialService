namespace Maliev.MaterialService.Data.Entities;

public class ManufacturingProcess : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
