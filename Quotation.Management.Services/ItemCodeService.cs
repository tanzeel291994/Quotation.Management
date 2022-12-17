using Microsoft.Extensions.Logging;
using Quotation.Management.Contracts;
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
    public class ItemCodeService : IItemCodeService
    {
        private readonly IItemCodeRepository<ItemMaster> _itemCodeRepository;
        private readonly ISeriesRepository<SeriesMaster> _seriesRepository;
        private readonly ILogger<ItemCodeService> _logger;

        public ItemCodeService(ILogger<ItemCodeService> logger, IItemCodeRepository<ItemMaster> itemCodeRepository, ISeriesRepository<SeriesMaster> seriesRepository)
        {
            _itemCodeRepository = itemCodeRepository ?? throw new ArgumentNullException(nameof(itemCodeRepository));
            _seriesRepository = seriesRepository ?? throw new ArgumentNullException(nameof(seriesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public dynamic GetItemCodes()
        {
            return _itemCodeRepository.GetAll();
        }

        public List<MasterDC> GetItemCodes(string searchString)
        {
            return _itemCodeRepository.GetItemCodes(searchString);
        }
        public ItemMaster? InsertItemCode(ItemMaster itemCode)
        {
            return _itemCodeRepository.InsertItemCode(itemCode);
        }

        //private string CreateItemCode(string baseItemCode , List<string> options)
        //{

        //}


        public List<string> ImportData(DataSet ds)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("ItemMasters");
                List<ItemMaster> itemCodes = new List<ItemMaster>();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i][2] == null || (string)dt.Rows[i][2] == "")
                        {
                            validationMessages.Add("Series name missing on Index " + i);
                            continue;
                        }
                        if (dt.Rows[i][0] == null || (string)dt.Rows[i][0] == "")
                        {
                            validationMessages.Add("Item Code name missing on Index " + i);
                            continue;
                        }
                        string seriesName = (string)dt.Rows[i][2];
                        string itemCodeName = (string)dt.Rows[i][0];
                        if (!itemCodeName.StartsWith(seriesName))
                        {
                            validationMessages.Add("Invalid item code name on index" + i);
                        }
                        SeriesMaster? seriesMaster = _seriesRepository.GetSeries(seriesName);
                        if (seriesMaster == null)
                        {
                            validationMessages.Add("Series name doesnt exist  " + seriesName + " on row index " + i);
                            continue;
                        }
                        ItemMaster? itemMasterExist = _itemCodeRepository.GetItemCode(itemCodeName);
                        if (itemMasterExist != null)
                        {
                            validationMessages.Add("Item Code name does exist  in db of " + itemCodeName + " on row index " + i);
                            continue;
                        }
                        ItemMaster itemMaster = new ItemMaster();
                        itemMaster.SeriesId = seriesMaster.SeriesId;
                        itemMaster.ItemCode = itemCodeName;
                        itemCodes.Add(itemMaster);

                    }
                    if(validationMessages.Count == 0)
                    {
                       bool result =  _itemCodeRepository.MultipleInsertItemCode(itemCodes);
                        if (!result) validationMessages.Add("Error in adding item codes");
                    }

                }
                return validationMessages;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return validationMessages;
            }
        }


    }
}
