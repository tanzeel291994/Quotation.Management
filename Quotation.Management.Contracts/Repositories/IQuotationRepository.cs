using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IQuotationRepository : ITransactional
    {
        public QuotationHeader InsertQuotation(QuotationHeader _quotationHeader);
        public QuotationLine InsertQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);
        public QuotationOptCode InsertQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);
        PricingMaster? GetPricingOptCode(string itemCode, string optCode);
        public QuotationLine? GetLatestQuotationLine(string quotationNum);
        QuotationHeader? GetQuotation(string quotationNum, int revNum);

        public QuotationLine? UpdateQuotationLine(QuotationLine _quotationLine);
        public List<QuotationLineDC> GetQuotationLines(string quotationNum, int revNum);

        public List<QuotationOptCode> GetQuotationOptCodes(string quotationNum, int lineNum, int revNum);

        public dynamic GetItemOptions(string itemCode);

        decimal? GetSumOfOptPrice(string itemCode, List<string> optCode);

        QuotationLine? GetQuotationLine(string quotationNum, int lineNum, int revNum);

        List<PricingMaster> GetPricingOptCode(string itemCode, List<string> optCode);
        public List<QuotationLineDC> GetQuotationLinesOptions(List<QuotationLineDC> quotationLines);
        public QuotationLine UpdateQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);

        public QuotationOptCode? RemoveQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);
    }
}
