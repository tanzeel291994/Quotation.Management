using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository<BrandMaster> _brandRepository;
        public BrandService(IBrandRepository<BrandMaster> brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }
        public List<BrandMaster> GetBrands()
        {
            return _brandRepository.GetAll();
        }
        public BrandMaster InsertBrand(BrandMaster brand)
        {
            return _brandRepository.InsertBrand(brand);
        }

    }
}
