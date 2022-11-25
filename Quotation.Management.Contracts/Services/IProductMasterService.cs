using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IProductMasterService 
    {
        List<ProductMaster> GetProducts();

        ProductMaster InsertProduct(ProductMaster productMaster);
    }
}
