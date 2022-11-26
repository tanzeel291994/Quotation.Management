using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public partial class QuotationCostItemDC
    {
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public string ProdTypeId { get; set; } = null!;
        public string CostItemId { get; set; } = null!;
        public string CostItemType { get; set; } = null!;
        public decimal? CostItemValue { get; set; }
        public decimal? FreightRate { get; set; }
        public int? NoOfContainers { get; set; }
    }

    public enum CostItemType
    {
        ByVal,
        ByPercentage
    }

    public class ProdItemTotal
    {
        public string ProdTypeId { get; set; }
        public string ItemCode { get; set; }
        public decimal TotalValue { get; set; }
    }
}
