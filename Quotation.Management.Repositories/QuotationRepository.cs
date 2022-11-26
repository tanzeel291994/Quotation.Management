using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class QuotationRepository : BaseRepository<QuotationHeader>, IQuotationRepository
    {
        public QuotationRepository()
        {

        }

        public  QuotationHeader InsertQuotation (QuotationHeader _quotationHeader)
        {

            using (var context = new QMTContext())
            {
                context.QuotationHeaders.Add(_quotationHeader);
                context.SaveChanges();
                return _quotationHeader;
            }
        }

        public QuotationLine? UpdateQuotationLine(QuotationLine _quotationLine)
        {
            using (var context = new QMTContext())
            {
                var line = context.QuotationLines.Where(x => x.QuotationNum == _quotationLine.QuotationNum.ToUpper() && x.RevNum == _quotationLine.RevNum
                 && x.LineNum == _quotationLine.LineNum).FirstOrDefault();
                if(line != null)
                {
                    line.Qty = _quotationLine.Qty;
                    line.Mtlp = _quotationLine.Mtlp;
                    context.SaveChanges();
                }
                return line;
            }
        }

        public QuotationLine UpdateQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var line = context.QuotationLines.Where(x => x.QuotationNum == _quotationLine.QuotationNum.ToUpper() && x.RevNum == _quotationLine.RevNum
                 && x.LineNum == _quotationLine.LineNum).FirstOrDefault();
            if (line != null)
            {
                line.UnitPrice = _quotationLine.UnitPrice;
                line.Mtlp = _quotationLine.Mtlp;
                line.Qty = _quotationLine.Qty;
                context.SaveChanges();
            }
            return _quotationLine;
        }

        public List<QuotationLine> UpdateCostValueOfAllQuotationLine(List<QuotationLine> quotationLines, List<QuotationCostItem> costItems,Dictionary<string,decimal> prodTotalDict, QMTContext? _context)
        {

            bool firstIteration = true;
            foreach (var costItem in costItems)
            {
                if (prodTotalDict.ContainsKey(costItem.ProdTypeId))
                    continue;
                decimal totalValueProdwise = prodTotalDict[costItem.ProdTypeId];
                foreach (var _line in quotationLines)
                {
                    decimal ttslsPrice = _line.Qty * _line.Mtlp * _line.UnitPrice;
                    if (costItem.CostItemType == CostItemType.ByVal.ToString())
                    {
                        if (firstIteration)
                            _line.CostItemLineValue = costItem.CostItemValue * (ttslsPrice / totalValueProdwise);
                        else
                            _line.CostItemLineValue += costItem.CostItemValue * (ttslsPrice / totalValueProdwise);
                    }
                    if (costItem.CostItemType == CostItemType.ByPercentage.ToString())
                    {
                        if (firstIteration)
                            _line.CostItemLineValue = (costItem.CostItemValue / 100 * ttslsPrice) / totalValueProdwise;
                        else
                            _line.CostItemLineValue += (costItem.CostItemValue / 100 * ttslsPrice) / totalValueProdwise;
                    }
                }
                firstIteration = false;
            }

            var context = _context ?? new QMTContext();
            
            if (quotationLines.Count > 0)
            {
                context.SaveChanges();
            }
            return quotationLines;
        }

        public List<QuotationLine> GetQuotationLines(string quotationNum, int revNum, QMTContext _context)
        {
            var context = _context;
            var lines = context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 ).ToList();
            return lines;
        }

        public List<QuotationOptCode> GetQuotationOptCodes(string quotationNum , int lineNum , int revNum)
        {
            using (var context = new QMTContext())
            {
                var optCodeList = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 && x.LineNum == lineNum).ToList();
                return optCodeList;
            }
        }
        public List<QuotationCostItem> GetQuotationCostItems(string quotationNum,int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItems = context.QuotationCostItems.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 ).ToList();
           return costItems;
        }

        public void UpdateQuotationOptCodes(string quotationNum, int lineNum, int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            decimal? sumOfUnitPrice = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 && lineNum == x.LineNum).Sum(x=>x.UnitPrice);
            var line = context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 && x.LineNum == lineNum).FirstOrDefault();
            if (line != null)
            {
                line.UnitPrice = sumOfUnitPrice ?? 0;
                context.SaveChanges();
            }
        }

        public dynamic GetItemOptions(string itemCode)
        {
            using (var context = new QMTContext())
            {
                var optCodeList = (from a in context.PricingMasters
                                   join b in context.OptionMasters on a.OptCode equals b.OptCode
                                   where a.ItemCode == itemCode
                                   select new { optCode =  a.OptCode, price = a.Price, optName = b.OptName }).ToList();
                return optCodeList;
            }
        }

        public QuotationLine InsertQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.QuotationLines.Add(_quotationLine);
            context.SaveChanges();
            return _quotationLine;
        }

        public QuotationCostItem InsertQuotationCostItemLine(QuotationCostItem _quotationCostItem, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.QuotationCostItems.Add(_quotationCostItem);
            context.SaveChanges();
            return _quotationCostItem;
        }

        //insert if not found
        public QuotationOptCode InsertQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var optCode = context.QuotationOptCodes.Where(x => x.QuotationNum == _quotationOptCode.QuotationNum.ToUpper() && x.RevNum == _quotationOptCode.RevNum
                 && x.LineNum == _quotationOptCode.LineNum && _quotationOptCode.OptCode == x.OptCode).FirstOrDefault();
            if (optCode == null)
            {
                context.QuotationOptCodes.Add(_quotationOptCode);
                context.SaveChanges();
            }
            return _quotationOptCode;
        }

        public QuotationOptCode RemoveQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var optCode = context.QuotationOptCodes.Where(x => x.QuotationNum == _quotationOptCode.QuotationNum.ToUpper() && x.RevNum == _quotationOptCode.RevNum
                 && x.LineNum == _quotationOptCode.LineNum && _quotationOptCode.OptCode == x.OptCode).FirstOrDefault();
            if (optCode != null)
            {
                context.QuotationOptCodes.Remove(optCode);
                context.SaveChanges();
            }
            return optCode;
        }

        public PricingMaster? GetPricingOptCode(string itemCode, string optCode)
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x => x.ItemCode == itemCode.ToUpper() && x.OptCode == optCode.ToUpper()).OrderByDescending(x => x.Version).FirstOrDefault();
            }
        }

        public List<PricingMaster> GetPricingOptCode(string itemCode, List<string> optCode)
        {
            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x => x.ItemCode == itemCode.ToUpper() && optCode.Contains(x.OptCode)).ToList();
            }
        }

        public QuotationLine? GetLatestQuotationLine(string quotationNum)
        {

            using (var context = new QMTContext())
            {
                return context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper()).OrderByDescending(x => x.RevNum).OrderByDescending(x => x.LineNum).FirstOrDefault();
            }
        }

        public QuotationLine? GetQuotationLine(string quotationNum,int lineNum , int revNum)
        {

            using (var context = new QMTContext())
            {
                return context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.LineNum == lineNum && x.RevNum ==  revNum).FirstOrDefault();
            }
        }

        public QuotationHeader? GetQuotation(string quotationNum,int? revNum=null)
        {

            using (var context = new QMTContext())
            {
                if (revNum != null)
                    return context.QuotationHeaders.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum).FirstOrDefault();
                else
                    return context.QuotationHeaders.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.IsActiveRevision == true).FirstOrDefault();
            }
        }

        public decimal? GetSumOfOptPrice (string itemCode ,List<string> optCode)
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x => x.ItemCode == itemCode.ToUpper() && optCode.Contains(x.OptCode)).Sum(x => x.Price);
            }
        }

        public List<QuotationLineDC> GetQuotationLines(string quotationNum,int revNum ,List<int>? selectedLines=null,string prodTypeId="")
        {
            using (var context = new QMTContext())
            {
                List<QuotationLineDC> lines = (from ql in context.QuotationLines
                                               join im in context.ItemMasters on ql.ItemCode equals im.ItemCode
                                               join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                                               join ig in context.ItemGroupMasters on sm.GroupId equals ig.GroupId
                                               where ql.QuotationNum == quotationNum.ToUpper() && ql.RevNum == revNum
                                               select new QuotationLineDC
                                               {
                                                   LineNum = ql.LineNum,
                                                   Mtlp = ql.Mtlp,
                                                   ActiveLine = ql.ActiveLine,
                                                   Qty = ql.Qty,
                                                   RevNum = ql.RevNum,
                                                   QuotationNum = ql.QuotationNum,
                                                   ItemCode = ql.ItemCode ?? "",
                                                   ProdTypeId = ig.ProdTypeId ?? "",
                                                   UnitPrice = ql.UnitPrice,
                                                   TtslsPrice = ql.Qty * ql.Mtlp * ql.UnitPrice
                                               }).ToList();
                if (selectedLines != null)
                    lines = lines.Where(x => selectedLines.Contains(x.LineNum)).ToList();
                if(prodTypeId != "")
                    lines = lines.Where(x => prodTypeId == x.ProdTypeId).ToList();
                return lines;
            }
        }

        public List<QuotationLineDC> GetQuotationLinesOptions(List<QuotationLineDC> quotationLines)
        {
            using (var context = new QMTContext())
            {
                foreach (var line in quotationLines)
                {
                    List<string> optCodes = context.QuotationOptCodes.
                                               Where(x => x.QuotationNum == line.QuotationNum.ToUpper() && x.LineNum == line.LineNum && x.RevNum == line.RevNum)
                                               .Select(x => x.OptCode).ToList();
                    line.optCodes = string.Join(",",optCodes);
                }
                
                return quotationLines;
            }
        }
    }
}
