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
        void UpdateMultipleLines(List<QuotationLine> quotationLines, decimal? inputValue, string updateType, QMTContext _context);
        List<QuotationLine> GetQuotationLines(string quotationNum, List<int> lineNums, int revNum, QMTContext? _context = null);
        List<string> GetAllQuotationNums();
         QuotationHeader InsertUpdateQuotation(QuotationHeader _quotationHeader, int? updatedBy, QMTContext? _context = null);
         QuotationLine InsertQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);
         QuotationOptCode InsertQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);
        //PricingMaster? GetPricingOptCode(string itemCode, string optCode);
        QuotationLine? GetLatestQuotationLine(string quotationNum, QMTContext? _context = null);
        dynamic? GetQuotation(string quotationNum, int? revNum=null);
        List<QuotationLineDC> GetQuotationLinesDC(string quotationNum, int revNum, List<int>? selectedLines = null, string prodTypeId = "", int? brandId = null, QMTContext? _context = null);
        int GetNewRevNum(string quotationNum, QMTContext? _context = null);
        string GenerateItemCode(QuotationLineDC _quotationLine, QMTContext? _context = null);
        QuotationOptCode? GetQuotationOptCode(string quotationNum, int revNum, int lineNum, string optCode, QMTContext? _context = null);
        List<QuotationOptCode> GetQuotationOptCodes(string quotationNum, int revNum, QMTContext? _context, int? lineNum = null);

        dynamic GetItemOptions(string itemCode, decimal convFactor);

        List<QuotationCostItem> GetQuotationCostItems(string quotationNum, int revNum, QMTContext? _context = null);

        dynamic GetAllRevisions(string quotationNum);
        QuotationLine GetQuotationLine(string quotationNum, int lineNum, int revNum, QMTContext? _context = null);
        int GetQuotationLatestNum(string areaCode, int userId, int year);
        List<PricingMasterDC> GetPricingOptCode(string itemCode, List<string> optCode);
        List<QuotationOptCodeDC> GetQuotationLinesOptions(string quotationNum, int revNum);
        QuotationLineDC UpdateQuotationLine(QuotationLineDC _quotationLine, QMTContext? _context = null);

        void UpdateQuotationOptCodes(QuotationOptCode quotationOptCode, QMTContext? _context = null);
         QuotationOptCode? RemoveQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);

        QuotationCostItem InsertQuotationCostItemLine(QuotationCostItem _quotationCostItem, QMTContext? _context = null);

        List<QuotationLine> GetQuotationLines(string quotationNum, int revNum, QMTContext? _context);

        List<QuotationCostItemDC> GetQuotationCostLines(string quotatioNum, int revNum);

        QuotationHeader? GetQuotation(string quotationNum, int revNum, QMTContext? _context = null);

        int CreateRevision(string quotationNum, int revNum, int oldRevNum);

        List<QuotationOptCodeDC> GetQuotationLinesNonStandardOptions(string quotationNum, int revNum, int lineNum);
        QuotationHeader UpdateQuotationHeader(QuotationHeader _quotationHeader, QMTContext? _context = null);

        QuotationCostItem UpdateCostItem(QuotationCostItem _quotationCostItem, QMTContext? _context = null);
        List<QuotationCostItemLine> GetQuotationCostItemLines(string quotationNum, int revNum, QMTContext? _context = null);
        List<QuotationCostItemLine> InUpdDelQuotationCostItemLines(string quotationNum, int revNum, string groupId, List<QuotationCostItemLine> _quotationCostItemLines, QMTContext? _context = null);
        QuotationHeaderDC? GetQuotationHeader(string quotatioNum, int revNum);
        List<string> GetQuotationOptions(string quotationNum, int revNum);
        QuotationCostItem DeleteCostItem(QuotationCostItem _quotationCostItem, QMTContext? _context = null);
        QuotationCostItem GetQuotationCostItem(string quotationNum, int revNum, string groupId, QMTContext? _context = null);
        Dictionary<string, decimal> UpdateCostValueOfAllQuotationLine(List<QuotationLine> quotationLines, List<QuotationCostItemLine> costItemLines, List<QuotationCostItem> costItems, Dictionary<string, decimal> groupIdTotalDict, string seaFreightCostCode, string customDutyCode, QMTContext context);
        void UpdateCustomDutyCostItemValue(string quotationNum, int revNum, List<QuotationCostItem> customDutyItems, Dictionary<string, decimal> groupIdTotalDict, QMTContext context);
        //dynamic GetQuotationSearch(QuotationSearchDC input);

        dynamic GetQuotationLinesSearch(QuotationSearchDC input);

        void DeleteQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null);
        void DeleteQuotationOptions(string quotationNum, int linenum, int revNum, QMTContext? _context = null);

        void DeleteCostItemLines(string quotationNum, int linenum, int revNum, QMTContext? _context = null);

        List<QuotationCostItemLine> GetQuotationCostItemLines(string quotationNum, int lineNum, int revNum, QMTContext? _context = null);

        //dynamic GetBrandData(QuotationSearchDC input, out dynamic brandValue);
        void SetActiveRevision(string quotationNum, int revNum);
        void DeleteCostItemGroup(string quotationNum, int linenum, int revNum, List<string> costItemGroupIds, QMTContext? _context = null);

        void UpdateQuotationStatus(string quotationNum, int revNum, int userId);

        dynamic GetQuotationSearch(QuotationSearchDC input);

        dynamic GetQuotationLinesForActiveRevison(string quotationNum);

        QuotationOptCode UpdateQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null);
    }
}
