namespace Maliev.MaterialService.Data.Entities;

public class MechanicalProperty : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<MaterialMechanicalProperty> MaterialMechanicalProperties { get; set; } = new List<MaterialMechanicalProperty>();
}
