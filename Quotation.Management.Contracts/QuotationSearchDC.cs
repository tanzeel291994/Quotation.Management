using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class QuotationSearchDC
    {
        public string? QuotationNum { get; set; }
        //public int RevNum { get; set; }
        public string? CustomerCode { get; set; }
        public int? Msp { get; set; }
        public string? ProjectName { get; set; }
        public int? BrandId { get; set; }
        public string? Product { get; set; }
        //public bool ItemCodeWise { get; set; }
        public string? AreaCode { get; set; }
        public int? StatusId { get; set; }
        public int? QuotationYear { get; set; }
        public DateTime? FromBookingDate { get; set; }
        public DateTime? ToBookingDate { get; set; }
    }
}
