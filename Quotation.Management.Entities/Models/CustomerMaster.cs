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

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int CustomerType { get; set; }

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
