namespace Maliev.MaterialService.Application.DTOs.Materials;

/// <summary>
/// Response model for material mechanical property.
/// </summary>
public class MaterialMechanicalPropertyResponse
{
    /// <summary>
    /// Mechanical property ID.
    /// </summary>
    public Guid MechanicalPropertyId { get; set; }

    /// <summary>
    /// Mechanical property name.
    /// </summary>
    public string MechanicalPropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// Property value.
    /// </summary>
    public decimal Value { get; set; }
}
