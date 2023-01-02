using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class UserMaster
    {
        public UserMaster()
        {
            QuotationHeaderAspNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderMspNavigations = new HashSet<QuotationHeader>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaderAspNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderMspNavigations { get; set; }
    }
}
