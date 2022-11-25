using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class DeliveryTermMaster
    {
        public DeliveryTermMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public int Id { get; set; }
        public string DeliveryTermName { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
