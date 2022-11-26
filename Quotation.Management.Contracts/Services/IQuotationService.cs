using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;

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

         List<QuotationLineDC>? GetQuotationLinesOptCodes(string Id, int revNum);

        QuotationCostItem? InsertQuotationCostItem(QuotationCostItemDC input);

         bool UpdateQuotationCurrency(CurrencyDC currencyDC);
        QuotationCostItem? UpdateQuotationCostItem(QuotationCostItemDC input);

        QuotationCostItem? DeleteQuotationCostItem(QuotationCostItemDC input);
        List<QuotationCostItemDC> GetQuotationCostLines(string quotationNum, int revNum);
    }
}
