using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.DTOs
{
    public class UpdateMaterialRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int MaterialGroupId { get; set; }
        public bool Machinable { get; set; }
        public bool Printable { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Aisi { get; set; }

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
    }
}