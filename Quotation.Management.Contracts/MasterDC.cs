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
    }

    public class CurrencyDC
    {
        public string CurrencyCode { get; set; } = null!;
        public string OldCurrencyCode { get; set; } = null!;
        public decimal ConvFactor { get; set; }
        public decimal? NewConvFactor { get; set; }
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
    }
}
