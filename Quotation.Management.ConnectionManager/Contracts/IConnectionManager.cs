using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.ConnectionManager.Contracts
{
    public interface IConnectionManager
    {
        void BeginTransaction();
        void Commit();
        void RollBack();
        void Dispose();
        Task<int> ExecuteAsync(string query, object arg = null);
        Task<T> ExecuteScalarAsync<T>(string query, object arg = null);
        Task<IEnumerable<T>> QueryAsync<T>(string query);

        Task<int> InsertAsync<T>(T entity) where T : class;
        Task<int> UpdateAsync<T>(T entity, string condition, object condiitonParam = null) where T : class;

        Task<bool> ExistAsync<T>(object arg) where T: class;
        Task<T> QuerySingleAsync<T>(string condition , object param =null);
        Task<T> QueryAllAsync<T>(string condition , object param =null);
        Task<T> QuerySpecificSingleAsync<T>(string queryColumn,string condition , object param =null);
        Task<IEnumerable<T>> QueryAllAsyncWithPagination<T>(string condition, int pageNumber,int pageSize , object param =null);

    }
}
