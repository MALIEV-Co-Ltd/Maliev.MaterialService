using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs.Materials;

/// <summary>
/// Request model for creating a new material
/// </summary>
public class CreateMaterialRequest
{
    /// <summary>
    /// Name of the material
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code identifier for the material
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the material
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Price per unit of the material
    /// </summary>
    [Range(0, (double)decimal.MaxValue)]
    public decimal PricePerUnit { get; set; }

    /// <summary>
    /// Current stock level
    /// </summary>
    [Range(0, int.MaxValue)]
    public int StockLevel { get; set; }

    /// <summary>
    /// Optional supplier ID (validated against Supplier Service)
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// List of manufacturing process IDs associated with this material
    /// </summary>
    public List<Guid> ManufacturingProcessIds { get; set; } = new();

    /// <summary>
    /// List of color IDs available for this material
    /// </summary>
    public List<Guid> ColorIds { get; set; } = new();

    /// <summary>
    /// List of post-processing method IDs applicable to this material
    /// </summary>
    public List<Guid> PostProcessingMethodIds { get; set; } = new();

    /// <summary>
    /// Mechanical properties of the material
    /// </summary>
    public List<MaterialMechanicalPropertyRequest> MechanicalProperties { get; set; } = new();
}