using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class IssueService : IIssueService
    {
        private readonly IIssueRepository<Issue> _issueRepository;
        public IssueService(IIssueRepository<Issue> issueRepository)
        {
            _issueRepository = issueRepository ?? throw new ArgumentNullException(nameof(issueRepository));
        }
        public dynamic GetAll()
        {
            return _issueRepository.GetAll();
        }
        public Issue InsertIssue(Issue issue)
        {
            return _issueRepository.InsertIssue(issue);
        }

    }
}
