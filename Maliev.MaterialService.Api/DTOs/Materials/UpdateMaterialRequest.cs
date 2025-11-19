using System;
using System.Collections.Generic;

namespace Maliev.MaterialService.Api.DTOs.Materials;

/// <summary>
/// Request model for updating an existing material
/// </summary>
public class UpdateMaterialRequest
{
    /// <summary>
    /// Name of the material
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Unique code identifier
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
    /// Supplier ID (validated against Supplier Service)
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Manufacturing process IDs
    /// </summary>
    public List<Guid> ManufacturingProcessIds { get; set; } = new();
    /// <summary>
    /// Color IDs
    /// </summary>
    public List<Guid> ColorIds { get; set; } = new();
    /// <summary>
    /// Post-processing method IDs
    /// </summary>
    public List<Guid> PostProcessingMethodIds { get; set; } = new();
    /// <summary>
    /// Mechanical properties
    /// </summary>
    public List<MaterialMechanicalPropertyRequest> MechanicalProperties { get; set; } = new();

    /// <summary>
    /// Version for optimistic concurrency
    /// </summary>
    public int Version { get; set; }
}
