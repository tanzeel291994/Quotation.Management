using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class ProductMaster
    {
        public ProductMaster()
        {
            ItemGroupMasters = new HashSet<ItemGroupMaster>();
            QuotationCostItems = new HashSet<QuotationCostItem>();
        }

        public string ProdTypeId { get; set; } = null!;
        public string? ProdName { get; set; }

        public virtual ICollection<ItemGroupMaster> ItemGroupMasters { get; set; }
        public virtual ICollection<QuotationCostItem> QuotationCostItems { get; set; }
    }
}
