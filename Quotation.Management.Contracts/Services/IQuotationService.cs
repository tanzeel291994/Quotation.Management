using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;
using System.Data;

namespace Quotation.Management.Contracts.Services
{
    public interface IQuotationService
    {

         bool UpdateMultipleLines(UpdateMultipleLinesDC data);
         List<string> GetAllQuotationNums();
         QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader);
         QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine);
         QuotationHeader? GetQuotation(string Id, int? revNum = null);
         QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine);
         JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC);
         bool CopyOptionLine(QuotationCopyOptionDC input);
         QuotationLineDC? InsertQuotationOptions(QuotationLineDC inputLine);
         dynamic GetAllRevisions(string quotationNum);
         QuotationLineDC? RemoveQuotationOptions(QuotationLineDC inputLine);
        int CreateRevision(string quotationNum, int revNum, int userId);
        List<QuotationOptCodeDC>? GetQuotationLinesOptCodes(string Id, int revNum);

        List<QuotationOptCodeDC>? GetQuotationLinesNonStandardOptCodes(string Id, int revNum, int lineNum);
        List<QuotationCostItemDC> InsertQuotationCostItem(List<QuotationCostItemDC> input);

        CurrencyDC GetCurrencyCode(string curencyCode, string oldCurrencyCode, string quotationNum, int revNum);
         bool UpdateQuotationCurrency(CurrencyDC currencyDC);
        QuotationCostItemDC UpdateQuotationCostItem(QuotationCostItemDC input);

        PriceBreakDownDC GetQuotationPBD(string quotationNum, int revNum);
        QuotationCostItemDC DeleteQuotationCostItem(QuotationCostItemDC input);
        List<QuotationCostItemDC> GetQuotationCostLines(string quotationNum, int revNum);

        List<QuotationLineDC> GetQuotationLines(string Id, int revNum);
        QuotationNonStandardOptCodeDC? RemoveNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        QuotationNonStandardOptCodeDC? InsertNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        void SetActiveRevision(string quotationNum, int revNum);
        dynamic SearchQuotations(QuotationSearchDC quotationSearch);
        void DeleteQuotationLine(QuotationLineDC input);

        dynamic GetProductsFromQuotation(string Id, int revNum);

        void ImportData(DataSet ds);
        void ImportQuotationLines(DataSet ds, string quotationNum, int revNum);
        //List<QuotationLine> UpdateUnitPriceFromOptions(string quotatioNum, int revNum, List<int> lineNums, QMTContext context);
    }
}
