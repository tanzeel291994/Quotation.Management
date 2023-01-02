using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class UserMaster
    {
        public UserMaster()
        {
            QuotationHeaders = new HashSet<QuotationHeader>();
            QuotationLines = new HashSet<QuotationLine>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public virtual ICollection<QuotationHeader> QuotationHeaders { get; set; }
        public virtual ICollection<QuotationLine> QuotationLines { get; set; }
    }
}
