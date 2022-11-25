using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class OptionMaster
    {
        public OptionMaster()
        {
            PricingMasters = new HashSet<PricingMaster>();
            QuotationOptCodes = new HashSet<QuotationOptCode>();
        }

        public string OptCode { get; set; } = null!;
        public string? OptName { get; set; }

        public virtual ICollection<PricingMaster> PricingMasters { get; set; }
        public virtual ICollection<QuotationOptCode> QuotationOptCodes { get; set; }
    }
}
