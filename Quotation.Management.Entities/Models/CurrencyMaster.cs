using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CurrencyMaster
    {
        public CurrencyMaster()
        {
            BrandMasters = new HashSet<BrandMaster>();
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public string CurrencyCode { get; set; } = null!;
        public decimal ConvFactor { get; set; }

        public virtual ICollection<BrandMaster> BrandMasters { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
