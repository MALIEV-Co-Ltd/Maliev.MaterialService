using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class SurfaceFinish
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Category { get; set; } // "Mechanical", "Chemical", "Coating", etc.

    [MaxLength(100)]
    public string? Process { get; set; } // "Bead Blasting", "Anodizing", etc.

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<MaterialSurfaceFinish> MaterialSurfaceFinishes { get; set; } = new List<MaterialSurfaceFinish>();
}