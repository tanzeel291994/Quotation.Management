using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public  interface ITransactional
    {
        QMTContext BeginTransaction();
        void Commit();
        void RollBack();
        void DisposeConnection();
    }
}
