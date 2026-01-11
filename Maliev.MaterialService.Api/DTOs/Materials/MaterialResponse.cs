using System;
using System.Collections.Generic;

namespace Maliev.MaterialService.Api.DTOs.Materials;

/// <summary>
/// Response model containing material information
/// </summary>
public class MaterialResponse
{
    /// <summary>
    /// Unique identifier for the material
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the material
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code identifier for the material
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description of the material
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Price per unit
    /// </summary>
    public decimal PricePerUnit { get; set; }

    /// <summary>
    /// Current stock level
    /// </summary>
    public int StockLevel { get; set; }

    /// <summary>
    /// Supplier ID if associated
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Supplier name if associated
    /// </summary>
    public string? SupplierName { get; set; }

    /// <summary>
    /// List of manufacturing processes for this material
    /// </summary>
    public List<ManufacturingProcessResponse> ManufacturingProcesses { get; set; } = new();

    /// <summary>
    /// List of available colors for this material
    /// </summary>
    public List<ColorResponse> AvailableColors { get; set; } = new();

    /// <summary>
    /// List of post-processing methods for this material
    /// </summary>
    public List<PostProcessingMethodResponse> PostProcessingMethods { get; set; } = new();

    /// <summary>
    /// Mechanical properties of this material
    /// </summary>
    public List<MaterialMechanicalPropertyResponse> MechanicalProperties { get; set; } = new();

    /// <summary>
    /// User who created this material
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when material was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// User who last updated this material
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Timestamp when material was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Version number for optimistic concurrency
    /// </summary>
    public byte[] Version { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Whether this material is active
    /// </summary>
    public bool Active { get; set; }
}

/// <summary>
/// Manufacturing process information
/// </summary>
public class ManufacturingProcessResponse
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Process name
    /// </summary>
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Color information
/// </summary>
public class ColorResponse
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Color name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Hex color code
    /// </summary>
    public string? HexCode { get; set; }
}

/// <summary>
/// Post-processing method information
/// </summary>
public class PostProcessingMethodResponse
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Method name
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
