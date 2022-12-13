using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemGroupController : ControllerBase
    {
        private readonly IItemGroupService _itemGroupService;
        public ItemGroupController(IItemGroupService itemGroupService)
        {
            _itemGroupService = itemGroupService ?? throw new ArgumentNullException(nameof(itemGroupService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ItemGroupMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetItemGroups()
        {
            var _itemGroupList = _itemGroupService.GetItemGroups();
            return Ok(_itemGroupList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertItemGroups(ItemGroupMaster itemGroup)
        {
            var _itemGroup = _itemGroupService.InsertItemGroup(itemGroup);
            return Ok(_itemGroup);
        }
    }
}
