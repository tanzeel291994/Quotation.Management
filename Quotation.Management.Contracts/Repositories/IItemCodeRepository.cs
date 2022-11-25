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

        public List<string> ValidateAllItemCodes(List<string> itemCodes);

        ItemMaster? GetItemCode(string itemCodeName);

        bool MultipleInsertItemCode(List<ItemMaster> itemCodes);
    }
}
