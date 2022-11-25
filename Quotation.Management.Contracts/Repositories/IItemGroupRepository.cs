using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface  IItemGroupRepository<T> where T :class
    {
        List<ItemGroupMaster> GetAll();

        ItemGroupMaster InsertItemGroupIfNotExist(ItemGroupMaster _itemGroup, QMTContext? _context = null);
        ItemGroupMaster InsertItemGroup(ItemGroupMaster itemGroupMaster);
    }
}
