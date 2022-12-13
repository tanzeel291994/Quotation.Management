using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductMasterService _productService;
        public ProductController(IProductMasterService productService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(ProductMaster),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public  IActionResult GetProducts()
        {
            var _productList =  _productService.GetProducts();
            return Ok(_productList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertProducts(ProductMaster productMaster)
        {
            var _productList = _productService.InsertProduct(productMaster);
            return Ok(_productList);
        }
    }
}
