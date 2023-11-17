using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Services;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarrantyController : ControllerBase
    {
        private readonly IWarrantyService _warrantyService;
        public WarrantyController(IWarrantyService warrantyService)
        {
            _warrantyService = warrantyService ?? throw new ArgumentNullException(nameof(warrantyService));
        }

        [HttpPost("header")]
        [ProducesResponseType(typeof(WarrantyHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertWarranty(WarrantyHeaderDC warrantyHeaderDC)
        {
            try
            {
                var _quoatation = _warrantyService.InsertWarrantyHeader(warrantyHeaderDC);
                return Ok(_quoatation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WarrantyHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetWarranty(string id)
        {
            try
            {
                var _warranty = _warrantyService.GetWarranty(id);
                return Ok(JsonConvert.SerializeObject(_warranty, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("lines")]
        [ProducesResponseType(typeof(WarrantyLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertWarrantyLine([FromBody]WarrantyLineDC warrantyLineDC)
        {
            try
            {
                _warrantyService.InsertWarrantyLine(warrantyLineDC);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("all")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetWararantyJobRefsAll()
        {
            try
            {
                var _quotations = _warrantyService.GetAllJobRefs();
                return Ok(_quotations);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Search")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchQuotations([FromBody] string json)
        {
            try
            {
                WarrantySearchDC warrantySearch = JsonConvert.DeserializeObject<WarrantySearchDC>(json);
                dynamic _data = _warrantyService.SearchQuotations(warrantySearch);
                JsonSerializerSettings _camelCase = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
               
                return Ok(JsonConvert.SerializeObject(_data, _camelCase)); //NED BETTER SOLUTION
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
