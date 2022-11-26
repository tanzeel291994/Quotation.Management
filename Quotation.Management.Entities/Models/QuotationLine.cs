using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationLine
    {
        public QuotationLine()
        {
            QuotationOptCodes = new HashSet<QuotationOptCode>();
        }

        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string? ItemCode { get; set; }
        public string? SubItemCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Mtlp { get; set; }
        public decimal Qty { get; set; }
        public bool ActiveLine { get; set; }
        public decimal? CostItemLineValue { get; set; }

        public virtual ItemMaster? ItemCodeNavigation { get; set; }
        public virtual QuotationHeader QuotationHeader { get; set; } = null!;
        public virtual ICollection<QuotationOptCode> QuotationOptCodes { get; set; }
    }
}
