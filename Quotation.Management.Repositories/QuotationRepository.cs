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
                context.SaveChanges();
            }
            context.SaveChanges();
            return _quotationLine;
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

        public QuotationOptCode InsertQuotationOptCode(QuotationOptCode _quotationOptCode, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            context.QuotationOptCodes.Add(_quotationOptCode);
            context.SaveChanges();
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

        public QuotationHeader? GetQuotation(string quotationNum,int revNum)
        {

            using (var context = new QMTContext())
            {
                return context.QuotationHeaders.Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum).FirstOrDefault();
            }
        }

        public decimal? GetSumOfOptPrice (string itemCode ,List<string> optCode)
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x => x.ItemCode == itemCode.ToUpper() && optCode.Contains(x.OptCode)).Sum(x => x.Price);
            }
        }

        public List<QuotationLineDC> GetQuotationLines(string quotationNum,int revNum)
        {
            using (var context = new QMTContext())
            {
                List<QuotationLineDC> lines = context.QuotationLines.
                                              Where(x => x.QuotationNum == quotationNum.ToUpper() && x.RevNum == revNum)
                                              .Select(x=> new QuotationLineDC { 
                                                  LineNum = x.LineNum,
                                                  Mtlp = x.Mtlp,
                                                  ActiveLine = x.ActiveLine,
                                                  Qty = x.Qty,
                                                  RevNum = x.RevNum,
                                                  QuotationNum = x.QuotationNum,
                                                  ItemCode = x.ItemCode,
                                                  UnitPrice = x.UnitPrice,
                                                  TtslsPrice = x.Qty * x.Mtlp * x.UnitPrice                                              
                                              }).ToList();
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
