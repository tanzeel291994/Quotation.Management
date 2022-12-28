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

        public QuotationLineDC UpdateQuotationLine(QuotationLineDC _quotationLine, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var line = context.QuotationLines.Where(x => x.QuotationNum == _quotationLine.QuotationNum.ToUpper() && x.RevNum == _quotationLine.RevNum
                 && x.LineNum == _quotationLine.LineNum).FirstOrDefault();
            if (line != null)
            {
                line.UnitPrice = _quotationLine.UnitPrice;
                line.Mtlp = _quotationLine.Mtlp;
                line.Qty = _quotationLine.Qty;
                line.ActiveLine = _quotationLine.ActiveLine;
                line.CostItemLineValue = _quotationLine.CostItemLineValue;
                line.Vat = _quotationLine.Vat;
                line.TtNetPrice = _quotationLine.TtNetPrice;
                line.SubItemCode = _quotationLine.SubItemCode; //check this 
                context.SaveChanges();
            }
            if(_context == null)
            {
                context.Dispose();
            }
            return _quotationLine;
        }

        public QuotationHeader UpdateQuotationHeader(QuotationHeader _quotationHeader, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.SaveChanges();
            return _quotationHeader;
        }
        public QuotationCostItem UpdateCostItem(QuotationCostItem _quotationCostItem, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.SaveChanges();
            return _quotationCostItem;
        }
        public QuotationCostItem DeleteCostItem(QuotationCostItem _quotationCostItem, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.Remove(_quotationCostItem);
            context.SaveChanges();
            return _quotationCostItem;
        }
        public void DeleteQuotationLine(QuotationLine _quotationLine, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.Remove(_quotationLine);
            context.SaveChanges();
            if (_context == null)
                context.Dispose();
        }
        public void DeleteCostItemGroup(string quotationNum, int linenum, int revNum,List<string> costItemGroupIds, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            List<QuotationCostItemLine> costItemLines = context.QuotationCostItemLines.Where(x => x.QuotationNum == quotationNum && costItemGroupIds.Contains(x.QuotationCostItemGroupId)
                                                  && revNum == x.RevNum).ToList();
            //if no cost item lines  then delete the costItemGroup.
            if(costItemLines.Count == 0)
            {
                List<QuotationCostItem> costItems = context.QuotationCostItems.Where(x => x.QuotationNum == quotationNum 
                                                    && costItemGroupIds.Contains(x.QuotationCostItemGroupId) && revNum == x.RevNum).ToList();
                context.RemoveRange(costItems);
            }
            context.SaveChanges();
            if (_context == null)
                context.Dispose();
        }

        public void DeleteQuotationOptions(string quotationNum,int linenum, int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            List<QuotationOptCode> quotationOpts = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationNum && x.LineNum == linenum
                                                   && revNum == x.RevNum).ToList();
            context.RemoveRange(quotationOpts);
            context.SaveChanges();
            if (_context == null)
                context.Dispose();
        }

        public void DeleteCostItemLines(string quotationNum, int linenum, int revNum, QMTContext? _context)
        {
            var context = _context ?? new QMTContext();
            List<QuotationCostItemLine> costItemLines = context.QuotationCostItemLines.Where(x => x.QuotationNum == quotationNum && x.LineNum == linenum
                                                   && revNum == x.RevNum).ToList();
            context.RemoveRange(costItemLines);
            context.SaveChanges();
            if (_context == null)
                context.Dispose();
        }

        public List<QuotationLine> UpdateCostValueOfAllQuotationLine(List<QuotationLine> quotationLines, List<QuotationCostItemLine> costItemLines, List<QuotationCostItem> costItems,Dictionary<string,decimal> groupIdTotalDict, QMTContext? _context)
        {

            //bool firstIteration = true;
            foreach (var costItem in costItems)
            {
                if (!groupIdTotalDict.ContainsKey(costItem.QuotationCostItemGroupId))
                    continue;
                decimal totalValueGroupWise = groupIdTotalDict[costItem.QuotationCostItemGroupId];
                foreach(var _costItemLine in costItemLines.Where(x =>x.QuotationCostItemGroupId == costItem.QuotationCostItemGroupId))
                {
                    QuotationLine? quotationLine = quotationLines.Where(x => x.LineNum == _costItemLine.LineNum).FirstOrDefault();
                    if (quotationLine != null)
                    {
                        decimal ttslsPrice = quotationLine.TtNetPrice;
                        if (costItem.CostItemType == CostItemType.ByVal.ToString())
                        {
                            _costItemLine.CostItemLineValue = costItem.CostItemValue * (ttslsPrice / totalValueGroupWise);
                        }
                        if (costItem.CostItemType == CostItemType.ByPercentage.ToString())
                        {
                            _costItemLine.CostItemLineValue = (costItem.CostItemValue / 100 * totalValueGroupWise);
                        }
                    }
                }
            }
            foreach (var _quotationLine in quotationLines)
            {
                List<QuotationCostItemLine> _costItemLines = costItemLines.Where(x => x.LineNum == _quotationLine.LineNum).ToList();
                _quotationLine.CostItemLineValue = _costItemLines.Select(x => x.CostItemLineValue).Sum();
            }
            var context = _context ?? new QMTContext();
            
            if (quotationLines.Count > 0)
            {
                context.SaveChanges();
            }
            return quotationLines;
        }

        public List<QuotationLine> GetQuotationLines(string quotationNum, int revNum, QMTContext? _context)
        {
            var context = _context ?? new QMTContext();
            var lines = context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 ).ToList();
            if (_context == null)
                context.Dispose();
            return lines;
        }

        public List<QuotationOptCode> GetQuotationOptCodes(string quotationNum , int revNum, QMTContext? _context, int? lineNum=null)
        {
            var context = _context ?? new QMTContext();
            var optCodeList = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum).ToList();
                if (lineNum != null)
                    optCodeList = optCodeList.Where(x=> x.LineNum == lineNum).ToList();
            if (_context == null)
                context.Dispose();
            return optCodeList;
        }

        public QuotationOptCode? GetQuotationOptCode(string quotationNum, int revNum, int lineNum,string optCode ,QMTContext? _context)
        {
            var context = _context ?? new QMTContext();
            var _optCode = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationNum.ToUpper() 
                              && x.RevNum == revNum && x.OptCode == optCode).FirstOrDefault();
            if (_context == null)
                context.Dispose();
            return _optCode;
        }
        public List<QuotationCostItem> GetQuotationCostItems(string quotationNum,int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItems = context.QuotationCostItems.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 ).ToList();
            if (_context == null)
                context.Dispose();
           return costItems;
        }
        public QuotationCostItem GetQuotationCostItem(string quotationNum, int revNum,string groupId, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItem = context.QuotationCostItems.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum 
                            && x.QuotationCostItemGroupId == groupId).First();
            if (_context == null)
                context.Dispose();
            return costItem;
        }
        public List<QuotationCostItemLine> GetQuotationCostItemLines(string quotationNum, int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItemLines = context.QuotationCostItemLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 ).ToList();
            if (_context == null)
                context.Dispose();
            return costItemLines;
        }
        public List<QuotationCostItemLine> GetQuotationCostItemLines(string quotationNum, int lineNum, int revNum, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItemLines = context.QuotationCostItemLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                && x.LineNum ==lineNum ).ToList();
            if (_context == null)
                context.Dispose();
            return costItemLines;
        }

        public void UpdateQuotationOptCodes(QuotationOptCode quotationOptCode, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var _optCode = context.QuotationOptCodes.Where(x => x.QuotationNum == quotationOptCode.QuotationNum && x.RevNum == quotationOptCode.RevNum
                 && quotationOptCode.LineNum == x.LineNum && quotationOptCode.OptCode == x.OptCode).FirstOrDefault();
            if (_optCode != null)
            {
                _optCode.UnitPrice = quotationOptCode.UnitPrice;
                context.SaveChanges();
            }
            if (_context == null)
            {
                context.Dispose();
            }

        }

        public dynamic GetItemOptions(string itemCode,decimal convFactor)
        {
            using (var context = new QMTContext())
            {
                var optCodeList = (from a in context.PricingMasters
                                   join b in context.OptionMasters on a.OptCode equals b.OptCode
                                   where a.ItemCode == itemCode
                                   select new { 
                                       optCode =  a.OptCode, 
                                       price = a.Price * convFactor, 
                                       optName = b.OptName ,
                                       isNet = b.Net ?? false }).ToList();
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
            if (_context == null)
            {
                context.Dispose();
            }
            return _quotationCostItem;
        }

        public List<QuotationCostItemLine> InUpdDelQuotationCostItemLines(string quotationNum , int revNum , string groupId,List<QuotationCostItemLine> _quotationCostItemLines, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var costItemLines = context.QuotationCostItemLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum
                 && x.QuotationCostItemGroupId == groupId).ToList();

            context.QuotationCostItemLines.RemoveRange(costItemLines);
            context.QuotationCostItemLines.AddRange(_quotationCostItemLines);
            context.SaveChanges();
            if (_context == null)
            {
                context.Dispose();
            }
            return _quotationCostItemLines;
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

        /*public PricingMaster? GetPricingOptCode(string itemCode, string optCode)
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x => x.ItemCode == itemCode.ToUpper() && x.OptCode == optCode.ToUpper()).OrderByDescending(x => x.Version).FirstOrDefault();
            }
        }*/

        public List<PricingMasterDC> GetPricingOptCode(string itemCode, List<string> optCode)
        {
            using (var context = new QMTContext())
            {
                List<PricingMasterDC> _pricingList = (from pm in context.PricingMasters
                                                      join im in context.ItemMasters on pm.ItemCode equals im.ItemCode
                                                      join om in context.OptionMasters on pm.OptCode equals om.OptCode
                                                      join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                                                      join bm in context.BrandMasters on sm.BrandId equals bm.BrandId
                                                      join cm in context.CurrencyMasters on bm.CurrencyCode equals cm.CurrencyCode
                                                      where pm.ItemCode == itemCode.ToUpper() && optCode.Contains(pm.OptCode)
                                                      select new PricingMasterDC
                                                      {
                                                          ItemCode = im.ItemCode,
                                                          OptCode = pm.OptCode,
                                                          CurrencyCode = bm.CurrencyCode,
                                                          ConvFactor = cm.ConvFactor,
                                                          ConvFactorByBrand = bm.ConvFactor,
                                                          Version = pm.Version,
                                                          Price = pm.Price ?? 0,
                                                          IsItemCodeCreation = om.IsItemCodeCreation ?? false,
                                                         IsNet = om.Net ?? false,
                                                      }).ToList();
                return _pricingList;
            }
        }

        public dynamic GetQuotationLinesSearch(QuotationSearchDC input)
        {
            using (var context = new QMTContext())
            {

                var _data = (from qh in context.QuotationHeaders
                             join ql in context.QuotationLines on new { qh.QuotationNum,qh.RevNum } equals new { ql.QuotationNum, ql.RevNum }
                             join im in context.ItemMasters on ql.ItemCode equals im.ItemCode
                             join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                             join ig in context.ItemGroupMasters on sm.GroupId equals ig.GroupId
                             join bm in context.BrandMasters on sm.BrandId equals bm.BrandId
                             join pm in context.ProductMasters on ig.ProdTypeId equals pm.ProdTypeId
                             where (qh.QuotationNum == input.QuotationNum  || input.QuotationNum ==null) &&
                            (qh.CustomerCode == input.CustomerCode || input.CustomerCode == null) &&
                            (qh.ProjectName == input.ProjectName || input.ProjectName == null) &&
                            (pm.ProdTypeId == input.Product || input.Product == null) &&
                             (bm.BrandId == input.BrandId || input.BrandId == null) &&
                            (qh.AreaCode == input.AreaCode || input.AreaCode == null) 
                            select new 
                            {
                                QuotationNum = qh.QuotationNum,
                                ProjectName = qh.ProjectName,
                                CustomerName = qh.CustomerCodeNavigation.CustomerName,
                                AreaName = qh.AreaCodeNavigation.AreaName,
                                LineNum = ql.LineNum,
                                BrandName = bm.BrandName,
                                ProductName = pm.ProdName,
                                SeriesName = sm.SeriesName,
                                RevNum = qh.RevNum,
                                IsActiveRevision = qh.IsActiveRevision
                            }).ToList();
                return _data;
            }
        }

        
        public dynamic GetQuotationSearch(QuotationSearchDC input)
        {
            using (var context = new QMTContext())
            {

                var _data = (from qh in context.QuotationHeaders
                             where (qh.QuotationNum == input.QuotationNum || input.QuotationNum == null) &&
                             (qh.CustomerCode == input.CustomerCode || input.CustomerCode == null) &&
                             (qh.ProjectName == input.ProjectName || input.ProjectName == null) &&
                             (qh.AreaCode == input.AreaCode || input.AreaCode == null)
                             select new
                             {
                                 QuotationNum = qh.QuotationNum,
                                 ProjectName = qh.ProjectName,
                                 CustomerName = qh.CustomerCodeNavigation.CustomerName,
                                 AreaName = qh.AreaCodeNavigation.AreaName,
                                 RevNum = qh.RevNum,
                                 IsActiveRevision = qh.IsActiveRevision
                             }).ToList();
                return _data;
            }
        }

        public QuotationLine? GetLatestQuotationLine(string quotationNum)
        {

            using (var context = new QMTContext())
            {
                return context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper()).OrderByDescending(x => x.RevNum).OrderByDescending(x => x.LineNum).FirstOrDefault();
            }
        }

        public QuotationLine GetQuotationLine(string quotationNum,int lineNum , int revNum, QMTContext? _context=null)
        {

            var context = _context ?? new QMTContext();
            QuotationLine? quotationLine = context.QuotationLines.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.LineNum == lineNum && x.RevNum ==  revNum).First();
            if(_context == null)
            {
                context.Dispose();
            }
            return quotationLine;
            
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
        public int GetQuotationLatestNum()
        {
            using (var context = new QMTContext())
            {
                if (!context.QuotationHeaders.Any())
                    return 1;
                int maxNum= context.QuotationHeaders.Select(x => Convert.ToInt32(x.QuotationNum.Substring(7, x.QuotationNum.Length - 1))).Max();
                return maxNum + 1;
            }
        }

        public QuotationHeader? GetQuotation(string quotationNum, int revNum ,QMTContext _context)
        {

            var context = _context ?? new QMTContext();
            return context.QuotationHeaders.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum).FirstOrDefault();
        }

        public List<QuotationLineDC> GetQuotationLinesDC(string quotationNum,int revNum ,List<int>? selectedLines=null,string prodTypeId="", QMTContext? _context=null)
        {
            var context = _context ?? new QMTContext();
            List<QuotationLineDC> lines = (from ql in context.QuotationLines
                                               join qh in context.QuotationHeaders on new { ql.QuotationNum, ql.RevNum } equals new { qh.QuotationNum, qh.RevNum }
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
                                                   ItemCode = ql.SubItemCode ?? ql.ItemCode ?? "",
                                                   ProdTypeId = ig.ProdTypeId ?? "",
                                                   UnitPrice = ql.UnitPrice,
                                                   Vat = ql.Vat,
                                                   CurrencyCode = qh.CurrencyCode,
                                                   TtNetPrice = ql.TtNetPrice,
                                                   CostItemLineValue = ql.CostItemLineValue ?? 0,
                                                   TtslsPriceWOVat = Math.Round(ql.TtNetPrice + (ql.CostItemLineValue ?? 0), 2),
                                                   TtslsPrice = Math.Round((100+(ql.Vat))/100 * (ql.TtNetPrice + (ql.CostItemLineValue ?? 0)),2)
                                               }).ToList();
                if (selectedLines != null)
                    lines = lines.Where(x => selectedLines.Contains(x.LineNum)).ToList();
                if(prodTypeId != "")
                    lines = lines.Where(x => prodTypeId == x.ProdTypeId).ToList();
                return lines;
            
        }

        public List<QuotationOptCodeDC> GetQuotationLinesOptions(string quotationNum, int revNum)
        {
            using (var context = new QMTContext())
            {
                    List<QuotationOptCodeDC> optCodes = (from qoc in context.QuotationOptCodes
                                                         join om in context.OptionMasters on  qoc.OptCode  equals  om.OptCode
                                                         join ql in context.QuotationLines on new { qoc.QuotationNum, qoc.LineNum, qoc.RevNum } equals new { ql.QuotationNum, ql.LineNum, ql.RevNum }
                                                         where qoc.QuotationNum == quotationNum.ToUpper() && qoc.RevNum == revNum 
                                                         select new QuotationOptCodeDC
                                                         {
                                                            QuotationNum = qoc.QuotationNum,
                                                            RevNum = qoc.RevNum,
                                                            LineNum = ql.LineNum,
                                                            ItemCode = ql.ItemCode,
                                                            OptCode = qoc.OptCode,
                                                            OptName = qoc.OptName ?? qoc.OptCode,
                                                            IsNet = qoc.IsNet ?? false,
                                                            OptType = qoc.OptType ?? "STANDRAD",

                                                         }).ToList();
                return optCodes;
            }
        }

        public List<QuotationOptCodeDC> GetQuotationLinesNonStandardOptions(string quotationNum, int revNum)
        {
            using (var context = new QMTContext())
            {
                List<QuotationOptCodeDC> optCodes = (from qoc in context.QuotationOptCodes
                                                     join ql in context.QuotationLines on new { qoc.QuotationNum, qoc.LineNum, qoc.RevNum } equals new { ql.QuotationNum, ql.LineNum, ql.RevNum }
                                                     where qoc.QuotationNum == quotationNum.ToUpper() && qoc.RevNum == revNum 
                                                     && qoc.OptType == OptionType.NonStandard.ToString()
                                                     select new QuotationOptCodeDC
                                                     {
                                                         QuotationNum = qoc.QuotationNum,
                                                         RevNum = qoc.RevNum,
                                                         LineNum = ql.LineNum,
                                                         Price = qoc.UnitPrice,
                                                         ItemCode = ql.ItemCode,
                                                         OptCode = qoc.OptCode,
                                                         OptName = qoc.OptName ?? qoc.OptCode,
                                                         IsNet = qoc.IsNet ?? false,
                                                         OptType = qoc.OptType ?? OptionType.NonStandard.ToString(),

                                                     }).ToList();
                return optCodes;
            }
        }

        public List<string> GetQuotationOptions(string quotationNum , int revNum)
        {
            using (var context = new QMTContext())
            {
                List<string> optCodes = (from q in context.QuotationHeaders
                                         join qc in context.QuotationOptCodes on new { q.QuotationNum, q.RevNum } equals new { qc.QuotationNum, qc.RevNum }
                                         into result
                                         from r in result
                                         where r.OptCode != null
                                         select r.OptCode).Distinct().ToList();
                return optCodes;
            }
        }

        public List<QuotationCostItemDC> GetQuotationCostLines(string quotatioNum , int revNum)
        {
            using (var context = new QMTContext())
            {

                List<QuotationCostItemDC>  quotationCostItems = context.QuotationCostItems.
                                                Where(x => x.QuotationNum == quotatioNum.ToUpper()  && x.RevNum == revNum)
                                                .Select( x => new QuotationCostItemDC
                                                {
                                                    QuotationNum = x.QuotationNum,
                                                    CostItemId = x.CostItemId,
                                                    CostItemType = x.CostItemType,
                                                    CostItemValue = x.CostItemValue,
                                                    QuotationCostItemGroupId = x.QuotationCostItemGroupId,
                                                    ProdTypeId = x.ProdTypeId,
                                                    RevNum = x.RevNum,
                                                })
                                               .ToList();
                foreach(var _costItem in quotationCostItems)
                {
                    List<QuotationLineCostItem> quotCostItemLines = (from qc in context.QuotationCostItemLines
                                                                    join ql in context.QuotationLines on new { qc.QuotationNum, qc.RevNum,qc.LineNum } equals new { ql.QuotationNum, ql.RevNum, ql.LineNum }
                                                                    where ql.QuotationNum == quotatioNum.ToUpper() && ql.RevNum == revNum 
                                                                    && qc.QuotationCostItemGroupId == _costItem.QuotationCostItemGroupId
                                                                    select new QuotationLineCostItem
                                                                    {
                                                                        LineNum = ql.LineNum,
                                                                        ItemCode = ql.ItemCode
                                                                    }).ToList();
                    _costItem.quotationLineCostItems = quotCostItemLines.ToArray();
                }


                return quotationCostItems;
            }
        }

        public QuotationHeaderDC? GetQuotationHeader(string quotatioNum, int revNum)
        {
            using (var context = new QMTContext())
            {

                var quotationHeaderDC = context.QuotationHeaders.
                                                Where(x => x.QuotationNum == quotatioNum.ToUpper() && x.RevNum == revNum)
                                                .Select(x => new QuotationHeaderDC
                                                {
                                                    QuotationNum = x.QuotationNum,
                                                    RevNum= x.RevNum,
                                                    MspName = x.MspNavigation.FirstName+' '+ x.MspNavigation.LastName,
                                                    AreaName = x.AreaCodeNavigation.AreaName,
                                                    StatusName = x.Status.StatusName,
                                                    ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                                                    ProjectName = x.ProjectName,
                                                    Probability = x.Probability,
                                                    CurrencyCode = x.CurrencyCode,
                                                    QuotationDate = x.QuotationDate,
                                                    CustomerName = x.CustomerCodeNavigation.CustomerName
                                                })
                                               .FirstOrDefault();
                return quotationHeaderDC;
            }
        }
    }
}
