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

        UserMaster? GetCurrentUserDetails(string email);
        CustomerMaster InsertCustomer(CustomerMaster customerMaster);
        void InsertCostItem(string name);
        void InsertPaymentTerm(string name);
        void InsertDeliveryTerm(string name);
        //void InsertSalesArea(string name);
        //void InsertCurrency(string name);
        //void InsertIndustry(string name);
        //void InsertStatus(string name);
    }
}
