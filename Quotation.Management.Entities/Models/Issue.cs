using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class Issue
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int? CreatedBy { get; set; }
        public string? AdditionalRemarks { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedOn { get; set; }
        public string? DevRemarks { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual UserMaster? CreatedByNavigation { get; set; }
    }
}
