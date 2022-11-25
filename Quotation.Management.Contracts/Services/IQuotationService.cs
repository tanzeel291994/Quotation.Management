using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;

namespace Quotation.Management.Contracts.Services
{
    public interface IQuotationService
    {
        public QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader);
        public QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine);
        public JObject? GetQuotation(string Id,int revNum);
        public QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine);
        public JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC);

        public QuotationLineDC? InsertQuotationOptions(QuotationLineDC inputLine);

        public QuotationLineDC? RemoveQuotationOptions(QuotationLineDC inputLine);

        public List<QuotationLineDC>? GetQuotationLinesOptCodes(string Id, int revNum);
    }
}
