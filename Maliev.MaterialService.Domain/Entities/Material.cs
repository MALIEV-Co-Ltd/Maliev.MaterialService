namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a material in the MALIEV manufacturing catalog.
/// </summary>
public class Material : BaseEntity
{
    /// <summary>
    /// Human-readable name of the material.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code identifier for the material.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the material.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Price per unit in THB.
    /// </summary>
    public decimal PricePerUnit { get; set; }

    /// <summary>
    /// Current stock level.
    /// </summary>
    public int StockLevel { get; set; }

    /// <summary>
    /// Material category (e.g. "Metal", "Polymer", "Resin", "Sheet").
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Material density in g/cm³ (used for weight/pricing calculations).
    /// </summary>
    public decimal? DensityGCm3 { get; set; }

    /// <summary>
    /// CNC machinability rating 1–5 (higher = easier to machine).
    /// </summary>
    public decimal? MachinabilityRating { get; set; }

    /// <summary>
    /// Display ordering for UI lists.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Optional supplier ID reference.
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Navigation property to the associated supplier.
    /// </summary>
    public Supplier? Supplier { get; set; }

    /// <summary>
    /// Manufacturing processes applicable to this material.
    /// </summary>
    public ICollection<ManufacturingProcess> ManufacturingProcesses { get; set; } = new List<ManufacturingProcess>();

    /// <summary>
    /// Colors available for this material.
    /// </summary>
    public ICollection<Color> AvailableColors { get; set; } = new List<Color>();

    /// <summary>
    /// Post-processing methods applicable to this material.
    /// </summary>
    public ICollection<PostProcessingMethod> PostProcessingMethods { get; set; } = new List<PostProcessingMethod>();

    /// <summary>
    /// Mechanical properties associated with this material.
    /// </summary>
    public ICollection<MaterialMechanicalProperty> MechanicalProperties { get; set; } = new List<MaterialMechanicalProperty>();

    /// <summary>
    /// Surface finishes compatible with this material.
    /// </summary>
    public ICollection<SurfaceFinish> SurfaceFinishes { get; set; } = new List<SurfaceFinish>();
}
