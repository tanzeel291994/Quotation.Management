using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class QuotationHeaderDC
    {
        public string? QuotationNum { get; set; }
        public int RevNum { get; set; }
        public string CustomerCode { get; set; } = null!;
        public string? CustomerName { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public int Msp { get; set; }
        public int Asp { get; set; }
        public string? ProjectName { get; set; }
        public string AreaCode { get; set; } = null!;
        public DateTime? ExpectedDeliveryDate { get; set; }
        public DateTime? QuotationDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public int DeliveryTermId { get; set; }
        public int PaymentTermId { get; set; }
        public int? IndustryId { get; set; }
        public int StatusId { get; set; }
        public int? UserId { get; set; }
        public string? StatusName { get; set; } = null!;
        public string? AreaName { get; set; } = null!;
        public string? MspName { get; set; } = null!;
        public int Probability { get; set; }
    }
}
