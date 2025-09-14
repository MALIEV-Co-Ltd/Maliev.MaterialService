using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Maliev.MaterialService.Data.Constants;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialProcessCompatibility
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int ProcessId { get; set; }

    public int CompatibilityLevel { get; set; } // 1-5 scale (1=Poor, 3=Good, 5=Excellent)

    [MaxLength(500)]
    public string? ProcessingNotes { get; set; }

    public int? TypicalLeadDays { get; set; }

    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleThickness)]
    public decimal? MinimumThickness { get; set; }

    [Column(TypeName = DatabaseConstants.DecimalPrecisionScaleThickness)]
    public decimal? MaximumThickness { get; set; }

    [MaxLength(20)]
    public string? ToleranceClass { get; set; }

    public bool IsRecommended { get; set; } = false;

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual Material Material { get; set; } = null!;
    public virtual ManufacturingProcess Process { get; set; } = null!;
}