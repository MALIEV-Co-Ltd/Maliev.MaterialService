using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Maliev.MaterialService.Data.Constants;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialProperty
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int PropertySubtypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Value { get; set; } // Could be "150-200" for ranges

    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? MinValue { get; set; }

    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleGeneral)]
    public decimal? MaxValue { get; set; }

    [MaxLength(200)]
    public string? TestConditions { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual Material Material { get; set; } = null!;
    public virtual PropertySubtype PropertySubtype { get; set; } = null!;
}