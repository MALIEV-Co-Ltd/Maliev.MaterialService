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
    /// Short code identifier (e.g. "CNC", "FDM", "SLA_DLP").
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the process.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Display ordering for UI lists.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Materials that support this manufacturing process.
    /// </summary>
    public ICollection<Material> Materials { get; set; } = new List<Material>();

    /// <summary>
    /// Surface finishes available for this process.
    /// </summary>
    public ICollection<SurfaceFinish> SurfaceFinishes { get; set; } = new List<SurfaceFinish>();

    /// <summary>
    /// Tolerance classes available for this process.
    /// </summary>
    public ICollection<ToleranceClass> ToleranceClasses { get; set; } = new List<ToleranceClass>();

    /// <summary>
    /// Dynamic configuration options for this process.
    /// </summary>
    public ICollection<ProcessConfigOption> ConfigOptions { get; set; } = new List<ProcessConfigOption>();
}
