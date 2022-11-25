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

        List<string> ImportData(DataSet ds);
    }
}
