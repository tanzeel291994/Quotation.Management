using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Services;
using System.Data;
using ExcelDataReader;
using Newtonsoft.Json.Linq;

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
                var _quotation = _quotationService.GetQuotation(Id, revNum);
                return Ok(JsonConvert.SerializeObject(_quotation, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("all")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetQuotationsAll()
        {
            try
            {
                var _quotations = _quotationService.GetAllQuotationNums();
                return Ok(_quotations);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductsFromQuotation(string Id, int revNum )
        {
            try
            {
                var _products = _quotationService.GetProductsFromQuotation(Id, revNum);
                return Ok(JsonConvert.SerializeObject(_products));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Search")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SearchQuotations(QuotationSearchDC quotationSearch)
        {
            try
            {
                var _data = _quotationService.SearchQuotations(quotationSearch);
                return Ok(JsonConvert.SerializeObject(_data, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region Lines

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
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, JsonConvert.SerializeObject(ex._messages));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("line/delete")]
        [ProducesResponseType(typeof(QuotationCostItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCostItemLIne(QuotationLineDC input)
        {
            try
            {
                _quotationService.DeleteQuotationLine(input);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        #endregion

        #region CostLines
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
        [HttpPost("costLine/add")]
        [ProducesResponseType(typeof(QuotationCostItem), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddCostItemLIne([FromBody]List<QuotationCostItemDC> quotationCostItemDC)
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

        [HttpGet("currency/convfactor")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrencyConv(string code, string oldcode,string quotationNum , int revNum)
        {
            try
            {
                var _currencyDC = _quotationService.GetCurrencyCode(code, oldcode, quotationNum,revNum);
                return Ok(Newtonsoft.Json.JsonConvert.SerializeObject(_currencyDC));
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

        #endregion

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

        #region revisions

        [HttpGet("setActiveRevision")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetActiveRevision(string Id, int revNum)
        {
            try
            {
                _quotationService.SetActiveRevision(Id, revNum);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("allrevisions")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllRevisions(string Id)
        {
            try
            {
                var data = _quotationService.GetAllRevisions(Id);
                return Ok(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("createRevision")]
        [ProducesResponseType(typeof(QuotationHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateRevision(string Id, int revNum)
        {
            try
            {
                var data = _quotationService.CreateRevision(Id,revNum,0); //need to uodate userId
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

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

        [HttpPost("multiple/update")]
        [ProducesResponseType(typeof(QuotationLine), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMultipleLines(JObject data)
        {
            try
            {
                var result = _quotationService.UpdateMultipleLines(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        #region Options

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
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, JsonConvert.SerializeObject(ex._messages));
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
        public IActionResult GetQuotationLineNonStandadOptions(string quotationId, int revNum, int lineNum)
        {
            try
            {
                var _optCodes = _quotationService.GetQuotationLinesNonStandardOptCodes(quotationId, revNum, lineNum);
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

        #endregion

        [HttpPost("lines/import/excel")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
        public IActionResult ReadQuotationlinesFromExcelFile()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                List<string> validationMessages = new List<string>();
                IExcelDataReader? reader = null;
                DataSet ds = new();
                if (httpRequest.Form.Files.Count > 0)
                {
                    var inputFile = httpRequest.Form.Files[0];
                    string quotationNum = httpRequest.Form["QuotationNum"][0];
                    int revNum = Convert.ToInt32(httpRequest.Form["RevNum"][0]);
                    using (var fileStream = inputFile.OpenReadStream())
                    {
                        if (inputFile.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                        else if (inputFile.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                        else
                            throw new ValidationException(new List<string> { "File format not supported" });

                        ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                    }
                    if (ds != null && ds.Tables.Count > 0 && validationMessages.Count == 0)
                    {
                        _quotationService.ImportQuotationLines(ds, quotationNum,revNum);
                    }
                }
                return Ok();
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, JsonConvert.SerializeObject(ex._messages));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("import/header")]
        [ProducesResponseType(typeof(ItemMaster), StatusCodes.Status200OK)]
        public IActionResult ReadExcelFromFile()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                List<string> validationMessages = new List<string>();
                IExcelDataReader? reader = null;
                DataSet ds = new();
                if (httpRequest.Form.Files.Count > 0)
                {
                    var inputFile = httpRequest.Form.Files[0];
                    using (var fileStream = inputFile.OpenReadStream())
                    {
                        if (inputFile.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(fileStream);
                        else if (inputFile.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                        else
                            validationMessages.Add("File format not supported");

                        ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });
                    }
                    if (ds != null && ds.Tables.Count > 0 && validationMessages.Count == 0)
                    {
                        _quotationService.ImportData(ds);
                    }

                }
                return Ok(JsonConvert.SerializeObject(validationMessages));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
