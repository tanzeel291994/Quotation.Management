using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationCostItemLine
    {
        public string QuotationCostItemGroupId { get; set; } = null!;
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public decimal CostItemLineValue { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual UserMaster? CreatedByNavigation { get; set; }
        public virtual QuotationCostItem QuotationCostItemGroup { get; set; } = null!;
        public virtual QuotationLine QuotationLine { get; set; } = null!;
        public virtual UserMaster? UpdatedByNavigation { get; set; }
    }
}
