using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IIssueService
    {
        dynamic GetAll();

        Issue InsertIssue(Issue issue);
    }
}
