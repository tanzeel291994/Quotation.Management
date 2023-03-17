using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationOptCode
    {
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string OptCode { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public bool? IsNet { get; set; }
        public string? OptName { get; set; }
        public string? OptType { get; set; }
        public decimal? Baseprice { get; set; }
        public string? Version { get; set; }

        public virtual QuotationLine QuotationLine { get; set; } = null!;
    }
}
