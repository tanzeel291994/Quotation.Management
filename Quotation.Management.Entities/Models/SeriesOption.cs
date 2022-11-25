using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class SeriesOption
    {
        public string? OptCode { get; set; }
        public int? SeriesId { get; set; }

        public virtual OptionMaster? OptCodeNavigation { get; set; }
        public virtual SeriesMaster? Series { get; set; }
    }
}
