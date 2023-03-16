using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class MasterDC
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public int Id { get; set; }
        public string Type="";
    }

    public class CurrencyDC
    {
        public string CurrencyCode { get; set; } = null!;
        public ProductCAFCode[] ProductCAFs = new ProductCAFCode[] { };
        public string? OldCurrencyCode { get; set; }
        public decimal ConvFactor { get; set; }
        // public decimal? NewConvFactor { get; set; }
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
    }

    public class ProductCAFCode
    {
        public string ProductCode { get; set; } = null!;
        public string BrandCode = null!;
        public string CurrencyCode = null!;
        public decimal CAF;
    }

    public enum MasterEnum
    {
        DELIVERY_TERM=1,
        PAYMENT_TERM=2,
        SALES_AREA=3,
        CUSTOMER=4,
        CLIENT=5,
        CONSULTANT=6,
        INDUSTRY =7,
        COSTITEMS=8,
        STATUS =9
    }

}
