using Newtonsoft.Json.Linq;
using Quotation.Management.Entities.Models;

namespace Quotation.Management.Contracts.Services
{
    public interface IMastersService
    {
        JObject? GetAllMasters();
        JObject? GetAllMastersForSearch();
        CurrencyDC GetCurrencyCode(string curencyCode, string oldCurrencyCode);
    }
}
