using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class QuotationLineDC
    {
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string ItemCode { get; set; }
        public string? SubItemCode { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Mtlp { get; set; }
        public decimal Qty { get; set; }
        public bool ActiveLine { get; set; }
        public decimal TtslsPrice { get; set; }
        public string ProdTypeId { get; set; }
        public string? optCodes { get; set; } = null!;
    }

    public class QuotationCopyOptionDC
    {
        public List<int> to { get; set; }
        public int from { get; set; }

        public string QuotationNum { get; set; }

        public int RevNum { get; set; }
    }

}
