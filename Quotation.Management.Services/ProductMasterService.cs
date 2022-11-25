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
    public class ProductMasterService : IProductMasterService
    {
        private readonly IProductMasterRepository<ProductMaster> _productMasterRepository;
        public ProductMasterService(IProductMasterRepository<ProductMaster> productMasterRepository)
        {
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
        }
        public List<ProductMaster> GetProducts()
        {
            return _productMasterRepository.GetAll();
        }
        public ProductMaster InsertProduct(ProductMaster productMaster)
        {
            return _productMasterRepository.InsertProduct(productMaster);
        }
        
    }
}
