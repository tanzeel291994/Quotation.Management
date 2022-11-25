using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class BrandRepository : IBrandRepository<BrandMaster>
    {
        public BrandRepository()
        {

        }

        public  List<BrandMaster> GetAll ()
        {

            using (var context = new QMTContext())
            {
               return  context.BrandMasters.ToList();
            }
        }

        public BrandMaster InsertBrandIfNotExist(BrandMaster _brand, QMTContext? _context = null)
        {
            // using (var context = _context ?? new QMTContext())
            //{
            var context = _context ?? new QMTContext();
                BrandMaster? brandMaster = context.BrandMasters.Where(x => x.BrandName == _brand.BrandName).FirstOrDefault();
                if (brandMaster == null)
                {
                    context.BrandMasters.Add(_brand);
                    context.SaveChanges();
                    return _brand;
                }
                return brandMaster;
            //}
        }

        public BrandMaster InsertBrand(BrandMaster brand)
        {

            using (var context = new QMTContext())
            {
                BrandMaster _brand = new BrandMaster();
                _brand.BrandName = brand.BrandName;
                context.BrandMasters.Add(_brand);
                context.SaveChanges();
                return _brand;
            }
        }
    }
}
