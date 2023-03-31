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
        List<MasterDC> GetBuyers(int type);
        void InsertCustomer(string code, string name, int type);
        void InsertMasterData(string code, int type, string named,decimal? convFactor);
        List<MasterDC> GetAllCustomers();

        List<MasterDC> GetAllCustomers(string searchString);
        List<MasterDC> GetCostItems();
        List<MasterDC> GetAllAreas();
        List<UserMaster> GetAllUsers();
        void InsertUser(UserMaster user);
        void UpdateUser(UserMaster user);
        UserMaster? GetCurrentUserDetails(string email);
    }
}
