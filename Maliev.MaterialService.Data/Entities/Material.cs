using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Maliev.MaterialService.Data.Entities;

public class Material
{
    public int Id { get; set; }

    public int MaterialGroupId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? MaterialNumber { get; set; }

    [MaxLength(100)]
    public string? ManufacturerReference { get; set; }

    // Legacy properties for backward compatibility
    [Column(TypeName = "decimal(18,6)")]
    public decimal? DensityKilogramPerCubicMeter { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? TensileStrengthUltimateGigaPascal { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? TensileStrengthYieldMegaPascal { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? MachinabilityPercent { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? ShearModulusGigaPascal { get; set; }

    [Column(TypeName = "decimal(18,6)")]
    public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }

    // Pricing information (reference pricing)
    [Column(TypeName = "decimal(18,6)")]
    public decimal? PricePerKilogram { get; set; }

    public int? CurrencyId { get; set; } // References CurrencyService

    [MaxLength(500)]
    public string? Url { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual MaterialGroup MaterialGroup { get; set; } = null!;
    public virtual ICollection<MaterialStandard> MaterialStandards { get; set; } = new List<MaterialStandard>();
    public virtual ICollection<MaterialProperty> MaterialProperties { get; set; } = new List<MaterialProperty>();
    public virtual ICollection<MaterialProcessCompatibility> ProcessCompatibilities { get; set; } = new List<MaterialProcessCompatibility>();
    public virtual ICollection<MaterialColor> MaterialColors { get; set; } = new List<MaterialColor>();
    public virtual ICollection<MaterialSurfaceFinish> MaterialSurfaceFinishes { get; set; } = new List<MaterialSurfaceFinish>();
}