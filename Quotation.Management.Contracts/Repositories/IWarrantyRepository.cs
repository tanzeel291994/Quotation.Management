using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IWarrantyRepository : ITransactional
    {
        WarrantyHeader InsertUpdateWarranty(WarrantyHeader _warrantyHeader, int userId, QMTContext? _context = null);

        WarrantyLine InsertUpdateWarrantyLine(WarrantyLine _warrantyLine, QMTContext? _context = null);
        WarrantyHeader? GetWarranty(int _id);

        List<WarrantyLine> GetWarrantyLines(int _id);
        dynamic GetWarrantySearch(WarrantySearchDC input);
        List<string> GetAllJobRefs();

    }
}
