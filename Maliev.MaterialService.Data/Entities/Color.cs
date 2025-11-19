using System;
using System.Collections.Generic;

namespace Maliev.MaterialService.Data.Entities;

public class Color : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? HexCode { get; set; }

    // Navigation properties
    public ICollection<Material> Materials { get; set; } = new List<Material>();
}
