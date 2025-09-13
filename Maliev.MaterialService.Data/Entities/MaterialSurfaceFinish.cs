using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public class MaterialSurfaceFinish
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int SurfaceFinishId { get; set; }

    public bool IsRecommended { get; set; } = false;

    [MaxLength(200)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    // Navigation properties
    public virtual Material Material { get; set; } = null!;
    public virtual SurfaceFinish SurfaceFinish { get; set; } = null!;
}