using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class PricingMaster
    {
        public string ItemCode { get; set; } = null!;
        public string OptCode { get; set; } = null!;
        public decimal? Price { get; set; }
        public string Version { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual UserMaster? CreatedByNavigation { get; set; }
        public virtual OptionMaster OptCodeNavigation { get; set; } = null!;
    }
}
