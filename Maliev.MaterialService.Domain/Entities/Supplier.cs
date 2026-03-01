namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a supplier in the material service domain.
/// </summary>
public class Supplier : BaseEntity
{
    /// <summary>
    /// Supplier name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Contact information for the supplier.
    /// </summary>
    public string? ContactInfo { get; set; }

    /// <summary>
    /// Materials sourced from this supplier.
    /// </summary>
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
