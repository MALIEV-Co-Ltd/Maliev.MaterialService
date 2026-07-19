namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Represents a post-processing method applicable to a material.
/// </summary>
public class PostProcessingMethod : BaseEntity
{
    /// <summary>
    /// Name of the post-processing method.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Materials that support this post-processing method.
    /// </summary>
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
