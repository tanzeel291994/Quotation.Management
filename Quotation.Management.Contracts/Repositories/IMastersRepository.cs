using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IMastersRepository
    {
        List<MasterDC> GetProjects();
        List<MasterDC> GetQuotations();
        List<MasterDC> GetBrands();
        List<MasterDC> GetAreas();
        List<MasterDC> GetDeliveryTerms();
        List<MasterDC> GetPaymentTerms();
        List<MasterDC> GetStatuses();
        List<MasterDC> GetCustomers();
        List<MasterDC> GetUsers();
        List<MasterDC> GetCurrency();
        List<MasterDC> GetCostItems(QMTContext? _context = null);
        List<MasterDC> GetProducts();
        UserMaster GetUserByUserId(int userId);
        List<MasterDC> GetAllQuotationYears();
        CurrencyMaster? GetCurrencyByCode(string currencyCode);
        CostItemCode GetCostItemByCode(string costItemId);
        List<MasterDC> GetCustomers(string searchString);
        List<MasterDC> GetIndustrys();
        List<MasterDC> GetBuyers(int type);
        UserMaster? GetCurrentUserDetails(string email);
        void InsertCustomer(string code, string name, int type);
        void InsertUser(UserMaster user);
        List<UserMaster> GetAllUsers();
        void UpdateUser(UserMaster user);
        void InsertMaster(string code, string name, MasterEnum type, decimal? convFactor);
    }
}
