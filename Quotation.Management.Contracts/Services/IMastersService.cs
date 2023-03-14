using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;

namespace Quotation.Management.Contracts.Services
{
    public interface IMastersService
    {
        List<MasterDC> GetMasterData(string type);
        JObject? GetAllMasters();
        JObject? GetAllMastersForSearch();
        CurrencyDC GetCurrencyCode(string curencyCode, string oldCurrencyCode);

        CustomerMaster InsertCustomer(CustomerMaster customerMaster);
        void InsertMasterData(string type, string name);
        List<MasterDC> GetAllCustomers();

        List<MasterDC> GetAllCustomers(string searchString);

        List<MasterDC> GetCostItems();
        List<MasterDC> GetAllAreas();
        UserMaster? GetCurrentUserDetails(string email);
    }
}
