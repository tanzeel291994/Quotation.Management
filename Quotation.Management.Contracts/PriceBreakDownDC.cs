using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class PriceBreakDownDC
    {
        //quotation header
        //cost items
        public QuotationHeaderDC quotationHeader;
        public List<ProductPrice> productPrices = new();
        public DataTable costItemBreakDownDCs = new();
        public DataTable totalValueDCs = new();
        public DataTable netOptions = new();

    }

    public class ProductPrice
    {
        public DataTable optionsPricing = new DataTable();
        public string productType { get; set; }
        public DataTable totals = new();
        public DataTable costItemProductWise = new();
    }

    public class CostItemBreakDownDC
    {
        public string CostItemCode { get; set; }
        public string CostItemName { get; set;}
        public decimal TotalCostProv { get; set; }
        public decimal Percentage { get; set; }
    }
}
