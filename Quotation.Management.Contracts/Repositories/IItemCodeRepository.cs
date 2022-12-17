using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IItemCodeRepository<T> where T :class
    {
        dynamic GetAll();

        ItemMaster? InsertItemCode(ItemMaster itemCode);

        ItemMaster InsertItemCodeIfNotExist(ItemMaster _itemCode, QMTContext? _context = null);

        List<string> ValidateAllItemCodes(List<string> itemCodes, out List<string> validItemCodes);

        ItemMaster? GetItemCode(string itemCodeName);

        List<MasterDC> GetItemCodes(string searchString);
        bool MultipleInsertItemCode(List<ItemMaster> itemCodes);
    }
}
