using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class WarrantyHeader
    {
        public WarrantyHeader()
        {
            WarrantyLines = new HashSet<WarrantyLine>();
        }

        public int JobDetailsId { get; set; }
        public string? JobReference { get; set; }
        public string? ClientCode { get; set; }
        public string? ConsultantCode { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomersOrderReference { get; set; }
        public int? PaymentTermsId { get; set; }
        public string? PaymentStatus { get; set; }
        public string? AreaCode { get; set; }
        public int? SalesRepresentativeId { get; set; }
        public string? WarrantyProvisionCurrency { get; set; }
        public decimal? WarrantyProvisionPartsTotal { get; set; }
        public decimal? WarrantyProvisionPartsUtilized { get; set; }
        public decimal? WarrantyProvisionPartsReversed { get; set; }
        public decimal? WarrantyProvisionPartsBalance { get; set; }
        public decimal? WarrantyProvisionLabourTotal { get; set; }
        public decimal? WarrantyProvisionLabourUtilized { get; set; }
        public decimal? WarrantyProvisionLabourReversed { get; set; }
        public decimal? WarrantyProvisionLabourBalance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public virtual SalesArea? AreaCodeNavigation { get; set; }
        public virtual CustomerMaster? ClientCodeNavigation { get; set; }
        public virtual CustomerMaster? ConsultantCodeNavigation { get; set; }
        public virtual UserMaster CreatedByNavigation { get; set; } = null!;
        public virtual CustomerMaster? CustomerCodeNavigation { get; set; }
        public virtual PaymentTermMaster? PaymentTerms { get; set; }
        public virtual UserMaster? SalesRepresentative { get; set; }
        public virtual UserMaster? UpdatedByNavigation { get; set; }
        public virtual CurrencyMaster? WarrantyProvisionCurrencyNavigation { get; set; }
        public virtual ICollection<WarrantyLine> WarrantyLines { get; set; }
    }
}
