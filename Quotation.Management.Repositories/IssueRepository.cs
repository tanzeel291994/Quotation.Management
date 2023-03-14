using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class IssueRepository : IIssueRepository<Issue>
    {
        public IssueRepository()
        {

        }

        public  dynamic GetAll ()
        {

            using (var context = new QMTContext())
            {
                return context.Issues.Select(x => new {
                    Title = x.Title,
                    Status = x.Status,
                    Type = x.Type,
                    Id = x.Id,
                    CreatedOn = x.CreatedOn,
                    AdditionalRemarks = x.AdditionalRemarks,
                    ReportedBy = x.CreatedByNavigation.FirstName+" "+x.CreatedByNavigation.LastName,

                }).ToList();
            }
        }


        public Issue InsertIssue(Issue issue)
        {

            using (var context = new QMTContext())
            {
                int num = 0;
                  if(context.Issues.Any())
                    num = context.Issues.Select(x => x.Id).Max();
                 Issue _issue = new Issue();
                _issue.Id = num+1;
                _issue.Title = issue.Title;
                _issue.CreatedBy = issue.CreatedBy;
                _issue.Type = issue.Type;
                _issue.Status = "Open";
                _issue.AdditionalRemarks = issue.AdditionalRemarks;
                _issue.CreatedOn = DateTime.Now;

                context.Issues.Add(_issue);
                context.SaveChanges();
                return _issue;
            }
        }
    }
}
