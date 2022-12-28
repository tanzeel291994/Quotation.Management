using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    [Serializable]
    public class ValidationException : Exception
    {
        public List<string> _messages;
        public ValidationException(List<string> messages) 
        {
            _messages = messages;
        }
    }
}
