namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a surface finish option available for manufactured parts.
/// </summary>
public class SurfaceFinish : BaseEntity
{
    /// <summary>
    /// Display name (e.g. "Anodized Black").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short code identifier (e.g. "ANODIZE_BLACK").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Surface roughness Ra value in μm (null for finishes where Ra is not applicable).
    /// </summary>
    public decimal? RaValueUm { get; set; }

    /// <summary>
    /// Additional cost surcharge as a percentage (e.g. 15.0 = 15% surcharge).
    /// </summary>
    public decimal AdditionalCostPercent { get; set; }

    /// <summary>
    /// Display ordering for UI lists.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Manufacturing processes for which this finish is available.
    /// </summary>
    public ICollection<ManufacturingProcess> AvailableForProcesses { get; set; } = new List<ManufacturingProcess>();

    /// <summary>
    /// Materials compatible with this finish.
    /// </summary>
    public ICollection<Material> CompatibleMaterials { get; set; } = new List<Material>();
}
