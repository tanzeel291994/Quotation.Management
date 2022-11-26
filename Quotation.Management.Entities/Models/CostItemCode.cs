using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CostItemCode
    {
        public CostItemCode()
        {
            QuotationCostItems = new HashSet<QuotationCostItem>();
        }

        public string CostItemId { get; set; } = null!;
        public string CostItemName { get; set; } = null!;

        public virtual ICollection<QuotationCostItem> QuotationCostItems { get; set; }
    }
}
