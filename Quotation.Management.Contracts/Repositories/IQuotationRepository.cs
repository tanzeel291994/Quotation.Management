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
         QuotationHeader InsertQuotation(QuotationHeader _quotationHeader);
         QuotationLine InsertQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);
         QuotationOptCode InsertQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);
        PricingMaster? GetPricingOptCode(string itemCode, string optCode);
         QuotationLine? GetLatestQuotationLine(string quotationNum);
         QuotationHeader? GetQuotation(string quotationNum, int? revNum=null);
        List<QuotationLineDC> GetQuotationLines(string quotationNum, int revNum, List<int>? selectedLines = null, string prodTypeId = "");

         List<QuotationOptCode> GetQuotationOptCodes(string quotationNum, int lineNum, int revNum);

         dynamic GetItemOptions(string itemCode);

        List<QuotationCostItem> GetQuotationCostItems(string quotationNum, int revNum, QMTContext? _context = null);
        decimal? GetSumOfOptPrice(string itemCode, List<string> optCode);

        QuotationLine? GetQuotationLine(string quotationNum, int lineNum, int revNum);

        List<PricingMaster> GetPricingOptCode(string itemCode, List<string> optCode);
         List<QuotationLineDC> GetQuotationLinesOptions(List<QuotationLineDC> quotationLines);
         QuotationLine UpdateQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);

         void UpdateQuotationOptCodes(string quotationNum, int lineNum, int revNum, QMTContext? _context = null);
         QuotationOptCode? RemoveQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);

         QuotationCostItem InsertQuotationCostItemLine(QuotationCostItem _quotationCostItem, QMTContext? _context = null);

        List<QuotationLine> GetQuotationLines(string quotationNum, int revNum, QMTContext _context);

        List<QuotationCostItemDC> GetQuotationCostLines(string quotatioNum, int revNum);

        QuotationHeader? GetQuotation(string quotationNum, int revNum, QMTContext _context);

        QuotationHeader UpdateQuotationHeader(QuotationHeader _quotationHeader, QMTContext? _context = null);

        QuotationCostItem UpdateCostItemLine(QuotationCostItem _quotationCostItem, QMTContext? _context = null);

        QuotationCostItem? GetQuotationCostItem(string quotationNum, int revNum, string prodTypeId, string costItemId, QMTContext? _context = null);

        QuotationCostItem DeleteCostItemLine(QuotationCostItem _quotationCostItem, QMTContext? _context = null);
        List<QuotationLine> UpdateCostValueOfAllQuotationLine(List<QuotationLine> quotationLines, List<QuotationCostItem> costItems, Dictionary<string, decimal> prodTotalDict, QMTContext? _context);
    }
}
