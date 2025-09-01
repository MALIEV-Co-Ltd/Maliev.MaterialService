namespace Maliev.MaterialService.Api.DTOs
{
    public class MaterialDto
    {
        public int Id { get; set; }
        public int MaterialGroupId { get; set; }
        public bool Machinable { get; set; }
        public bool Printable { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Aisi { get; set; }
        public string? Din { get; set; }
        public string? Bts { get; set; }
        public string? Jis { get; set; }
        public string? Uns { get; set; }
        public string? En { get; set; }
        public string? Afnor { get; set; }
        public string? Uni { get; set; }
        public string? Sis { get; set; }
        public string? Sae { get; set; }
        public string? Astm { get; set; }
        public string? Ams { get; set; }
        public string? MaterialNumber { get; set; }
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