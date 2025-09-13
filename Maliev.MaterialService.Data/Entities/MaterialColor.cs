using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialColor
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int ColorId { get; set; }

    public bool IsStandard { get; set; } = true;

    [MaxLength(200)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual Material Material { get; set; } = null!;
    public virtual Color Color { get; set; } = null!;
}