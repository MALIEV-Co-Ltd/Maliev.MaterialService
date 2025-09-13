using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class ManufacturingProcess
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(50)]
    public string? Technology { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ManufacturingProcessCategory Category { get; set; } = null!;
    public virtual ICollection<MaterialProcessCompatibility> MaterialCompatibilities { get; set; } = new List<MaterialProcessCompatibility>();
}