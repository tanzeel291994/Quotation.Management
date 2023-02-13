using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using System.Data;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;
        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService ?? throw new ArgumentNullException(nameof(pricingService));
        }

        [HttpGet]
        //[ProducesResponseType(typeof(BrandMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPricings()
        {
            var _pricingList = _pricingService.GetPricings();
            return Ok(_pricingList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BrandMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertBrand(PricingMaster pricing)
        {
            var _pricing = _pricingService.InsertPricing(pricing);
            return Ok(_pricing);
        }

        [HttpPost("import/excel")]
        [ProducesResponseType(typeof(ItemMaster), StatusCodes.Status200OK)]
        public IActionResult ReadExcelFile()
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
                        validationMessages.AddRange(_pricingService.ImportData(ds));
                    }

                }
                if (validationMessages.Count > 0) throw new ValidationException(validationMessages);
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

        [HttpPost("import/excel/pricing")]
        [ProducesResponseType(typeof(ItemMaster), StatusCodes.Status200OK)]
        public IActionResult ReadExcelFileForPricing()
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
                        validationMessages.AddRange(_pricingService.ImportPricingData(ds));
                    }

                }
                if (validationMessages.Count > 0) throw new ValidationException(validationMessages);
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

    }
}
