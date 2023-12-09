using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class WarrantySearchDC
    {
        public int JobDetailsId { get; set; } // Primary key
        public string SalesOrderReference { get; set; }
        public string ClientCode { get; set; }
        public string ConsultantCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomersOrderReference { get; set; }
        public List<int> PaymentTermsId = new();
        public string PaymentStatus { get; set; }
        public List<string> AreaCode = new();
        public List<int> SalesRepresentativeId = new();
    }

    public class TableData
    {
        public List<string> Headers { get; set; }
        public List<List<string>> Rows { get; set; }
    }
}
