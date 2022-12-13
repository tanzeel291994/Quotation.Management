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
    public class ItemCodeController : ControllerBase
    {
        private readonly IItemCodeService _itemCodeService;
        public ItemCodeController(IItemCodeService itemCodeService)
        {
            _itemCodeService = itemCodeService ?? throw new ArgumentNullException(nameof(itemCodeService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(BrandMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetItemCodes()
        {
            var _itemCodeList = _itemCodeService.GetItemCodes();
            return Ok(_itemCodeList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertItemCode(ItemMaster itemCode)
        {
            var _itemCode = _itemCodeService.InsertItemCode(itemCode);
            return Ok(_itemCode);
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
                        validationMessages.AddRange(_itemCodeService.ImportData(ds));
                    }

                }
                return Ok(JsonConvert.SerializeObject(validationMessages));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }
    }
}
