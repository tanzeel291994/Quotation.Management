using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class UserMaster
    {
        public UserMaster()
        {
            Issues = new HashSet<Issue>();
            QuotationCostItemCreatedByNavigations = new HashSet<QuotationCostItem>();
            QuotationCostItemUpdatedByNavigations = new HashSet<QuotationCostItem>();
            QuotationHeaderAspNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderCreatedByNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderMspNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderUpdatedByNavigations = new HashSet<QuotationHeader>();
            QuotationLineCreatedByNavigations = new HashSet<QuotationLine>();
            QuotationLineUpdatedByNavigations = new HashSet<QuotationLine>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Role { get; set; }

        public virtual ICollection<Issue> Issues { get; set; }
        public virtual ICollection<QuotationCostItem> QuotationCostItemCreatedByNavigations { get; set; }
        public virtual ICollection<QuotationCostItem> QuotationCostItemUpdatedByNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderAspNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderCreatedByNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderMspNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderUpdatedByNavigations { get; set; }
        public virtual ICollection<QuotationLine> QuotationLineCreatedByNavigations { get; set; }
        public virtual ICollection<QuotationLine> QuotationLineUpdatedByNavigations { get; set; }
    }
}
