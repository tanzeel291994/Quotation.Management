using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;
using System.Data;

namespace Quotation.Management.Contracts.Services
{
    public interface IQuotationService
    {
         QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader);
         QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine);
         JObject? GetQuotation(string Id, int? revNum = null);
         QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine);
         JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC);
         bool CopyOptionLine(QuotationCopyOptionDC input);
         QuotationLineDC? InsertQuotationOptions(QuotationLineDC inputLine);

         QuotationLineDC? RemoveQuotationOptions(QuotationLineDC inputLine);

        List<QuotationOptCodeDC>? GetQuotationLinesOptCodes(string Id, int revNum);

        List<QuotationOptCodeDC>? GetQuotationLinesNonStandardOptCodes(string Id, int revNum);
        QuotationCostItemDC InsertQuotationCostItem(QuotationCostItemDC input);

         bool UpdateQuotationCurrency(CurrencyDC currencyDC);
        QuotationCostItemDC UpdateQuotationCostItem(QuotationCostItemDC input);

        PriceBreakDownDC GetQuotationPBD(string quotationNum, int revNum);
        QuotationCostItemDC DeleteQuotationCostItem(QuotationCostItemDC input);
        List<QuotationCostItemDC> GetQuotationCostLines(string quotationNum, int revNum);

        List<QuotationLineDC> GetQuotationLines(string Id, int revNum);
        QuotationNonStandardOptCodeDC? RemoveNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        QuotationNonStandardOptCodeDC? InsertNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);

        dynamic SearchQuotations(QuotationSearchDC quotationSearch);
        void DeleteQuotationLine(QuotationLineDC input);
        //List<QuotationLine> UpdateUnitPriceFromOptions(string quotatioNum, int revNum, List<int> lineNums, QMTContext context);
    }
}
