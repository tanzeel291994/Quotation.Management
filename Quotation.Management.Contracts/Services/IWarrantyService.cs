using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IWarrantyService
    {
        WarrantyHeader? InsertWarrantyHeader(WarrantyHeaderDC inputHeader);
        void InsertWarrantyLine(WarrantyLineDC inputLine);
        dynamic? GetWarranty(string Id);
        dynamic SearchQuotations(WarrantySearchDC warrantySearch);
        List<string> GetAllJobRefs();
        List<string> ImportWarrantyData(DataSet ds, int userId);
    }
}
