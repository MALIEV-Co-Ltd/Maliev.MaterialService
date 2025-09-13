using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialGroup
{
    public int Id { get; set; }

    public int MaterialFamilyId { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string? TypicalApplications { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual MaterialFamily MaterialFamily { get; set; } = null!;
    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}