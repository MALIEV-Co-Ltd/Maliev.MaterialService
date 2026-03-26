namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Defines a dynamic configuration field available for a specific manufacturing process.
/// </summary>
public class ProcessConfigOption : BaseEntity
{
    /// <summary>
    /// The manufacturing process this option belongs to.
    /// </summary>
    public Guid ManufacturingProcessId { get; set; }

    /// <summary>
    /// Navigation property to the manufacturing process.
    /// </summary>
    public ManufacturingProcess ManufacturingProcess { get; set; } = null!;

    /// <summary>
    /// Machine-readable key (e.g. "infill_percent", "layer_height_mm").
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable label (e.g. "Infill Percentage", "Layer Height").
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// UI control type: "dropdown", "toggle", "number", or "text".
    /// </summary>
    public string ConfigType { get; set; } = string.Empty;

    /// <summary>
    /// Default value as a string (e.g. "20", "false").
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// JSON-encoded list of allowed values for dropdown controls (null for toggle/number/text).
    /// </summary>
    public string? OptionsJson { get; set; }

    /// <summary>
    /// Display unit suffix (e.g. "%", "mm"). Null if not applicable.
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// Optional tooltip / help text shown in the UI.
    /// </summary>
    public string? HelpText { get; set; }

    /// <summary>
    /// Whether the user must provide a value before submitting.
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Display ordering within the process configuration panel.
    /// </summary>
    public int SortOrder { get; set; }
}
