using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IPricingService
    {
        List<PricingMaster> GetPricings();

        PricingMaster InsertPricing(PricingMaster pricing);
        List<string> ImportPricingData(DataSet ds);
        List<string> ImportData(DataSet ds);
    }
}
