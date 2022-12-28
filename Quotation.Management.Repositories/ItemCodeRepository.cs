using Microsoft.Extensions.Logging;
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
    public class ItemCodeRepository : IItemCodeRepository<ItemMaster>
    {

        #region variables
        private readonly ILogger<ItemCodeRepository> _logger;
        #endregion

        public ItemCodeRepository(ILogger<ItemCodeRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public  dynamic GetAll ()
        {

            using (var context = new QMTContext())
            {
                return context.ItemMasters.Select(x => new
                {
                    SeriesName = x.Series!.SeriesName,
                    SeriesId = x.Series.SeriesId,
                    ItemCode = x.ItemCode,
                }).ToList();
            }
        }

        public ItemMaster? GetItemCode(string itemCodeName)
        {

            using (var context = new QMTContext())
            {
                return context.ItemMasters.Where(x => x.ItemCode == itemCodeName.ToUpper()).FirstOrDefault();
            }
        }

        public CurrencyMaster? GetItemCodeCurrency(string itemCodeName)
        {

            using (var context = new QMTContext())
            {
                return (from im in context.ItemMasters
                        join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                        join bm in context.BrandMasters on sm.BrandId equals bm.BrandId
                        join cm in context.CurrencyMasters on bm.CurrencyCode equals cm.CurrencyCode
                        where im.ItemCode == itemCodeName
                        select cm).FirstOrDefault();
                      
            }
        }

        public List<MasterDC> GetItemCodes(string searchString)
        {

            using (var context = new QMTContext())
            {
                return context.ItemMasters.Where(x => x.ItemCode.StartsWith(searchString.ToUpper())).Select(x=>
                        new MasterDC { Code = x.ItemCode ,Name = x.ItemCode }).ToList();
            }
        }

        public ItemMaster InsertItemCodeIfNotExist(ItemMaster _itemCode, QMTContext? _context = null)
        {
            //using (var context = _context ?? new QMTContext())
            //{
                var context = _context ?? new QMTContext();
                ItemMaster? itemMaster = context.ItemMasters.Where(x => x.ItemCode == _itemCode.ItemCode && x.SeriesId == _itemCode.SeriesId).FirstOrDefault();
                if (itemMaster == null)
                {
                    context.ItemMasters.Add(_itemCode);
                    context.SaveChanges();
                    return _itemCode;
                }
                if (_context == null)
                    context.Dispose();
                return itemMaster;
            //}
        }

        public List<string> ValidateAllItemCodes(List<string> itemCodes ,out List<string> validItemCodes)
        {
            using (var context = new QMTContext())
            {
                List<string> validationMessages = new();
                validItemCodes = new();
                foreach (var itemCode in itemCodes)
                {
                    ItemMaster? itemMaster = context.ItemMasters.Where(x => x.ItemCode == itemCode).FirstOrDefault();
                    if (itemMaster == null)
                        validationMessages.Add("ItemCode " + itemCode + " doesnt exist in ItemMaster");
                    else
                        validItemCodes.Add(itemCode);
                }
                return validationMessages;
            }
        }


        public ItemMaster? InsertItemCode(ItemMaster itemCode)
        {
            try
            {
                using (var context = new QMTContext())
                {
                    ItemMaster _itemCode = new ItemMaster();
                    _itemCode.ItemCode = itemCode.ItemCode;
                    _itemCode.SeriesId = itemCode.SeriesId;
                    context.ItemMasters.Add(_itemCode);
                    context.SaveChanges();
                    return _itemCode;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                return null;
            }
        }

        public bool MultipleInsertItemCode(List<ItemMaster> itemCodes)
        {
            try
            {
                using (var context = new QMTContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        foreach(var itemCode in itemCodes)
                            context.ItemMasters.Add(itemCode);

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
