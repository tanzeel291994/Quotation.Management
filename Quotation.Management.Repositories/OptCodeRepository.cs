using Microsoft.Extensions.Logging;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class OptCodeRepository : BaseRepository<OptionMaster>, IOptCodeRepository<OptionMaster>
    {
        #region variables
        private readonly ILogger<OptCodeRepository> _logger;
        #endregion
        public OptCodeRepository(ILogger<OptCodeRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public  List<OptionMaster> GetAll ()
        {

            using (var context = new QMTContext())
            {
               return  context.OptionMasters.ToList();
            }
        }

        public OptionMaster? GetOptCode(string optName)
        {

            using (var context = new QMTContext())
            {
                return context.OptionMasters.Where(x => x.OptName == optName.ToUpper()).FirstOrDefault();
            }
        }

        public OptionMaster InsertOrUpdateOptCodeIfNotExist(OptionMaster _optCodeMaster, QMTContext? _context = null)
        {
            //using (var context = _context  ?? new QMTContext())
            //{
            var context = _context ?? new QMTContext();
            OptionMaster? optionMaster = context.OptionMasters.Where(x => x.OptCode == _optCodeMaster.OptCode).FirstOrDefault();
            if (optionMaster == null)
            {
                context.OptionMasters.Add(_optCodeMaster);
                context.SaveChanges();
                return _optCodeMaster;
            }
            else
            {
                if (optionMaster.OptName != _optCodeMaster.OptName)
                {
                    optionMaster.OptName = _optCodeMaster.OptName;
                    context.SaveChanges();
                }
                return optionMaster;
            }
            
            //}
        }

        public OptionMaster InsertOptCode(OptionMaster optCode)
        {

            using (var context = new QMTContext())
            {
                OptionMaster _optCode = new OptionMaster();
                _optCode.OptName = optCode.OptName;
                _optCode.OptCode = optCode.OptCode;
                context.OptionMasters.Add(_optCode);
                context.SaveChanges();
                return _optCode;
            }
        }

        public bool MultipleInsertOptCodes(List<OptionMaster> optCodeList)
        {
            try
            {
                using (var context = new QMTContext())
                {
                    using (var dbContextTransaction = context.Database.BeginTransaction())
                    {
                        foreach (var optCode in optCodeList)
                            context.OptionMasters.Add(optCode);

                        context.SaveChanges();
                        dbContextTransaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }
    }
}
