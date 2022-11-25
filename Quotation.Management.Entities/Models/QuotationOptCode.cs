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

        public virtual OptionMaster OptCodeNavigation { get; set; } = null!;
        public virtual QuotationLine QuotationLine { get; set; } = null!;
    }
}
