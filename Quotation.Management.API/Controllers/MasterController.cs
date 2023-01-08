using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMastersService _mastersService;
        public MasterController(IMastersService mastersService)
        {
            _mastersService = mastersService ?? throw new ArgumentNullException(nameof(mastersService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var _masterList = _mastersService.GetAllMasters();
                return Ok(JsonConvert.SerializeObject(_masterList));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Customer/add")]
        [ProducesResponseType(typeof(CustomerMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertCustomer(CustomerMaster customerMaster)
        {
            try
            {
                var _customer = _mastersService.InsertCustomer(customerMaster);
                return Ok(_customer);
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, JsonConvert.SerializeObject(ex._messages));
            }

        }
        [HttpGet("customer/all")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCustomers()
        {
            try
            {
                var _customerList = _mastersService.GetAllCustomers();
                return Ok(JsonConvert.SerializeObject(_customerList));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("costItems")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCostItems()
        {
            try
            {
                var _costItemList = _mastersService.GetCostItems();
                return Ok(JsonConvert.SerializeObject(_costItemList));
            }

            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("Search/GetAll")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllForSearch()
        {
            try
            {
                var _masterList = _mastersService.GetAllMastersForSearch();
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(_masterList));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("currency/convfactor")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrencyConv(string code,string oldcode)
        {
            try
            {
                var _currencyDC = _mastersService.GetCurrencyCode(code,oldcode);
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(_currencyDC));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
