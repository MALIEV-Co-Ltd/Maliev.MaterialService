namespace Maliev.MaterialService.Api.DTOs.Materials;

/// <summary>
/// Request model for material mechanical property
/// </summary>
public class MaterialMechanicalPropertyRequest
{
    /// <summary>
    /// Mechanical property ID
    /// </summary>
    public Guid MechanicalPropertyId { get; set; }

    /// <summary>
    /// Property value
    /// </summary>
    public decimal Value { get; set; }
}
