using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Services
{
    public interface IWordDocumentService
    {
        byte[] CreateWordDocument(dynamic headerData, TableData data, decimal totalAmout);
    }
}
