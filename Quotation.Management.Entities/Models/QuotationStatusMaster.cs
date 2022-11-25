using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationStatusMaster
    {
        public QuotationStatusMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
