using Microsoft.Extensions.Logging;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class OptCodeService : IOptCodeService
    {
        private readonly IOptCodeRepository<OptionMaster> _optCodeRepository;
        private readonly ILogger<OptCodeService> _logger;
        public OptCodeService(ILogger<OptCodeService> logger, IOptCodeRepository<OptionMaster> optCodeRepository)
        {
            _optCodeRepository = optCodeRepository ?? throw new ArgumentNullException(nameof(optCodeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public List<OptionMaster> GetOptCodes()
        {
            return _optCodeRepository.GetAll();
        }
        public OptionMaster InsertOptCode(OptionMaster optCode)
        {
            return _optCodeRepository.InsertOptCode(optCode);
        }

        public List<string> ImportData(DataSet ds)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("OptionMaster");
                List<OptionMaster> optCodeList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][1] == null || (string)dt.Rows[i][1] == "")
                        {
                            validationMessages.Add("OptCode name missing on Index " + i);
                            continue;
                        }
                        if (dt.Rows[i][0] == null || (string)dt.Rows[i][0] == "")
                        {
                            validationMessages.Add("OptCode name missing on Index " + i);
                            continue;
                        }

                        string optCodeName = (string)dt.Rows[i][1];
                        string optCode = (string)dt.Rows[i][0];

                        OptionMaster? optionDoesExist = _optCodeRepository.GetOptCode(optCode);
                        if (optionDoesExist != null)
                        {
                            validationMessages.Add("OptCode name exist  " + optCode + " on row index " + i);
                            continue;
                        }

                        OptionMaster optCodeMaster = new OptionMaster();
                        optCodeMaster.OptCode = optCode;
                        optCodeMaster.OptName = optCodeName;
                        optCodeList.Add(optCodeMaster);

                    }
                    if (validationMessages.Count == 0)
                    {
                        bool result = _optCodeRepository.MultipleInsertOptCodes(optCodeList);
                        if (!result) validationMessages.Add("Error in adding option data");
                    }

                }
                return validationMessages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return validationMessages;
            }
        }

    }
}
