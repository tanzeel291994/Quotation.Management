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

        public ItemCodeDetailsDC? GetItemCodeDetails(string itemCode)
        {
            return _itemCodeRepository.GetItemCodeDetails(new List<string>() { itemCode }).FirstOrDefault();
        }

        public string? CreateItemCode(string baseItemCode, string option)
        {
            string[] optionSplit = option.Split("-");
            if(optionSplit.Length == 2)
            {
                int index = Convert.ToInt32(optionSplit[0])-1;
                string opt = optionSplit[1];

                char[] itemCodeArray = baseItemCode.ToArray();
               

                if (opt.Length == 2)
                {
                    if (itemCodeArray.Length  < index || itemCodeArray.Length - 1  < index+1)
                    {
                        return null;
                    }
                    itemCodeArray[index] = opt[0];
                    itemCodeArray[index + 1] = opt[1];
                   
                    return  new string(itemCodeArray);
                }
                else if (opt.Length == 1)
                {
                    if (itemCodeArray.Length  < index)
                    {
                        return null;
                    }
                    itemCodeArray[index] = opt[0];
                    return new string(itemCodeArray);
                }
                else
                    return null;
            }
            else
                return null;
        }


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
