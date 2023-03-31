using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
        [HttpGet("users")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUsers()
        {
            try
            {
                var _masterList = _mastersService.GetAllUsers();
                return Ok(JsonConvert.SerializeObject(_masterList, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Buyer/add")]
        [ProducesResponseType(typeof(CustomerMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertCustomer(string code, string name, int type)
        {
            try
            {
                 _mastersService.InsertCustomer(code,name,type);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, JsonConvert.SerializeObject(ex._messages));
            }

        }
        [HttpGet("buyers/all")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetBuyers(int type)
        {
            try
            {
                var _buyerList = _mastersService.GetBuyers(type);
                return Ok(JsonConvert.SerializeObject(_buyerList));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("area/all")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSalesAreas()
        {
            try
            {
                var _areaList = _mastersService.GetAllAreas();
                return Ok(JsonConvert.SerializeObject(_areaList));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("all")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetMasterData(string type)
        {
            try
            {
                var _data = _mastersService.GetMasterData(type);
                return Ok(JsonConvert.SerializeObject(_data));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("insert")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertMasterData([FromBody] string json)
        {
            try
            {
                JObject data = JObject.Parse(json);
                var code = data["code"] ?? "";
                var type = data["type"];
                var name = data["name"];
                var convFactor = data["convFactor"] ?? null;
                MasterEnum typeEnum = ((MasterEnum) Enum.Parse(typeof(MasterEnum), type.ToString().ToUpper()));
                _mastersService.InsertMasterData(code.ToString(), (int)typeEnum, name.ToString(), (decimal?) convFactor);
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

        [HttpPost("insert/user")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertUser([FromBody]UserMaster user)
        {
            try
            {
                _mastersService.InsertUser(user);
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

        [HttpPost("update/user")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateUser([FromBody] UserMaster user)
        {
            try
            {
                _mastersService.UpdateUser(user);
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

        [HttpGet("customer/filter")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCustomers(string searchString)
        {
            try
            {
                var _customerList = _mastersService.GetAllCustomers(searchString);
                return Ok(JsonConvert.SerializeObject(_customerList));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("getCurrentUser")]
        [ProducesResponseType(typeof(JObject), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetCurrentUserDetails(string email)
        {
            try
            {
                var _user = _mastersService.GetCurrentUserDetails(email);
                return Ok(JsonConvert.SerializeObject(_user));
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

        
    }
}
