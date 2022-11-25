using Microsoft.EntityFrameworkCore.ChangeTracking;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class ItemGroupRepository : IItemGroupRepository<ItemGroupMaster>
    {
        public ItemGroupRepository()
        {

        }

        public  List<ItemGroupMaster> GetAll ()
        {

            using (var context = new QMTContext())
            {
               return  context.ItemGroupMasters.ToList();
            }
        }

        public ItemGroupMaster InsertItemGroupIfNotExist(ItemGroupMaster _itemGroup, QMTContext? _context = null)
        {
            //using (var context = _context  ?? new QMTContext())
            //{
            var context = _context ?? new QMTContext();
                ItemGroupMaster? itemGroupMaster = context.ItemGroupMasters.Where(x => x.GroupName == _itemGroup.GroupName).FirstOrDefault();
                if (itemGroupMaster == null)
                {
                    context.ItemGroupMasters.Add(_itemGroup);
                    context.SaveChanges();
                    return _itemGroup;
                }
                return itemGroupMaster;
            //}
        }

        public ItemGroupMaster InsertItemGroup(ItemGroupMaster itemGroup)
        {

            using (var context = new QMTContext())
            {
                ItemGroupMaster _itemGroup = new ItemGroupMaster();
                _itemGroup.GroupName = itemGroup.GroupName;
                _itemGroup.ProdTypeId = itemGroup.ProdTypeId;
                context.ItemGroupMasters.Add(_itemGroup);
                context.SaveChanges();
                return _itemGroup;
            }
        }
    }
}
