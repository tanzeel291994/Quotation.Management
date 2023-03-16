using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationCostItem
    {
        public QuotationCostItem()
        {
            QuotationCostItemLines = new HashSet<QuotationCostItemLine>();
        }

        public string QuotationCostItemGroupId { get; set; } = null!;
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public string ProdTypeId { get; set; } = null!;
        public string CostItemId { get; set; } = null!;
        public string CostItemType { get; set; } = null!;
        public decimal CostItemValue { get; set; }
        public decimal? FreightRate { get; set; }
        public int? NoOfContainers { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string? Remarks { get; set; }
        public virtual CostItemCode CostItem { get; set; } = null!;
        public virtual UserMaster? CreatedByNavigation { get; set; }
        public virtual ProductMaster ProdType { get; set; } = null!;
        public virtual QuotationHeader QuotationHeader { get; set; } = null!;
        public virtual UserMaster? UpdatedByNavigation { get; set; }
        public virtual ICollection<QuotationCostItemLine> QuotationCostItemLines { get; set; }
    }
}
