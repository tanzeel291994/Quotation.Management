using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class CustomerMaster
    {
        public CustomerMaster()
        {
            QuotationHeaderClientCodeNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderConsultantCodeNavigations = new HashSet<QuotationHeader>();
            QuotationHeaderCustomerCodeNavigations = new HashSet<QuotationHeader>();
            WarrantyHeaderClientCodeNavigations = new HashSet<WarrantyHeader>();
            WarrantyHeaderConsultantCodeNavigations = new HashSet<WarrantyHeader>();
            WarrantyHeaderCustomerCodeNavigations = new HashSet<WarrantyHeader>();
        }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Type { get; set; }

        public virtual ICollection<QuotationHeader> QuotationHeaderClientCodeNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderConsultantCodeNavigations { get; set; }
        public virtual ICollection<QuotationHeader> QuotationHeaderCustomerCodeNavigations { get; set; }
        public virtual ICollection<WarrantyHeader> WarrantyHeaderClientCodeNavigations { get; set; }
        public virtual ICollection<WarrantyHeader> WarrantyHeaderConsultantCodeNavigations { get; set; }
        public virtual ICollection<WarrantyHeader> WarrantyHeaderCustomerCodeNavigations { get; set; }
    }
}
