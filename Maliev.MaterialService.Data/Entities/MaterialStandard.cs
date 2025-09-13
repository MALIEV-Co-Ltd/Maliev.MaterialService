using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialStandard
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int StandardTypeId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string StandardValue { get; set; }

    [MaxLength(50)]
    public string? Grade { get; set; }

    [MaxLength(50)]
    public string? Condition { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual Material Material { get; set; } = null!;
    public virtual MaterialStandardType StandardType { get; set; } = null!;
}