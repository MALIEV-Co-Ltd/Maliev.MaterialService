using System;
using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    [ConcurrencyCheck]
    public int Version { get; set; }

    public bool Active { get; set; } = true;
}
