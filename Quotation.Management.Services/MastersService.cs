using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class MastersService : IMastersService
    {
        private readonly IMastersRepository _mastersRepository;
        private readonly IItemCodeRepository<ItemMaster> _itemCodeRepository;
        private readonly ILogger<MastersService> _logger;
        public MastersService(ILogger<MastersService> logger, IMastersRepository mastersRepository, IItemCodeRepository<ItemMaster> itemCodeRepository)
        {
            _mastersRepository = mastersRepository ?? throw new ArgumentNullException(nameof(mastersRepository));
            _itemCodeRepository = itemCodeRepository ?? throw new ArgumentNullException(nameof(itemCodeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }
        public JObject? GetAllMasters()
        {
            JObject jobject = new();
            try
            {
                jobject.Add(new JProperty("users",JsonConvert.SerializeObject(_mastersRepository.GetUsers())));
                jobject.Add(new JProperty("areas", JsonConvert.SerializeObject(_mastersRepository.GetAreas())));
                jobject.Add(new JProperty("deliveryTerms", JsonConvert.SerializeObject(_mastersRepository.GetDeliveryTerms())));
                jobject.Add(new JProperty("paymentTerms", JsonConvert.SerializeObject(_mastersRepository.GetPaymentTerms())));
                jobject.Add(new JProperty("customers", JsonConvert.SerializeObject(_mastersRepository.GetCustomers())));
                jobject.Add(new JProperty("statuses", JsonConvert.SerializeObject(_mastersRepository.GetStatuses())));
                jobject.Add(new JProperty("currency", JsonConvert.SerializeObject(_mastersRepository.GetCurrency())));
                jobject.Add(new JProperty("industries", JsonConvert.SerializeObject(_mastersRepository.GetIndustrys())));
                
                //jobject.Add(new JProperty("itemCodes", JsonConvert.SerializeObject(_itemCodeRepository.GetAll())));
                //jobject.Add(new JProperty("products", JsonConvert.SerializeObject(_mastersRepository.GetProducts())));
                //jobject.Add(new JProperty("costItems", JsonConvert.SerializeObject(_mastersRepository.GetCostItems())));

                return jobject;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public void InsertCustomer(string code , string name , int type)
        {
            try
            {
                _mastersRepository.InsertCustomer(code,name,type);
            }
            catch(ValidationException ex)
            {
                throw;
            }
        }
        public List<MasterDC> GetAllCustomers()
        {
            return _mastersRepository.GetCustomers();
        }
        public List<MasterDC> GetAllAreas()
        {
            return _mastersRepository.GetAreas();
        }
        public List<MasterDC> GetMasterData(string type)
        {
            List<MasterDC> data = new();
            if (type == "CostItems") data = _mastersRepository.GetCostItems();
            else if (type == "PaymentTerms") data = _mastersRepository.GetPaymentTerms();
            else if (type == "DeliveryTerms") data = _mastersRepository.GetDeliveryTerms();
            else if (type == "Areas") data = _mastersRepository.GetDeliveryTerms();
            else if (type == "Currency") data = _mastersRepository.GetCurrency();
            else if (type == "Industry") data = _mastersRepository.GetIndustrys();
            else if (type == "Status") data = _mastersRepository.GetStatuses();
            else data = new();

            return data;
        }
        public void InsertMasterData(string code,int type,string name)
        {
            try
            {
                List<MasterDC> data = new();
                //if (type == "CostItems") _mastersRepository.InsertCostItem(name);
                //else if (type == "PaymentTerms") _mastersRepository.InsertPaymentTerm(name);
                //else if (type == "DeliveryTerms") _mastersRepository.InsertDeliveryTerm(name);
                //else if (type == "Areas") _mastersRepository.InsertSalesArea(name);
                //else if (type == "Currency")  _mastersRepository.InsertCurrency(name);
                //else if (type == "Industry") _mastersRepository.InsertIndustry(name);
                //else if (type == "Status")  _mastersRepository.InsertStatus(name);
                //else data = new();
                _mastersRepository.InsertMaster(code, name, (MasterEnum)type);
            }
             catch (ValidationException ex)
            {
                throw;
            }
        }
        public List<MasterDC> GetAllCustomers(string searchString)
        {
            return _mastersRepository.GetCustomers(searchString);
        }
        public UserMaster? GetCurrentUserDetails(string email)
        {
            UserMaster? userMaster = _mastersRepository.GetCurrentUserDetails(email);
            if (userMaster == null)
            {
                throw new ValidationException(new List<string> { "User not found" });
            }
            else
                return userMaster;
        }
        public List<MasterDC> GetCostItems()
        {
            return _mastersRepository.GetCostItems();
        }
        public JObject? GetAllMastersForSearch()
        {
            JObject jobject = new JObject();
            try
            {
                jobject.Add(new JProperty("users", JsonConvert.SerializeObject(_mastersRepository.GetUsers())));
                jobject.Add(new JProperty("areas", JsonConvert.SerializeObject(_mastersRepository.GetAreas())));
                jobject.Add(new JProperty("customers", JsonConvert.SerializeObject(_mastersRepository.GetCustomers())));
                jobject.Add(new JProperty("quotations", JsonConvert.SerializeObject(_mastersRepository.GetStatuses())));
                jobject.Add(new JProperty("brands", JsonConvert.SerializeObject(_mastersRepository.GetBrands())));
                jobject.Add(new JProperty("statuses", JsonConvert.SerializeObject(_mastersRepository.GetStatuses())));
                jobject.Add(new JProperty("products", JsonConvert.SerializeObject(_mastersRepository.GetProducts())));
                jobject.Add(new JProperty("projects", JsonConvert.SerializeObject(_mastersRepository.GetProjects())));
                jobject.Add(new JProperty("quotationYears", JsonConvert.SerializeObject(_mastersRepository.GetAllQuotationYears())));

                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public CurrencyDC GetCurrencyCode(string curencyCode,string oldCurrencyCode)
        {
            try
            {
                CurrencyMaster? currency = _mastersRepository.GetCurrencyByCode(curencyCode);
                CurrencyMaster? oldCurrency = _mastersRepository.GetCurrencyByCode(oldCurrencyCode);
                CurrencyDC currencyDC = new();
                currencyDC.OldCurrencyCode = oldCurrency!.CurrencyCode;
                currencyDC.CurrencyCode = currency!.CurrencyCode;
                currencyDC.ConvFactor = Math.Round(currency!.ConvFactor / oldCurrency.ConvFactor,2);
                return currencyDC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
