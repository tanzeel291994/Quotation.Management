using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CurrencyMaster
    {
        public CurrencyMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public string CurrencyCode { get; set; } = null!;
        public decimal ConvFactor { get; set; }

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
