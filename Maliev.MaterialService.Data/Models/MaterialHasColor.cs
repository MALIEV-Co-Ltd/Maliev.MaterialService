using System;
using System.Collections.Generic;

namespace Maliev.MaterialService.Data.Models
{
    public partial class MaterialHasColor
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int ColorId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Color Color { get; set; } = null!;
        public virtual Material Material { get; set; } = null!;
    }
}