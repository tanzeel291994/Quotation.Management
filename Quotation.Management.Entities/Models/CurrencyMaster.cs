using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CurrencyMaster
    {
        public CurrencyMaster()
        {
            BrandMasters = new HashSet<BrandMaster>();
            QuotationHeaderCurrencyCodeNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderOldCurrencyCodeNavigations = new HashSet<QuotationHeader>();
        }

        public string CurrencyCode { get; set; } = null!;
        public decimal ConvFactor { get; set; }

        public virtual ICollection<BrandMaster> BrandMasters { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderCurrencyCodeNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderOldCurrencyCodeNavigations { get; set; }
    }
}
