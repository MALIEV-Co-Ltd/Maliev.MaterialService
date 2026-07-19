namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a type of mechanical property (e.g. tensile strength, hardness).
/// </summary>
public class MechanicalProperty : BaseEntity
{
    /// <summary>
    /// Name of the mechanical property.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement (e.g. MPa, HRC).
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Material-level property values for this property type.
    /// </summary>
    public ICollection<MaterialMechanicalProperty> MaterialMechanicalProperties { get; set; } = new List<MaterialMechanicalProperty>();
}
