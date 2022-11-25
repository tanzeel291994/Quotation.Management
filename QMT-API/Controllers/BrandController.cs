using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BrandMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetBrands()
        {
            var _brandList = _brandService.GetBrands();
            return Ok(_brandList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BrandMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertBrand(BrandMaster brand)
        {
            var _brand = _brandService.InsertBrand(brand);
            return Ok(_brand);
        }
    }
}
