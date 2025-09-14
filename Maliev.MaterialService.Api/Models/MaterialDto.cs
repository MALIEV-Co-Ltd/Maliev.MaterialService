using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Models;

public class MaterialDto
{
    public int Id { get; set; }
    public int MaterialGroupId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? MaterialNumber { get; set; }
    public string? ManufacturerReference { get; set; }
    public decimal? DensityKilogramPerCubicMeter { get; set; }
    public decimal? TensileStrengthUltimateGigaPascal { get; set; }
    public decimal? TensileStrengthYieldMegaPascal { get; set; }
    public decimal? MachinabilityPercent { get; set; }
    public decimal? ShearModulusGigaPascal { get; set; }
    public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }
    public decimal? PricePerKilogram { get; set; }
    public string? CurrencyCode { get; set; }
    public string? Url { get; set; }
    public string? Comment { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    // Navigation data
    public MaterialGroupDto? MaterialGroup { get; set; }
    public ICollection<MaterialStandardDto>? MaterialStandards { get; set; }
    public ICollection<MaterialPropertyDto>? MaterialProperties { get; set; }
    public ICollection<MaterialProcessCompatibilityDto>? ProcessCompatibilities { get; set; }
    public ICollection<MaterialColorDto>? MaterialColors { get; set; }
    public ICollection<MaterialSurfaceFinishDto>? MaterialSurfaceFinishes { get; set; }
}

public class MaterialGroupDto
{
    public int Id { get; set; }
    public int MaterialFamilyId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public MaterialFamilyDto? MaterialFamily { get; set; }
}

public class MaterialFamilyDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class MaterialStandardDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int StandardTypeId { get; set; }
    public required string StandardValue { get; set; }
    public MaterialStandardTypeDto? StandardType { get; set; }
}

public class MaterialStandardTypeDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class MaterialPropertyDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int PropertySubtypeId { get; set; }
    public required string Value { get; set; }
    public PropertySubtypeDto? PropertySubtype { get; set; }
}

public class PropertySubtypeDto
{
    public int Id { get; set; }
    public int PropertyTypeId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public PropertyTypeDto? PropertyType { get; set; }
}

public class PropertyTypeDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class MaterialProcessCompatibilityDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int ProcessId { get; set; }
    public int CompatibilityScore { get; set; }
    public string? Notes { get; set; }
    public ManufacturingProcessDto? Process { get; set; }
}

public class ManufacturingProcessDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public ManufacturingProcessCategoryDto? Category { get; set; }
}

public class ManufacturingProcessCategoryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class MaterialColorDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int ColorId { get; set; }
    public ColorDto? Color { get; set; }
}

public class ColorDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? HexCode { get; set; }
    public string? Description { get; set; }
}

public class MaterialSurfaceFinishDto
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public int SurfaceFinishId { get; set; }
    public SurfaceFinishDto? SurfaceFinish { get; set; }
}

public class SurfaceFinishDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal? RoughnessRa { get; set; }
}

public class CreateMaterialRequest
{
    [Required]
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

    public decimal? DensityKilogramPerCubicMeter { get; set; }
    public decimal? TensileStrengthUltimateGigaPascal { get; set; }
    public decimal? TensileStrengthYieldMegaPascal { get; set; }
    public decimal? MachinabilityPercent { get; set; }
    public decimal? ShearModulusGigaPascal { get; set; }
    public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }
    public decimal? PricePerKilogram { get; set; }
    public string? CurrencyCode { get; set; }

    [MaxLength(500)]
    public string? Url { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}

public class UpdateMaterialRequest
{
    [Required]
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

    public decimal? DensityKilogramPerCubicMeter { get; set; }
    public decimal? TensileStrengthUltimateGigaPascal { get; set; }
    public decimal? TensileStrengthYieldMegaPascal { get; set; }
    public decimal? MachinabilityPercent { get; set; }
    public decimal? ShearModulusGigaPascal { get; set; }
    public decimal? ThermalConductivityWattPerMeterKelvin { get; set; }
    public decimal? PricePerKilogram { get; set; }
    public string? CurrencyCode { get; set; }

    [MaxLength(500)]
    public string? Url { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public bool IsActive { get; set; } = true;
}