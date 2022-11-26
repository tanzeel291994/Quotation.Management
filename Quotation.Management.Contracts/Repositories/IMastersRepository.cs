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
        List<MasterDC> GetAreas();
        List<MasterDC> GetDeliveryTerms();
        List<MasterDC> GetPaymentTerms();
        List<MasterDC> GetStatuses();
        List<MasterDC> GetCustomers();
        List<MasterDC> GetUsers();
        List<MasterDC> GetCurrency();
        List<MasterDC> GetCostItems();
        List<MasterDC> GetProducts();

        CurrencyMaster? GetCurrencyByCode(string currencyCode);
    }
}
