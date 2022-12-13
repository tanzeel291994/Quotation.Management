using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using System.Data;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OptCodeController : ControllerBase
    {
        private readonly IOptCodeService _optCodeService;
        public OptCodeController(IOptCodeService optCodeService)
        {
            _optCodeService = optCodeService ?? throw new ArgumentNullException(nameof(optCodeService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(OptionMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetOptCodes()
        {
            var _optCodes = _optCodeService.GetOptCodes();
            return Ok(_optCodes);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OptionMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertOptCode(OptionMaster optCode)
        {
            var _optCode = _optCodeService.InsertOptCode(optCode);
            return Ok(_optCode);
        }

        [HttpPost("import/excel")]
        [ProducesResponseType(typeof(ItemMaster), StatusCodes.Status200OK)]
        public IActionResult ReadExcelFile()
        {
            try
            {
                var httpRequest = HttpContext.Request;
                List<string> validationMessages = new();
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
                        validationMessages.AddRange(_optCodeService.ImportData(ds));
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
