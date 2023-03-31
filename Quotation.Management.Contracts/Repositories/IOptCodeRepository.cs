using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IOptCodeRepository<T> : ITransactional where T : class
    {
        List<OptionMaster> GetAll();
        OptionMaster InsertOptCode(OptionMaster brand);
        OptionMaster? GetOptCode(string optName);
        OptionMaster InsertOrUpdateOptCodeIfNotExist(OptionMaster _optCodeMaster, QMTContext? _context = null);
        bool MultipleInsertOptCodes(List<OptionMaster> itemCodes);

    }
}
