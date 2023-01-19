using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IItemCodeService
    {
        dynamic GetItemCodes();
        ItemMaster? InsertItemCode(ItemMaster itemCode);
        ItemCodeDetailsDC? GetItemCodeDetails(string itemCode);
        string? CreateItemCode(string baseItemCode, string option);
        List<MasterDC> GetItemCodes(string searchString);
        List<string> ImportData(DataSet ds);
    }
}
