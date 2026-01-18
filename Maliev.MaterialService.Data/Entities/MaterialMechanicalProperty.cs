namespace Maliev.MaterialService.Data.Entities;

public class MaterialMechanicalProperty
{
    public Guid MaterialId { get; set; }
    public Guid MechanicalPropertyId { get; set; }
    public decimal Value { get; set; }

    // Navigation properties
    public Material Material { get; set; } = null!;
    public MechanicalProperty MechanicalProperty { get; set; } = null!;
}
