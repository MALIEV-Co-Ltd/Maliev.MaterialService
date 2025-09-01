using System;
using System.Collections.Generic;

namespace Maliev.MaterialService.Data.Models
{
    public partial class MaterialHasSupplier
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int SupplierId { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual Material Material { get; set; } = null!;
    }
}