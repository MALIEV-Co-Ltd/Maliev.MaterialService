namespace Maliev.MaterialService.Data.Entities;

public class Material : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PricePerUnit { get; set; }
    public int StockLevel { get; set; }
    public Guid? SupplierId { get; set; }

    // Navigation properties
    public Supplier? Supplier { get; set; }
    public ICollection<ManufacturingProcess> ManufacturingProcesses { get; set; } = new List<ManufacturingProcess>();
    public ICollection<Color> AvailableColors { get; set; } = new List<Color>();
    public ICollection<PostProcessingMethod> PostProcessingMethods { get; set; } = new List<PostProcessingMethod>();
    public ICollection<MaterialMechanicalProperty> MechanicalProperties { get; set; } = new List<MaterialMechanicalProperty>();
}
