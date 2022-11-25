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

        public  List<PricingMaster> GetAll ()
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.ToList();
            }
        }
        public PricingMaster? GetPricing(string itemCode,string optCode)
        {

            using (var context = new QMTContext())
            {
                return context.PricingMasters.Where(x=>x.ItemCode == itemCode.ToUpper() && x.OptCode == optCode.ToUpper()).OrderByDescending(x => x.Version).FirstOrDefault();
            }
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
            //using (var context = _context ?? new QMTContext())
            //{
            var context = _context ?? new QMTContext();
            PricingMaster? pricingMaster = context.PricingMasters.Where(x => x.ItemCode == _pricingMaster.ItemCode && x.OptCode == _pricingMaster.OptCode).FirstOrDefault();
            if (pricingMaster == null)
            {
                context.PricingMasters.Add(_pricingMaster);
                context.SaveChanges();
                return _pricingMaster;
            }
            return pricingMaster;
            //}
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
