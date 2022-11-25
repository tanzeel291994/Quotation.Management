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
    public class ItemGroupService : IItemGroupService
    {
        private readonly IItemGroupRepository<ItemGroupMaster> _itemGroupRepository;
        public ItemGroupService(IItemGroupRepository<ItemGroupMaster> itemGroupRepository)
        {
            _itemGroupRepository = itemGroupRepository ?? throw new ArgumentNullException(nameof(itemGroupRepository));
        }
        public List<ItemGroupMaster> GetItemGroups()
        {
            return _itemGroupRepository.GetAll();
        }
        public ItemGroupMaster InsertItemGroup(ItemGroupMaster itemGroup)
        {
            return _itemGroupRepository.InsertItemGroup(itemGroup);
        }

    }
}
