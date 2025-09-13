using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class PropertySubtype
{
    public int Id { get; set; }

    public int PropertyTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(20)]
    public string? Unit { get; set; }

    [MaxLength(50)]
    public string? TestStandard { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual PropertyType PropertyType { get; set; } = null!;
    public virtual ICollection<MaterialProperty> MaterialProperties { get; set; } = new List<MaterialProperty>();
}