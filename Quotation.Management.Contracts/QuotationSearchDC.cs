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
        public string? ClientCode { get; set; }
        public string? ConsultantCode { get; set; }
        public List<int> Msp = new();
        public string? ProjectName { get; set; }
        public int? BrandId { get; set; }
        public string? Product { get; set; }
        //public bool ItemCodeWise { get; set; }
        public List<string> AreaCode = new();
        public List<int> StatusId = new();
        public List<int> QuotationYear = new();
        public DateTime? FromBookingDate { get; set; }
        public DateTime? ToBookingDate { get; set; }
    }
}
