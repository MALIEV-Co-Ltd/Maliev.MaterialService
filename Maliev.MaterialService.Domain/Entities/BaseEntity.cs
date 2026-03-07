namespace Maliev.MaterialService.Domain.Entities;

/// <summary>
/// Base entity class providing common audit and concurrency fields.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Timestamp when the entity was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// User who created the entity.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the entity was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// User who last updated the entity.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Whether the entity is active (soft-delete flag).
    /// </summary>
    public bool Active { get; set; } = true;
}
