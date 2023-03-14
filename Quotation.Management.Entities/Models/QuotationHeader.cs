using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationHeader
    {
        public QuotationHeader()
        {
            QuotationCostItems = new HashSet<QuotationCostItem>();
            QuotationLines = new HashSet<QuotationLine>();
        }

        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public string CustomerCode { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public int Msp { get; set; }
        public string? ProjectName { get; set; }
        public string AreaCode { get; set; } = null!;
        public DateTime QuotationDate { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public int DeliveryTermId { get; set; }
        public int PaymentTermId { get; set; }
        public int StatusId { get; set; }
        public int Probability { get; set; }
        public bool IsActiveRevision { get; set; }
        public decimal? ConvFactor { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? IndustryId { get; set; }
        public int? Asp { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public string? OldCurrencyCode { get; set; }

        public virtual SalesArea AreaCodeNavigation { get; set; } = null!;
        public virtual UserMaster? AspNavigation { get; set; }
        public virtual UserMaster? CreatedByNavigation { get; set; }
        public virtual CurrencyMaster CurrencyCodeNavigation { get; set; } = null!;
        public virtual CustomerMaster CustomerCodeNavigation { get; set; } = null!;
        public virtual DeliveryTermMaster DeliveryTerm { get; set; } = null!;
        public virtual IndustryMaster? Industry { get; set; }
        public virtual UserMaster MspNavigation { get; set; } = null!;
        public virtual CurrencyMaster? OldCurrencyCodeNavigation { get; set; }
        public virtual PaymentTermMaster PaymentTerm { get; set; } = null!;
        public virtual QuotationStatusMaster Status { get; set; } = null!;
        public virtual UserMaster? UpdatedByNavigation { get; set; }
        public virtual ICollection<QuotationCostItem> QuotationCostItems { get; set; }
        public virtual ICollection<QuotationLine> QuotationLines { get; set; }
    }
}
