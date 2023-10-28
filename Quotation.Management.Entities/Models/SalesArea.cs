using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class SalesArea
    {
        public SalesArea()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
            WarrantyHeaders = new HashSet<WarrantyHeader>();
        }

        public string AreaCode { get; set; } = null!;
        public string AreaName { get; set; } = null!;
        public string? Frequency { get; set; }

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
        public virtual ICollection<WarrantyHeader> WarrantyHeaders { get; set; }
    }
}
