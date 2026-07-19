namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a dimensional tolerance class (e.g. ISO 2768-1 grades).
/// </summary>
public class ToleranceClass : BaseEntity
{
    /// <summary>
    /// Display name (e.g. "Fine (ISO 2768-f)").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Short code identifier (e.g. "ISO2768_F").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// ISO standard reference (e.g. "ISO 2768-1").
    /// </summary>
    public string IsoStandard { get; set; } = string.Empty;

    /// <summary>
    /// Grade designation within the standard (e.g. "f", "m", "IT6").
    /// </summary>
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable tolerance range (e.g. "±0.05mm (0.5–6mm)").
    /// </summary>
    public string? ToleranceRange { get; set; }

    /// <summary>
    /// Additional cost surcharge as a percentage (e.g. 25.0 = 25% surcharge).
    /// </summary>
    public decimal AdditionalCostPercent { get; set; }

    /// <summary>
    /// Display ordering for UI lists.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Manufacturing processes for which this tolerance class is available.
    /// </summary>
    public ICollection<ManufacturingProcess> AvailableForProcesses { get; set; } = new List<ManufacturingProcess>();
}
