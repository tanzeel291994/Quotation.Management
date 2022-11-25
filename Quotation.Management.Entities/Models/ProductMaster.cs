using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class ProductMaster
    {
        public ProductMaster()
        {
            ItemGroupMasters = new HashSet<ItemGroupMaster>();
        }

        public string ProdTypeId { get; set; } = null!;
        public string? ProdName { get; set; }

        public virtual ICollection<ItemGroupMaster> ItemGroupMasters { get; set; }
    }
}
