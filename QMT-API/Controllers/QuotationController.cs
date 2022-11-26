using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationService _quotationService;
        public QuotationController(IQuotationService quotationService)
        {
            _quotationService = quotationService ?? throw new ArgumentNullException(nameof(quotationService));
        }

        [HttpPost]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertQuotation(QuotationHeaderDC quotationHeaderDC)
        {
            try
            {
                var _quoatation = _quotationService.InsertQuotationHeader(quotationHeaderDC);
                return Ok(_quoatation);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotation(string  Id,int? revNum=null)
        {
            try
            {
                var _quotation = _quotationService.GetQuotation(Id);
                return Ok(JsonConvert.SerializeObject(_quotation));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("lines/options")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationLineOptions(string quotationId , int revNum)
        {
            try
            {
                var _lines = _quotationService.GetQuotationLinesOptCodes(quotationId, revNum);
                return Ok(JsonConvert.SerializeObject(_lines, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertQuotationLine(QuotationLineDC quotationLineDC)
        {
            try
            {
                var _quotationLine = _quotationService.InsertQuotationLine(quotationLineDC);
                return Ok(_quotationLine);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/copyoption")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CopyOptionLine(QuotationCopyOptionDC quotationCopyOptionDC)
        {
            try
            {
                var result = _quotationService.CopyOptionLine(quotationCopyOptionDC);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/update")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateQuotationLine(QuotationLineDC quotationLineDC)
        {
            try
            {
                var _quotationLine = _quotationService.UpdateQuotationLine(quotationLineDC);
                return Ok(_quotationLine);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("line/options")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetOptions(QuotationLineDC quotationLineDC)
        {
            try
            {
                var _quotationOptions = _quotationService.GetQuotationOptCodes(quotationLineDC);
                return Ok(JsonConvert.SerializeObject(_quotationOptions));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/options/add")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddOptions(QuotationLineDC quotationLineDC)
        {
            try
            {
                var _quotationOptions = _quotationService.InsertQuotationOptions(quotationLineDC);
                return Ok(JsonConvert.SerializeObject(_quotationOptions, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/options/remove")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RemoveOptions(QuotationLineDC quotationLineDC)
        {
            try
            {
                var _quotationOptions = _quotationService.RemoveQuotationOptions(quotationLineDC);
                return Ok(JsonConvert.SerializeObject(_quotationOptions, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
