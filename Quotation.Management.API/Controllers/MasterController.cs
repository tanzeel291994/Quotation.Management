using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Quotation.Management.Contracts.Services;

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
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(_masterList));
            }
            catch(Exception ex)
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
