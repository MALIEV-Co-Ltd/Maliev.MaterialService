namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Join entity recording the value of a mechanical property for a specific material.
/// </summary>
public class MaterialMechanicalProperty
{
    /// <summary>
    /// Foreign key to the material.
    /// </summary>
    public Guid MaterialId { get; set; }

    /// <summary>
    /// Foreign key to the mechanical property definition.
    /// </summary>
    public Guid MechanicalPropertyId { get; set; }

    /// <summary>
    /// Measured value of this property for the material.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Navigation property to the parent material.
    /// </summary>
    public Material Material { get; set; } = null!;

    /// <summary>
    /// Navigation property to the property definition.
    /// </summary>
    public MechanicalProperty MechanicalProperty { get; set; } = null!;
}
