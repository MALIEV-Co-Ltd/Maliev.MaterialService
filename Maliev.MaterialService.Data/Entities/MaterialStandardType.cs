using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialStandardType
{
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public required string Code { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? Organization { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<MaterialStandard> MaterialStandards { get; set; } = new List<MaterialStandard>();
}