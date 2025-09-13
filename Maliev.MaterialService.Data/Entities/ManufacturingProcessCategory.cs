using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class ManufacturingProcessCategory
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(100)]
    public required string DisplayName { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<ManufacturingProcess> ManufacturingProcesses { get; set; } = new List<ManufacturingProcess>();
}