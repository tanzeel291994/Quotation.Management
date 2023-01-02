using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class IndustryMaster
    {
        public IndustryMaster()
        {
            QuotationLines = new HashSet<QuotationLine>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<QuotationLine> QuotationLines { get; set; }
    }
}
