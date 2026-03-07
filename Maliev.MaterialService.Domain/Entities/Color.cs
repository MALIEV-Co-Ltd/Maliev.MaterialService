namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents an available color option for materials.
/// </summary>
public class Color : BaseEntity
{
    /// <summary>
    /// Color name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Hex color code (e.g. #FF0000).
    /// </summary>
    public string? HexCode { get; set; }

    /// <summary>
    /// Materials that offer this color.
    /// </summary>
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
