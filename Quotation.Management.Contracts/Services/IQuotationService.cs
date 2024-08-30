using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;
using System.Data;

namespace Quotation.Management.Contracts.Services
{
    public interface IQuotationService
    {
         List<string> AddAHULinesList(DataSet ds, string quotationNum, int revNum, int createdBy);
         byte[] GenerateQuotationWord(string quotationNum);
         bool UpdateMultipleLines(UpdateMultipleLinesDC data);
         List<string> GetAllQuotationNums();
         QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader);
         QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine);
         dynamic? GetQuotation(string Id, int? revNum = null);
         QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine);
         JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC);
         bool CopyOptionLine(QuotationCopyOptionDC input);
         QuotationLineDC? InsertQuotationOptions(QuotationLineDC inputLine);
         dynamic GetAllRevisions(string quotationNum);
         QuotationLineDC? RemoveQuotationOptions(QuotationLineDC inputLine);
        int CreateRevision(string quotationNum, int revNum, int userId);
        List<QuotationOptCodeDC>? GetQuotationLinesOptCodes(string Id, int revNum);
        QuotationNonStandardOptCodeDC? UpdateNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        List<QuotationOptCodeDC>? GetQuotationLinesNonStandardOptCodes(string Id, int revNum, int lineNum);
        List<QuotationCostItemDC> InsertQuotationCostItem(List<QuotationCostItemDC> input);
        bool CopyQuotationLinesFromQuotation(CopyQuotationLineDC input);
         CurrencyDC GetCurrencyCode(string curencyCode, string oldCurrencyCode, string quotationNum, int revNum);
         bool UpdateQuotationCurrency(CurrencyDC currencyDC);
        QuotationCostItemDC UpdateQuotationCostItem(QuotationCostItemDC input);
        //byte[] CreateExcelFilePBD(PriceBreakDownDC data);
        PriceBreakDownDC GetQuotationPBD(string quotationNum, int revNum);
        QuotationCostItemDC DeleteQuotationCostItem(QuotationCostItemDC input);
        List<QuotationCostItemDC> GetQuotationCostLines(string quotationNum, int revNum);

        List<QuotationLineDC> GetQuotationLines(string Id, int revNum);
        QuotationNonStandardOptCodeDC? RemoveNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        QuotationNonStandardOptCodeDC? InsertNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC);
        void SetActiveRevision(string quotationNum, int revNum);
        JArray SearchQuotations(QuotationSearchDC quotationSearch);
        void DeleteQuotationLine(QuotationLineDC input);

        dynamic GetProductsFromQuotation(string Id, int revNum);
        void UpdateQuotationStatus(string quotationNum, int revNum, int userId);
        //JArray GetQuotationDashboard(QuotationSearchDC quotationSearch);
        void ImportData(DataSet ds);
        void ImportQuotationLines(DataSet ds, string quotationNum, int revNum);

        List<ProductCAFCode> GetProductCAF(string quotationNum, int revNum);
        bool UpdateProductCAF(List<ProductCAFCode> currencyDC, string quotationNum, int revNum);
        dynamic GetQuotationLinesForActiveRevison(string quotationNum);
        //List<QuotationLine> UpdateUnitPriceFromOptions(string quotatioNum, int revNum, List<int> lineNums, QMTContext context);
    }
}
