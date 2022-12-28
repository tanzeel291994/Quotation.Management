using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IPricingRepository<T> where T :class
    {
        dynamic GetAll();

        PricingMaster InsertPricing(PricingMaster pricing);

        PricingMaster? GetPricing(string itemCode, string optCode, QMTContext? _context = null);

        PricingMaster InsertPricingIfNotExist(PricingMaster _pricingMaster, QMTContext? _context = null);

        bool MultipleInsertPricingData(List<PricingMaster> pricingList);
    }
}
