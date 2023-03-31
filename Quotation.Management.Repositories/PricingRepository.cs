using Microsoft.Extensions.Logging;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Quotation.Management.Repositories
{
    public class PricingRepository : IPricingRepository<PricingMaster>
    {
        #region variables
        private readonly ILogger<PricingRepository> _logger;
        #endregion
        public PricingRepository(ILogger<PricingRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public dynamic GetAll ()
        {

            using (var context = new QMTContext())
            {
                return (from pm in context.PricingMasters
                        join im in context.ItemMasters on pm.ItemCode equals im.ItemCode
                        join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                        join ig in context.ItemGroupMasters on sm.GroupId equals ig.GroupId
                        join bm in context.BrandMasters on sm.BrandId equals bm.BrandId
                        join pmo in context.ProductMasters on ig.ProdTypeId equals pmo.ProdTypeId
                        select new
                        {
                            pmo.ProdName,
                            ig.GroupName,
                            bm.BrandName,
                            sm.SeriesName,
                            pm.ItemCode,
                            pm.OptCode,
                            pm.Price,
                            pm.Version

                        }).ToList();

            }
        }

        public PricingMaster? GetPricing(string itemCode,string optCode, QMTContext? _context =null)
        {

            var context = _context ?? new QMTContext();
            var _data = context.PricingMasters.Where(x=>x.ItemCode == itemCode.ToUpper() && x.OptCode == optCode.ToUpper()).OrderByDescending(x => x.Version).FirstOrDefault();
            if (_context == null)
                context.Dispose();
            return _data;
        }

        public PricingMaster InsertPricing(PricingMaster pricing)
        {

            using (var context = new QMTContext())
            {
                PricingMaster _pricing = new PricingMaster();
                _pricing.ItemCode = pricing.ItemCode;
                _pricing.Price = pricing.Price;
                _pricing.OptCode = pricing.OptCode;
                context.PricingMasters.Add(_pricing);
                context.SaveChanges();
                return _pricing;
            }
        }


        public PricingMaster InsertPricingIfNotExist(PricingMaster _pricingMaster, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            PricingMaster? pricingMaster = context.PricingMasters.Where(x => x.ItemCode == _pricingMaster.ItemCode && x.OptCode == _pricingMaster.OptCode && x.Version == _pricingMaster.Version).FirstOrDefault();
            if (pricingMaster == null)
            {
                context.PricingMasters.Add(_pricingMaster);
                context.SaveChanges();
            }
            return _pricingMaster;
        }

        public PricingMaster DeletePricingIfNotExist(PricingMaster _pricingMaster, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var pricingMasterList = context.PricingMasters.Where(x => x.ItemCode == _pricingMaster.ItemCode && x.OptCode == _pricingMaster.OptCode).ToList();
            if (pricingMasterList.Count > 0)
            {
                //CHECK IF NO EXISTING QUOTATION USING IT 
                var quotationsUsingOptCodes = (from ql in context.QuotationLines
                                               join qo in context.QuotationOptCodes on new { ql.QuotationNum, ql.RevNum, ql.LineNum } equals new { qo.QuotationNum, qo.RevNum, qo.LineNum }
                                               where ql.ItemCode == _pricingMaster.ItemCode && qo.OptCode == _pricingMaster.OptCode
                                               select ql).Count();
                if (quotationsUsingOptCodes == 0)
                {
                    context.PricingMasters.RemoveRange(pricingMasterList);
                    context.SaveChanges();
                }
            }
            return _pricingMaster;
        }

        public bool MultipleInsertPricingData(List<PricingMaster> pricingList)
        {
            try
            {
                using (var context = new QMTContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        foreach (var pricing in pricingList)
                            context.PricingMasters.Add(pricing);

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
