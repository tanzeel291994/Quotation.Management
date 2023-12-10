using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class AHULineDC
    {
        public String QuotationNum;
        public int RevNum;
        public string ItemCode;
        public string UnitTag;
        public int Qty;
        public int Mtlp;
        public int? Vat;
        public bool? IsNet;
        public decimal? Margin;
        public decimal UnitPrice;
        public string? Optname; //by default BASIC
    }
    public class AHULineDCEqualityComparer : IEqualityComparer<AHULineDC>
    {
        public bool Equals(AHULineDC x, AHULineDC y)
        {
            // Adjust the logic here based on which properties define equality
            return x.ItemCode == y.ItemCode && x.UnitTag == y.UnitTag && x.UnitPrice == y.UnitPrice; ;
        }

        public int GetHashCode(AHULineDC obj)
        {
            // Adjust the hash code generation logic based on properties used in Equals
            return obj.ItemCode.GetHashCode() ^ obj.UnitTag.GetHashCode() ^ obj.UnitPrice.GetHashCode();
        }
    }
}
