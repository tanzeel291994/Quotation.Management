using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IOptCodeService
    {
        List<OptionMaster> GetOptCodes();

        OptionMaster InsertOptCode(OptionMaster brand);

        List<string> ImportData(DataSet ds);
    }
}
