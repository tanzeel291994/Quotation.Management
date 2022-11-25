using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class ItemMaster
    {
        public ItemMaster()
        {
            QuotationLines = new HashSet<QuotationLine>();
        }

        public string ItemCode { get; set; } = null!;
        public string? ItemCodeDescription { get; set; }
        public int? SeriesId { get; set; }

        public virtual SeriesMaster? Series { get; set; }
        public virtual ICollection<QuotationLine> QuotationLines { get; set; }
    }
}
