namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a manufacturing process that can be applied to a material.
/// </summary>
public class ManufacturingProcess : BaseEntity
{
    /// <summary>
    /// Name of the manufacturing process.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Materials that support this manufacturing process.
    /// </summary>
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
