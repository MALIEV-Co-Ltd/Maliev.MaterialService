using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For [MaxLength] if needed, but Fluent API is preferred

namespace Maliev.MaterialService.Data.Models
{
    public partial class Color
    {
        public Color()
        {
            MaterialHasColor = new HashSet<MaterialHasColor>();
        }

        public int Id { get; set; }

        [Required] // From IsRequired() in MaterialContext
        [MaxLength(50)] // From HasMaxLength(50) in MaterialContext
        public required string Name { get; set; } // Use 'required' for non-nullable properties

        public DateTime CreatedDate { get; set; } // Not nullable due to HasDefaultValueSql
        public DateTime ModifiedDate { get; set; } // Not nullable due to HasDefaultValueSql

        public virtual ICollection<MaterialHasColor> MaterialHasColor { get; set; }
    }
}