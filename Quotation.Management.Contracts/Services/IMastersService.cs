using Newtonsoft.Json.Linq;

namespace Quotation.Management.Contracts.Services
{
    public interface IMastersService
    {
        JObject? GetAllMasters();
    }
}
