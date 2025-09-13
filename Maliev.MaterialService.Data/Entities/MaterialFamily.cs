using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialFamily
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<MaterialGroup> MaterialGroups { get; set; } = new List<MaterialGroup>();
}