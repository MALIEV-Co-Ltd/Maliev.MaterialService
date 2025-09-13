using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class Color
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }

    [MaxLength(7)]
    public string? HexCode { get; set; } // #FFFFFF format

    [MaxLength(200)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int SortOrder { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual ICollection<MaterialColor> MaterialColors { get; set; } = new List<MaterialColor>();
}