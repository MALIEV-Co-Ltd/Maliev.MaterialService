using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class PropertyType
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Category { get; set; }

    [Required]
    [MaxLength(20)]
    public required string DataType { get; set; } // "decimal", "string", "range", "boolean"

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<PropertySubtype> PropertySubtypes { get; set; } = new List<PropertySubtype>();
}