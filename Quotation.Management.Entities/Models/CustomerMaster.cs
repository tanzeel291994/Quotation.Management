using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CustomerMaster
    {
        public CustomerMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public string CustomerCode { get; set; } = null!;
        public string CustomerName { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
