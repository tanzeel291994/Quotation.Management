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
    public class ProductMasterRepository : BaseRepository<ProductMaster>, IProductMasterRepository<ProductMaster>
    {
        public ProductMasterRepository()
        {

        }

        public  List<ProductMaster> GetAll ()
        {
            //using (var context = _context ?? new QMTContext())
            //{
               return _context.ProductMasters.ToList();
            //}
        }

        public List<ProdItemTotal> GetProductsFromItemCodes(List<string> itemCodes)
        {
            using (var context =  new QMTContext())
            {
                dynamic productTypeIdList = (from im in context.ItemMasters
                                               join sm in context.SeriesMasters on im.SeriesId equals sm.SeriesId
                                               join ig in context.ItemGroupMasters on sm.GroupId equals ig.GroupId
                                               where itemCodes.Contains(im.ItemCode)
                                               select new ProdItemTotal  { ProdTypeId= 
                                               ig.ProdTypeId, 
                                                   ItemCode= im.ItemCode,
                                               TotalValue =0}).ToList();
                return productTypeIdList;
            }
        }

        public ProductMaster InsertProductIfNotExist(ProductMaster _productMaster, QMTContext? _context = null)
        {
            //using (var context = _context ?? base._context ?? new QMTContext())
            //{
            var context = _context ?? base._context ?? new QMTContext();
                ProductMaster? productMaster = context.ProductMasters.Where(x=> x.ProdTypeId == _productMaster.ProdTypeId).FirstOrDefault();
                if(productMaster == null)
                {
                    context.ProductMasters.Add(_productMaster);
                    context.SaveChanges();
                    return _productMaster;
                }
                return productMaster;
            //}
        }

        public ProductMaster InsertProduct(ProductMaster productMaster)
        {

            using (var context = new QMTContext())
            {
                ProductMaster _productMaster = new ProductMaster();
                _productMaster.ProdTypeId = productMaster.ProdTypeId;
                _productMaster.ProdName = productMaster.ProdName;
                context.ProductMasters.Add(productMaster);
                context.SaveChanges();
                return _productMaster;
            }
        }
    }
}
