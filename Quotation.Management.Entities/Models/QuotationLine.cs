using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationLine
    {
        public QuotationLine()
        {
            QuotationCostItemLines = new HashSet<QuotationCostItemLine>();
            QuotationOptCodes = new HashSet<QuotationOptCode>();
        }

        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; } = null!;
        public string? SubItemCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Mtlp { get; set; }
        public decimal Qty { get; set; }
        public bool ActiveLine { get; set; }
        public decimal? CostItemLineValue { get; set; }
        public decimal Vat { get; set; }
        public decimal? CAF { get; set; }
        public decimal TtNetPrice { get; set; }
        public decimal? Margin { get; set; }
        public string? UnitTag { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public decimal? SeaFreightValue { get; set; }

        public virtual UserMaster? CreatedByNavigation { get; set; }
        public virtual ItemMaster ItemCodeNavigation { get; set; } = null!;
        public virtual QuotationHeader QuotationHeader { get; set; } = null!;
        public virtual UserMaster? UpdatedByNavigation { get; set; }
        public virtual ICollection<QuotationCostItemLine> QuotationCostItemLines { get; set; }
        public virtual ICollection<QuotationOptCode> QuotationOptCodes { get; set; }
    }
}
