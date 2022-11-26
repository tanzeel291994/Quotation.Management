using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class SalesArea
    {
        public SalesArea()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public string AreaCode { get; set; } = null!;
        public string AreaName { get; set; } = null!;
        public string? Frequency { get; set; }

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
