using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For [MaxLength] if needed, but Fluent API is preferred

namespace Maliev.MaterialService.Data.Models
{
    public partial class Material
    {
        public Material()
        {
            MaterialHasColor = new HashSet<MaterialHasColor>();
            MaterialHasSupplier = new HashSet<MaterialHasSupplier>();
            MaterialHasSurfaceFinish = new HashSet<MaterialHasSurfaceFinish>();
        }

        public int Id { get; set; }
        public int MaterialGroupId { get; set; }
        public bool Machinable { get; set; }
        public bool Printable { get; set; }

        [Required] // From IsRequired() in MaterialContext
        [MaxLength(50)] // From HasMaxLength(50) in MaterialContext
        public required string Name { get; set; }

        [MaxLength(50)]
        public string? Aisi { get; set; } // Nullable based on Fluent API (no IsRequired)

        [MaxLength(50)]
        public string? Din { get; set; }

        [MaxLength(50)]
        public string? Bts { get; set; }

        [MaxLength(50)]
        public string? Jis { get; set; }

        [MaxLength(50)]
        public string? Uns { get; set; }

        [MaxLength(50)]
        public string? En { get; set; }

        [MaxLength(50)]
        public string? Afnor { get; set; }

        [MaxLength(50)]
        public string? Uni { get; set; }

        [MaxLength(50)]
        public string? Sis { get; set; }

        [MaxLength(50)]
        public string? Sae { get; set; }

        [MaxLength(50)]
        public string? Astm { get; set; }

        [MaxLength(50)]
        public string? Ams { get; set; }

        [MaxLength(50)]
        public string? MaterialNumber { get; set; }

        [MaxLength(50)]
        public string? ManufacturerReference { get; set; }

        public decimal? HardnessBrinell { get; set; }
        public decimal? HardnessKnoop { get; set; }
        public decimal? HardnessRockwellA { get; set; }
        public decimal? HardnessRockwellB { get; set; }
        public decimal? HardnessRockwellC { get; set; }
        public decimal? HardnessVickers { get; set; }
        public decimal? DensityKilogramPerCubicMeter { get; set; }
        public decimal? TensileStrengthUltimateGigaPascal { get; set; }
        public decimal? TensileStrengthYieldMegaPascal { get; set; }
        public decimal? MachinabilityPercent { get; set; }
        public decimal? ShearModulusGigaPascal { get; set; }
        public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }
        public string? Url { get; set; }
        public decimal? PricePerKilogram { get; set; }
        public int? CurrencyId { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedDate { get; set; } // Not nullable due to HasDefaultValueSql
        public DateTime ModifiedDate { get; set; } // Not nullable due to HasDefaultValueSql

        public virtual MaterialGroup MaterialGroup { get; set; } = null!; // Navigation property, will be loaded by EF Core
        public virtual ICollection<MaterialHasColor> MaterialHasColor { get; set; }
        public virtual ICollection<MaterialHasSupplier> MaterialHasSupplier { get; set; }
        public virtual ICollection<MaterialHasSurfaceFinish> MaterialHasSurfaceFinish { get; set; }
    }
}