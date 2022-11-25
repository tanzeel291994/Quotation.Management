using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public abstract class BaseRepository<T> :ITransactional where T:class
    {
        protected readonly QMTContext _context;
        protected BaseRepository()
        {
            _context ??= new QMTContext();
        }

        public QMTContext BeginTransaction()
        {
            _context.Database.BeginTransaction();
            return _context;
        }

        public virtual void Commit()
        {
            _context.Database.CommitTransaction();
        }

        public virtual void DisposeConnection()
        {
            _context.Dispose();
        }

        public virtual void RollBack()
        {
            _context.Database.RollbackTransaction();
        }

        //protected async Task<bool> ExistsAsync(object param)
        //{
        //    return 
        //}
    }
}
