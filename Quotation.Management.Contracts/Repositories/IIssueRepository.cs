using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface IIssueRepository<T> where T :class
    {
        dynamic GetAll();
        Issue InsertIssue(Issue issue);
    }
}
