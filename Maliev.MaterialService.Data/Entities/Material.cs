using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Maliev.MaterialService.Data.Constants;

namespace Maliev.MaterialService.Data.Entities;

/// <summary>
/// Represents a material in the material service system.
/// </summary>
public class Material
{
    /// <summary>
    /// Gets or sets the unique identifier for the material.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the material group this material belongs to.
    /// </summary>
    [Required]
    public required int MaterialGroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the material.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the material.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the material number or part number.
    /// </summary>
    [MaxLength(100)]
    public string? MaterialNumber { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer's reference for the material.
    /// </summary>
    [MaxLength(100)]
    public string? ManufacturerReference { get; set; }

    // Legacy properties for backward compatibility
    /// <summary>
    /// Gets or sets the density of the material in kg/m³.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? DensityKilogramPerCubicMeter { get; set; }

    /// <summary>
    /// Gets or sets the ultimate tensile strength of the material in GPa.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? TensileStrengthUltimateGigaPascal { get; set; }

    /// <summary>
    /// Gets or sets the yield tensile strength of the material in MPa.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? TensileStrengthYieldMegaPascal { get; set; }

    /// <summary>
    /// Gets or sets the machinability percentage of the material.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? MachinabilityPercent { get; set; }

    /// <summary>
    /// Gets or sets the shear modulus of the material in GPa.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? ShearModulusGigaPascal { get; set; }

    /// <summary>
    /// Gets or sets the thermal conductivity of the material in W/(m·K).
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }

    // Pricing information (reference pricing)
    /// <summary>
    /// Gets or sets the price per kilogram of the material.
    /// </summary>
    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? PricePerKilogram { get; set; }

    /// <summary>
    /// Gets or sets the ISO 4217 currency code for the material price (e.g., "USD", "EUR").
    /// </summary>
    [MaxLength(3)]
    public string? CurrencyCode { get; set; } // ISO 4217 currency code (e.g., "USD", "EUR")

    /// <summary>
    /// Gets or sets a URL to more information about the material.
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the material.
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the material is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the date and time when the material was created.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the material was last modified.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    /// <summary>
    /// Gets or sets the material group this material belongs to.
    /// </summary>
    public virtual MaterialGroup MaterialGroup { get; set; } = null!;

    /// <summary>
    /// Gets or sets the collection of material standards associated with this material.
    /// </summary>
    public virtual ICollection<MaterialStandard> MaterialStandards { get; set; } = new List<MaterialStandard>();

    /// <summary>
    /// Gets or sets the collection of material properties associated with this material.
    /// </summary>
    public virtual ICollection<MaterialProperty> MaterialProperties { get; set; } = new List<MaterialProperty>();

    /// <summary>
    /// Gets or sets the collection of process compatibilities for this material.
    /// </summary>
    public virtual ICollection<MaterialProcessCompatibility> ProcessCompatibilities { get; set; } = new List<MaterialProcessCompatibility>();

    /// <summary>
    /// Gets or sets the collection of colors associated with this material.
    /// </summary>
    public virtual ICollection<MaterialColor> MaterialColors { get; set; } = new List<MaterialColor>();

    /// <summary>
    /// Gets or sets the collection of surface finishes associated with this material.
    /// </summary>
    public virtual ICollection<MaterialSurfaceFinish> MaterialSurfaceFinishes { get; set; } = new List<MaterialSurfaceFinish>();
}