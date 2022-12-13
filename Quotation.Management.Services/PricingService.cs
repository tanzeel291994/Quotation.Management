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
    public class PricingService : IPricingService
    {
        private readonly IPricingRepository<PricingMaster> _pricingRepository;
        private readonly IItemCodeRepository<ItemMaster> _itemCodeRepository;
        private readonly IOptCodeRepository<OptionMaster> _optCodeRepository;
        private readonly IProductMasterRepository<ProductMaster> _productRepository;
        private readonly IItemGroupRepository<ItemGroupMaster> _itemGroupRepository;
        private readonly IBrandRepository<BrandMaster> _brandRepository;
        private readonly ISeriesRepository<SeriesMaster> _seriesRepository;
        private readonly IItemCodeRepository<ItemMaster> _itemRepository;
        private readonly ILogger<PricingService> _logger;
        public PricingService(ILogger<PricingService> logger, IItemCodeRepository<ItemMaster> itemRepository, ISeriesRepository<SeriesMaster> seriesRepository, IBrandRepository<BrandMaster> brandRepository, IItemGroupRepository<ItemGroupMaster> itemGroupRepository, IProductMasterRepository<ProductMaster> productRepository, IOptCodeRepository<OptionMaster> optCodeRepository, IItemCodeRepository<ItemMaster> itemCodeRepository, IPricingRepository<PricingMaster> pricingRepository)
        {
            _pricingRepository = pricingRepository ?? throw new ArgumentNullException(nameof(pricingRepository));
            _itemCodeRepository = itemCodeRepository ?? throw new ArgumentNullException(nameof(itemCodeRepository));
            _optCodeRepository = optCodeRepository ?? throw new ArgumentNullException(nameof(optCodeRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _itemGroupRepository = itemGroupRepository ?? throw new ArgumentNullException(nameof(itemGroupRepository));
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
            _seriesRepository = seriesRepository ?? throw new ArgumentNullException(nameof(seriesRepository));
            _itemRepository = itemRepository ?? throw new ArgumentNullException(nameof(itemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public List<PricingMaster> GetPricings()
        {
            return _pricingRepository.GetAll();
        }
        public PricingMaster InsertPricing(PricingMaster pricing)
        {
            return _pricingRepository.InsertPricing(pricing);
        }

        public List<string> ImportData_Old(DataSet ds)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("Pricing");
                List<PricingMaster> pricingList = new List<PricingMaster>();
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
                            validationMessages.Add("Item Code name missing on Index " + i);
                            continue;
                        }
                        if (dt.Rows[i][2] == null)
                        {
                            validationMessages.Add("Pricing missing on Index " + i);
                            continue;
                        }
                        string optCode = (string)dt.Rows[i][1];
                        string itemCodeName = (string)dt.Rows[i][0];
                        string pricing = Convert.ToString(dt.Rows[i][2]);
                        string version = "";

                        OptionMaster? optionMaster = _optCodeRepository.GetOptCode(optCode);
                        if (optionMaster == null)
                        {
                            validationMessages.Add("OptCode name doesnt exist  " + optCode + " on row index " + i);
                            continue;
                        }
                        ItemMaster? itemMasterExist = _itemCodeRepository.GetItemCode(itemCodeName);
                        if (itemMasterExist == null)
                        {
                            validationMessages.Add("Item Code name doesnot exist  in db of " + itemCodeName + " on row index " + i);
                            continue;
                        }
                        bool isPricingDataType =decimal.TryParse(pricing.Replace(",", ""), out decimal pricingValue);
                        if(!isPricingDataType)
                        {
                            validationMessages.Add("Pricing is not correct value on row index" + i);
                            continue;
                        }
                        PricingMaster? pricingMasterExist = _pricingRepository.GetPricing(itemCodeName, optCode);
                        if (pricingMasterExist != null && pricingMasterExist.Price != pricingValue)
                            version = "V" + (Convert.ToInt32(pricingMasterExist.Version.Replace("V", "")) + 1); // if pricing is differnt then only make versions
                        else if (pricingMasterExist!.Price == pricingValue)
                            continue;
                        else
                            version = "V1";

                        PricingMaster pricingMaster = new PricingMaster();
                        pricingMaster.ItemCode = itemCodeName;
                        pricingMaster.OptCode = optCode;
                        pricingMaster.Version = version;
                        pricingMaster.Price = pricingValue;
                        pricingList.Add(pricingMaster);

                    }
                    if (validationMessages.Count == 0)
                    {
                        bool result = _pricingRepository.MultipleInsertPricingData(pricingList);
                        if (!result) validationMessages.Add("Error in adding pricing data");
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

        public List<string> ImportData(DataSet ds)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("Masters");
                List<PricingMaster> pricingList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    QMTContext context = _productRepository.BeginTransaction();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string? productTypeId = dt.Rows[i].Field<string>("ProductTypeId");
                        string? productName = dt.Rows[i].Field<string>("ProductName");
                        string? itemGroupName = dt.Rows[i].Field<string>("ItemGroupName");
                        string? brandName = dt.Rows[i].Field<string>("BrandName");
                        string? currencyCode = dt.Rows[i].Field<string>("CurrencyCode");
                        string? seriesName = dt.Rows[i].Field<string>("SeriesName");
                        string? parentSeries = dt.Rows[i].Field<string>("ParentSeries");
                        string? model = dt.Rows[i].Field<string>("Model");

                        if(productTypeId == null)
                        {
                            validationMessages.Add("ProductTypeId missing on Index " + i);
                            continue;
                        }
                        if (productName == null)
                        {
                            validationMessages.Add("ProductName missing on Index " + i);
                            continue;
                        }
                        if (itemGroupName == null)
                        {
                            validationMessages.Add("ItemGroupName missing on Index " + i);
                            continue;
                        }
                        if (brandName == null)
                        {
                            validationMessages.Add("BrandName missing on Index " + i);
                            continue;
                        }
                        if (seriesName == null)
                        {
                            validationMessages.Add("SeriesName missing on Index " + i);
                            continue;
                        }
                        if (model == null)
                        {
                            validationMessages.Add("Model missing on Index " + i);
                            continue;
                        }

                        ProductMaster productMaster = new();
                        productMaster.ProdTypeId = productTypeId;
                        productMaster.ProdName = productName ?? productTypeId;

                        ItemGroupMaster itemGroup = new();
                        itemGroup.GroupName = itemGroupName;
                        itemGroup.ProdTypeId = productTypeId;

                        BrandMaster brand = new ();
                        brand.BrandName = brandName;
                        brand.CurrencyCode = currencyCode!;
                     
                        _productRepository.InsertProductIfNotExist(productMaster, context);
                        itemGroup = _itemGroupRepository.InsertItemGroupIfNotExist(itemGroup, context);
                        brand = _brandRepository.InsertBrandIfNotExist(brand, context);

                        SeriesMaster series = new ();
                        series.SeriesName = seriesName;
                        series.GroupId = itemGroup.GroupId;
                        series.BrandId = brand.BrandId;
                        series.ParentSeries = parentSeries;

                        series = _seriesRepository.InsertSeriesIfNotExist(series, context);

                        ItemMaster itemMaster = new();
                        itemMaster.SeriesId = series.SeriesId;
                        itemMaster.ItemCode = model;

                        itemMaster = _itemCodeRepository.InsertItemCodeIfNotExist(itemMaster,context);
                    }
                    if (validationMessages.Count == 0)
                        _productRepository.Commit();
                    else
                        _productRepository.RollBack();

                }
                return validationMessages;
            }
            catch (Exception ex)
            {
                _productRepository.RollBack();
                _logger.LogError(ex, ex.Message);
                validationMessages.Add("Error in saving :"+ex.Message);
                return validationMessages;
            }
            finally
            {
                _productRepository.DisposeConnection();
            }
        }


        public List<string> ImportPricingData(DataSet ds)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("Pricing");
                List<PricingMaster> pricingList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    string[] columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                    List<string> itemCodes = new();
                    foreach (var columnName in columnNames)
                        if (columnName != "OptCode" && columnName != "OptName")
                            itemCodes.Add(columnName);
                    List<string> messages= _itemCodeRepository.ValidateAllItemCodes(itemCodes);
                    if (messages.Count > 0) return messages;

                    QMTContext context = _optCodeRepository.BeginTransaction();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string? optCode = dt.Rows[i].Field<string>("OptCode");
                        string? optName = dt.Rows[i].Field<string>("OptName");
                        if (optCode == null)
                        {
                            validationMessages.Add("OptionCode missing on Index " + i);
                            continue;
                        }
                        foreach(var itemCode in itemCodes)
                        {
                            string? pricing = Convert.ToString(dt.Rows[i].Field<object>(itemCode));
                            if (pricing == null || pricing == "")
                            {
                                //validationMessages.Add("Pricing missing on Row Index " + i);
                                continue;
                            }

                            string version = "";
                            bool isPricingDataType = decimal.TryParse(pricing.Replace(",", "."), out decimal pricingValue);
                            if (!isPricingDataType)
                            {
                                validationMessages.Add("Pricing is not number on row index" + i);
                                continue;
                            }
                            PricingMaster? pricingMasterExist = _pricingRepository.GetPricing(itemCode, optCode);
                            if (pricingMasterExist != null && pricingMasterExist.Price != pricingValue)
                                version = "V" + (Convert.ToInt32(pricingMasterExist.Version.Replace("V", "")) + 1); // if pricing is differnt then only make versions
                            else if (pricingMasterExist != null && pricingMasterExist!.Price == pricingValue)
                                continue;
                            else
                                version = "V1";

                            OptionMaster optionMaster = new();
                            optionMaster.OptCode = optCode;
                            optionMaster.OptName =optName ?? optCode;

                            optionMaster = _optCodeRepository.InsertOptCodeIfNotExist(optionMaster,context);

                            PricingMaster pricingMaster = new PricingMaster();
                            pricingMaster.ItemCode = itemCode;
                            pricingMaster.OptCode = optionMaster.OptCode;
                            pricingMaster.Version = version;
                            pricingMaster.Price = pricingValue;

                            pricingMaster = _pricingRepository.InsertPricingIfNotExist(pricingMaster, context);


                        }
                    }
                    if (validationMessages.Count == 0)
                        _optCodeRepository.Commit();
                    else
                        _optCodeRepository.RollBack();

                }
                return validationMessages;
            }
            catch (Exception ex)
            {
                _optCodeRepository.RollBack();
                _logger.LogError(ex, ex.Message);
                validationMessages.Add("Error in saving :" + ex.Message);
                return validationMessages;
            }
            finally
            {
                _optCodeRepository.DisposeConnection();
            }
        }

    }
}
