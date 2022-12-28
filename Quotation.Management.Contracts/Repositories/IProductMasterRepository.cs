using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface  IProductMasterRepository<T> : ITransactional where T:class
    {
        List<ProductMaster> GetAll();
        ProductMaster InsertProductIfNotExist(ProductMaster _productMaster, QMTContext? _context = null);
        ProductMaster InsertProduct(ProductMaster productMaster);

        List<MasterDC> GetProductsofQuotations(string quotationNum, int revNum);
        List<ProdItemTotal> GetProductsFromItemCodes(List<string> itemCodes);
    }
}
