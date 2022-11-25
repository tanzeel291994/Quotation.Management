using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IBrandRepository<T> where T :class
    {
        List<BrandMaster> GetAll();

        BrandMaster InsertBrandIfNotExist(BrandMaster _brand, QMTContext? _context = null);
        BrandMaster InsertBrand(BrandMaster brand);
    }
}
