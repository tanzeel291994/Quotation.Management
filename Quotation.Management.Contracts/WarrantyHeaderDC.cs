using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class WarrantyHeaderDC
    {
        public int JobDetailsId { get; set; } // Primary key
        public string JobReference { get; set; }
        public string ClientCode { get; set; }
        public string ConsultantCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomersOrderReference { get; set; }
        public int PaymentTermsId { get; set; }
        public string PaymentStatus { get; set; }
        public string AreaCode { get; set; }
        public int SalesRepresentativeId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string WarrantyProvisionCurrency { get; set; } // Assuming AED/OMR/USD/EURO/SAR are the possible values
        public decimal WarrantyProvisionPartsTotal { get; set; }
        public decimal WarrantyProvisionPartsUtilized { get; set; }
        public decimal WarrantyProvisionPartsReversed { get; set; }
        public decimal WarrantyProvisionPartsBalance { get; set; }
        public decimal WarrantyProvisionLabourTotal { get; set; }
        public decimal WarrantyProvisionLabourUtilized { get; set; }
        public decimal WarrantyProvisionLabourReversed { get; set; }
        public decimal WarrantyProvisionLabourBalance { get; set; }
    }
}
