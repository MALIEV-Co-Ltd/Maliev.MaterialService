using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Models
{
    public partial class MaterialGroup
    {
        public MaterialGroup()
        {
            Material = new HashSet<Material>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        [MaxLength(50)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<Material> Material { get; set; }
    }
}