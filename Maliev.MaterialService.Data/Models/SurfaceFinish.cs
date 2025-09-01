using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Maliev.MaterialService.Data.Models
{
    public partial class SurfaceFinish
    {
        public SurfaceFinish()
        {
            MaterialHasSurfaceFinish = new HashSet<MaterialHasSurfaceFinish>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<MaterialHasSurfaceFinish> MaterialHasSurfaceFinish { get; set; }
    }
}