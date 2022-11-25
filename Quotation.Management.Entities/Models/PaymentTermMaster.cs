using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class PaymentTermMaster
    {
        public PaymentTermMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public int Id { get; set; }
        public string PaymentTermName { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
