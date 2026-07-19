namespace Maliev.MaterialService.Application.DTOs.Catalog;

/// <summary>Manufacturing process response for catalog API.</summary>
public class ProcessCatalogResponse
{
    /// <summary>Unique identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Short code (e.g. "CNC", "FDM").</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
    /// <summary>Display ordering.</summary>
    public int SortOrder { get; set; }
}

/// <summary>Material response for catalog API.</summary>
public class MaterialCatalogResponse
{
    /// <summary>Unique identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Short code.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Material category (Metal, Polymer, Resin, Sheet).</summary>
    public string Category { get; set; } = string.Empty;
    /// <summary>Density in g/cm³.</summary>
    public decimal? DensityGCm3 { get; set; }
    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
    /// <summary>Display ordering.</summary>
    public int SortOrder { get; set; }
}

/// <summary>Surface finish response for catalog API.</summary>
public class SurfaceFinishCatalogResponse
{
    /// <summary>Unique identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Short code.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>Surface roughness Ra in μm.</summary>
    public decimal? RaValueUm { get; set; }
    /// <summary>Additional cost surcharge %.</summary>
    public decimal AdditionalCostPercent { get; set; }
    /// <summary>Optional description.</summary>
    public string? Description { get; set; }
    /// <summary>Display ordering.</summary>
    public int SortOrder { get; set; }
}

/// <summary>Tolerance class response for catalog API.</summary>
public class ToleranceClassCatalogResponse
{
    /// <summary>Unique identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Short code.</summary>
    public string Code { get; set; } = string.Empty;
    /// <summary>ISO standard reference.</summary>
    public string IsoStandard { get; set; } = string.Empty;
    /// <summary>Grade designation.</summary>
    public string Grade { get; set; } = string.Empty;
    /// <summary>Human-readable tolerance range.</summary>
    public string? ToleranceRange { get; set; }
    /// <summary>Additional cost surcharge %.</summary>
    public decimal AdditionalCostPercent { get; set; }
    /// <summary>Display ordering.</summary>
    public int SortOrder { get; set; }
}

/// <summary>Process configuration option response for catalog API.</summary>
public class ProcessConfigOptionCatalogResponse
{
    /// <summary>Unique identifier.</summary>
    public Guid Id { get; set; }
    /// <summary>Machine-readable config key.</summary>
    public string ConfigKey { get; set; } = string.Empty;
    /// <summary>Human-readable label.</summary>
    public string Label { get; set; } = string.Empty;
    /// <summary>Control type: dropdown, toggle, number, text.</summary>
    public string ConfigType { get; set; } = string.Empty;
    /// <summary>Default value as string.</summary>
    public string? DefaultValue { get; set; }
    /// <summary>JSON-encoded allowed values (for dropdown).</summary>
    public string? OptionsJson { get; set; }
    /// <summary>Unit suffix.</summary>
    public string? Unit { get; set; }
    /// <summary>Tooltip text.</summary>
    public string? HelpText { get; set; }
    /// <summary>Whether a value is required.</summary>
    public bool IsRequired { get; set; }
    /// <summary>Display ordering.</summary>
    public int SortOrder { get; set; }
}
