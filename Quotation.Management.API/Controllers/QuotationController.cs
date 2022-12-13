using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using System.Data;

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

        [HttpGet("lines")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationLines(string Id, int revNum)
        {
            try
            {
                var _quotationLines = _quotationService.GetQuotationLines(Id, revNum);
                return Ok(JsonConvert.SerializeObject(_quotationLines, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("cost/lines")]
        [ProducesResponseType(typeof(PriceBreakDownDC), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationCostLines(string Id, int revNum)
        {
            try
            {
                var _quotationCostItems = _quotationService.GetQuotationCostLines(Id, revNum);
                return Ok(JsonConvert.SerializeObject(_quotationCostItems, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("pbd")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationPBD(string quotationNum, int revNum)
        {
            try
            {
                PriceBreakDownDC _quotationPBD = _quotationService.GetQuotationPBD(quotationNum, revNum);
                return Ok(JsonConvert.SerializeObject(_quotationPBD));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("lines/options")]
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationLineOptions(string quotationId, int revNum)
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
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
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

        [HttpPost("line/nonstandard/options/add")]
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddNonStandadOption(QuotationNonStandardOptCodeDC nonStandardOptCodeDC)
        {
            try
            {
                var result = _quotationService.InsertNonStandardOption(nonStandardOptCodeDC);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/nonstandard/options/remove")]
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RemoveNonStandadOption(QuotationNonStandardOptCodeDC nonStandardOptCodeDC)
        {
            try
            {
                var result = _quotationService.RemoveNonStandardOption(nonStandardOptCodeDC);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("line/nonstandard/options")]
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationLineNonStandadOptions(string quotationId, int revNum)
        {
            try
            {
                var _optCodes = _quotationService.GetQuotationLinesNonStandardOptCodes(quotationId, revNum);
                return Ok(JsonConvert.SerializeObject(_optCodes, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("costLine/add")]
        [ProducesResponseType(typeof(QuotationCostItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddCostItemLIne(QuotationCostItemDC quotationCostItemDC)
        {
            try
            {
                var result = _quotationService.InsertQuotationCostItem(quotationCostItemDC);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("costLine/update")]
        [ProducesResponseType(typeof(QuotationCostItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCostItemLIne(QuotationCostItemDC quotationCostItemDC)
        {
            try
            {
                var result = _quotationService.UpdateQuotationCostItem(quotationCostItemDC);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("costLine/delete")]
        [ProducesResponseType(typeof(QuotationCostItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCostItemLIne(QuotationCostItemDC quotationCostItemDC)
        {
            try
            {
                var result = _quotationService.DeleteQuotationCostItem(quotationCostItemDC);
                return Ok(JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPost("currency/update")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateQuotationCurrency(CurrencyDC currencyDC)
        {
            try
            {
                var result = _quotationService.UpdateQuotationCurrency(currencyDC);
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
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(QuotationOptCode), StatusCodes.Status200OK)]
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
