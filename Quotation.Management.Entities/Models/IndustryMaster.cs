using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class IndustryMaster
    {
        public IndustryMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
    }
}
