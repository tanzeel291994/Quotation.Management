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
        public string ItemCode { get; set; } = null!;
        public string? BaseItemCode { get; set; }
        public string? SubItemCode { get; set; }
        public decimal CAF { get; set; }
        public decimal? CostItemLineValue { get; set; }
        public decimal? IndexValue { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Mtlp { get; set; }
        public decimal Qty { get; set; }
        public decimal Vat { get; set; }
        public int? UserId { get; set; }
        public decimal? Margin { get; set; }
        public bool ActiveLine { get; set; }
        public decimal TtNetPrice { get; set; }
        public decimal TtslsPrice { get; set; }
        public decimal TtslsPriceWOVat { get; set; }
        public decimal TtslsPriceWMargin { get; set; }
        public string? ProdTypeId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? UnitTag { get; set; }
        public string? optCodes { get; set; } = null!;
    }

    public class QuotationCopyOptionDC
    {
        public List<string> FromOptCodes { get; set; } = null!;
        public List<int> CopyToLines { get; set; } = null!;
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
    }

    public class QuotationOptCodeDC
    {
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string OptCode { get; set; } = null!;
        public string OptName { get; set; } = null!;
        public string ItemCode { get; set; } = null!;
        public bool IsNet { get; set; }
        public decimal? Price { get; set; }
        public string OptType { get; set; } = null!;
    }

    public class QuotationNonStandardOptCodeDC
    {
        public string QuotationNum { get; set; } = null!;
        public int RevNum { get; set; }
        public int LineNum { get; set; }
        public string OptCode { get; set; } = null!;
        public string? OptName { get; set; }
        public string? ItemCode { get; set; }
        public decimal? Price { get; set; }
    }

    public class PricingMasterDC
    {
        //public decimal? QuotationMultiplier { get; set; }
        public string ItemCode { get; set; } = null!;
        public string OptCode { get; set; } = null!;
       // public string CurrencyCode { get; set; } = null!;
        public decimal Price { get; set; }
       // public decimal ConvFactor { get; set; }
        //public decimal? ConvFactorByBrand { get; set; }
        public string Version { get; set; } = null!;
        public string? Status { get; set; }
        public bool IsNet { get; set; }
        public bool IsItemCodeCreation { get; set; }
    }

    public class ItemCodeDetailsDC
    {
        public string ItemCode { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string ProdName { get; set; } = null!;
        public string ProdTypeId { get; set; } = null!;
        public string CurrencyCode { get; set; } = null!;
        public decimal CAF { get; set; }
        public decimal? IndexConvFactor { get; set; }
        public decimal? Mtlp { get; set; }
    }
}
