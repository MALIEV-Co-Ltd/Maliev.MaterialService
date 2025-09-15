using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Api.Models;

/// <summary>
/// Request model for creating a new material group.
/// </summary>
public class CreateMaterialGroupRequest
{
    /// <summary>
    /// Gets or sets the identifier of the material family this group belongs to.
    /// </summary>
    [Required]
    public int MaterialFamilyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the material group.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the material group.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets typical applications for materials in this group.
    /// </summary>
    [MaxLength(1000)]
    public string? TypicalApplications { get; set; }

    /// <summary>
    /// Gets or sets the sort order for displaying material groups.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a URL to more information about the material group.
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the material group.
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the material group is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request model for updating an existing material group.
/// </summary>
public class UpdateMaterialGroupRequest
{
    /// <summary>
    /// Gets or sets the identifier of the material family this group belongs to.
    /// </summary>
    [Required]
    public int MaterialFamilyId { get; set; }

    /// <summary>
    /// Gets or sets the name of the material group.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the material group.
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets typical applications for materials in this group.
    /// </summary>
    [MaxLength(1000)]
    public string? TypicalApplications { get; set; }

    /// <summary>
    /// Gets or sets the sort order for displaying material groups.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a URL to more information about the material group.
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the material group.
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the material group is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request model for creating a new manufacturing process.
/// </summary>
public class CreateManufacturingProcessRequest
{
    /// <summary>
    /// Gets or sets the identifier of the process category this process belongs to.
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the manufacturing process.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the technology used in the manufacturing process.
    /// </summary>
    [MaxLength(50)]
    public string? Technology { get; set; }

    /// <summary>
    /// Gets or sets the description of the manufacturing process.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the sort order for displaying manufacturing processes.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a URL to more information about the manufacturing process.
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the manufacturing process.
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the manufacturing process is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Request model for updating an existing manufacturing process.
/// </summary>
public class UpdateManufacturingProcessRequest
{
    /// <summary>
    /// Gets or sets the identifier of the process category this process belongs to.
    /// </summary>
    [Required]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the name of the manufacturing process.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the technology used in the manufacturing process.
    /// </summary>
    [MaxLength(50)]
    public string? Technology { get; set; }

    /// <summary>
    /// Gets or sets the description of the manufacturing process.
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the sort order for displaying manufacturing processes.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets or sets a URL to more information about the manufacturing process.
    /// </summary>
    [MaxLength(500)]
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets additional comments about the manufacturing process.
    /// </summary>
    [MaxLength(1000)]
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the manufacturing process is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}