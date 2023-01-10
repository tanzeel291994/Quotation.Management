using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IMastersRepository _mastersRepository;
        private readonly IItemCodeService _itemCodeService;
        private readonly IProductMasterRepository<ProductMaster> _productMasterRepository;
        private readonly IItemCodeRepository<ItemMaster> _itemCodeRepository;
        private readonly ILogger<QuotationService> _logger;
        public QuotationService(IItemCodeRepository<ItemMaster> itemCodeRepository, IItemCodeService itemCodeService, IMastersRepository mastersRepository, IProductMasterRepository<ProductMaster> productMasterRepository, IQuotationRepository quotationRepository, ILogger<QuotationService> logger)
        {
            _quotationRepository = quotationRepository ?? throw new ArgumentNullException(nameof(quotationRepository));
            _mastersRepository = mastersRepository ?? throw new ArgumentNullException(nameof(mastersRepository));
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
            _itemCodeRepository = itemCodeRepository ?? throw new ArgumentNullException(nameof(itemCodeRepository));
            _itemCodeService = itemCodeService ?? throw new ArgumentNullException(nameof(itemCodeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Header
        public QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                QuotationHeader header = new QuotationHeader();
                header.RevNum = 0;    // need to add revnum
                header.QuotationDate = DateTime.Now;
                header.AreaCode = inputHeader.AreaCode;
                header.CurrencyCode = inputHeader.CurrencyCode;
                header.CustomerCode = inputHeader.CustomerCode;
                header.DeliveryTermId = inputHeader.DeliveryTermId;
                header.ExpectedDeliveryDate = inputHeader.ExpectedDeliveryDate;
                header.BookingDate = inputHeader.BookingDate;
                header.Msp = inputHeader.Msp;
                header.PaymentTermId = inputHeader.PaymentTermId;
                header.Probability = inputHeader.Probability;
                header.StatusId = inputHeader.StatusId;
                header.ProjectName = inputHeader.ProjectName;
                header.IsActiveRevision = true;
                header.Asp = inputHeader.Asp;
                header.IndustryId = inputHeader.IndustryId;
                header.CreatedBy = inputHeader.UserId;
                header.QuotationNum = inputHeader.QuotationNum ?? GenerateQuotionNum(header.AreaCode, header.Msp);

                header = _quotationRepository.InsertUpdateQuotation(header, inputHeader.UserId);
                return header;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public JObject? GetQuotation(string Id, int? revNum = null)
        {
            JObject jobject = new();
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id);
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLinesDC(Id, header!.RevNum);
                dynamic products = _productMasterRepository.GetProductsofQuotations(Id, header!.RevNum);
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(Id, header!.RevNum);
                jobject.Add(new JProperty("header", JsonConvert.SerializeObject(header!, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })));
                //jobject.Add(new JProperty("lines", JsonConvert.SerializeObject(lines, new JsonSerializerSettings
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //})));
                //jobject.Add(new JProperty("products", JsonConvert.SerializeObject(products)));

                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public dynamic GetProductsFromQuotation(string Id, int revNum)
        {
            try
            {
                dynamic products = _productMasterRepository.GetProductsofQuotations(Id, revNum);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public dynamic SearchQuotations(QuotationSearchDC quotationSearch)
        {
            JObject jobject = new();
            try
            {
                dynamic result;
                if(!quotationSearch.ItemCodeWise)
                {
                     result = _quotationRepository.GetQuotationSearch(quotationSearch);
                }
                else
                {
                    result = _quotationRepository.GetQuotationLinesSearch(quotationSearch);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }


        public List<QuotationLineDC> GetQuotationLines(string Id, int revNum)
        {
            JObject jobject = new();
            try
            {
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLinesDC(Id, revNum);
                List<string> itemCodes = lines.Select(x => x.ItemCode).Distinct().ToList();
                List<ItemCodeDetailsDC> itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(itemCodes,new QMTContext());
                foreach(var _line in lines)
                {
                    ItemCodeDetailsDC itemCodeDetail = itemCodeDetails.Where(x => x.ItemCode == _line.ItemCode).FirstOrDefault();
                    _line.CAF = itemCodeDetail != null ? itemCodeDetail.CAF : 1;
                    _line.IndexValue = itemCodeDetail != null ? itemCodeDetail.IndexConvFactor : 1;
                }

                return lines;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public bool UpdateQuotationCurrency(CurrencyDC currencyDC)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(currencyDC.QuotationNum, currencyDC.RevNum, context);
                quotationHeader!.CurrencyCode = currencyDC.CurrencyCode;
                if (currencyDC.NewConvFactor != null)
                    quotationHeader!.ConvFactor = currencyDC.NewConvFactor;

                quotationHeader = _quotationRepository.UpdateQuotationHeader(quotationHeader, context);
                List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(currencyDC.QuotationNum, currencyDC.RevNum, _context: context);

                List<QuotationOptCode> optCodeList = _quotationRepository.GetQuotationOptCodes(currencyDC.QuotationNum, currencyDC.RevNum, context);

                foreach (var _optCode in optCodeList)
                {
                    _optCode.UnitPrice = (decimal)(currencyDC.NewConvFactor ?? currencyDC.ConvFactor) * _optCode.UnitPrice;
                    _quotationRepository.UpdateQuotationOptCodes(_optCode, context);
                }

                UpdateUnitPriceFromOptions(currencyDC.QuotationNum, currencyDC.RevNum, quotationLines.Select(x => x.LineNum).ToList(), context);
                
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(currencyDC.QuotationNum, currencyDC.RevNum, context);
                foreach (var _costItem in costItems)
                {
                    if (_costItem.CostItemType == CostItemType.ByVal.ToString())
                    {
                        _costItem.CostItemValue = _costItem.CostItemValue * (currencyDC.NewConvFactor ?? currencyDC.ConvFactor);
                        _quotationRepository.UpdateCostItem(_costItem, context);
                    }
                }

                UpdateAllLinesCostItemValue(currencyDC.QuotationNum, currencyDC.RevNum, context);

                _quotationRepository.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        #endregion

        #region Lines
        public QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                ItemCodeDetailsDC itemDetails = _itemCodeRepository.GetItemCodeDetails(new List<string> { inputLine.ItemCode }, context).First();
                QuotationLine line = new();
                QuotationLine? latestLine = _quotationRepository.GetLatestQuotationLine(inputLine.QuotationNum);
                decimal costItemValue = 0;
                line.QuotationNum = inputLine.QuotationNum;
                line.ActiveLine = true; // BY DEFAULT ALL LINES ARE ACTIVE WHEN INSERTED
                line.Qty = inputLine.Qty;
                line.Mtlp = itemDetails.Mtlp ?? inputLine.Mtlp;
                line.UnitPrice = inputLine.UnitPrice;
                line.ItemCode = inputLine.ItemCode;
                line.Vat = inputLine.Vat;
                line.Margin = inputLine.Margin;
                line.UnitTag = inputLine.UnitTag;
                line.LineNum = latestLine != null ? latestLine.LineNum + 1 : 1;
                line.RevNum = inputLine.RevNum;
                line.UnitPrice = 0;
                line.TtNetPrice = 0;
                if (itemDetails.ProdTypeId == "AHU")
                {
                    if (line.UnitTag == null || line.UnitTag == "")
                        throw new ValidationException(new List<string> { "UnitTag cannot be empty for AHUs" + inputLine.ItemCode });
                    line.SubItemCode = _quotationRepository.GenerateItemCode(inputLine, context);
                    line = _quotationRepository.InsertQuotationLine(line, context);
                    inputLine.ItemCode = line.SubItemCode;
                }
                else
                {
                    List<PricingMasterDC> pricing = _quotationRepository.GetPricingOptCode(inputLine.ItemCode, new List<string>() { "BASIC" });

                    if (pricing.Count == 0)
                        throw new ValidationException(new List<string> { "BASIC option not present for ItemCode:" + inputLine.ItemCode });

                    QuotationHeader? header = _quotationRepository.GetQuotation(inputLine.QuotationNum, inputLine.RevNum);

                    string currencyCode = header!.CurrencyCode;
                    CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);

                    line.UnitPrice = currencyCode == itemDetails.CurrencyCode ? pricing.First().Price : CalculatePriceOnCurrency(quotationCurrency!, pricing.First(), itemDetails);
                    if (itemDetails.IndexConvFactor != null) line.UnitPrice = line.UnitPrice * itemDetails.IndexConvFactor!.Value;
                    
                    line.TtNetPrice = line.UnitPrice * line.Mtlp * line.Qty; // BE DEFAULT NO NET IS TAKEN ON LINE INSERT
                    line = _quotationRepository.InsertQuotationLine(line, context);

                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = line.RevNum;
                    optCode.LineNum = line.LineNum;
                    optCode.UnitPrice = line.UnitPrice;
                    optCode.OptCode = pricing[0].OptCode;
                    optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                    List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);

                    costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                }

                inputLine.LineNum = line.LineNum;
                inputLine.UnitPrice = line.UnitPrice;
                inputLine.TtNetPrice = line.TtNetPrice;
                inputLine.Margin = line.Margin;
                inputLine.ActiveLine = line.ActiveLine;
                inputLine.CAF = itemDetails.CAF;
                inputLine.IndexValue = itemDetails.IndexConvFactor;
                inputLine.UnitTag = line.UnitTag ?? "";
                inputLine.CostItemLineValue = costItemValue;
                inputLine.TtslsPriceWOVat = Math.Round((inputLine.TtNetPrice) + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtslsPriceWMargin = CalculateMarginValue(line.Margin, inputLine.TtslsPriceWOVat);
                inputLine.TtslsPrice = CalculateTotalValue(inputLine);

                _quotationRepository.Commit();
                return inputLine;
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                _quotationRepository.UpdateQuotationLine(inputLine, context);

                List<QuotationLineDC> linesDC = UpdateUnitPriceFromOptions(inputLine.QuotationNum, inputLine.RevNum, new List<int> { inputLine.LineNum }, context);
                inputLine = linesDC.Where(x => x.LineNum == inputLine.LineNum).First();

                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                decimal costItemValue = lines.Where(x => x.LineNum == inputLine.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.CostItemLineValue = Math.Round(costItemValue,2);
                
                inputLine.TtslsPriceWOVat = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0),2);
                inputLine.TtslsPriceWMargin =  CalculateMarginValue(inputLine.Margin, inputLine.TtslsPriceWOVat);
                inputLine.TtslsPrice = CalculateTotalValue(inputLine);

                _quotationRepository.Commit();
                return inputLine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        #endregion

        #region Options
        public bool CopyOptionLine(QuotationCopyOptionDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                List<string> copyOptions = input.FromOptCodes.Distinct().ToList();
                List<QuotationLineDC> copyToLines = _quotationRepository.GetQuotationLinesDC(input.QuotationNum, input.RevNum, selectedLines: input.CopyToLines);
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(input.QuotationNum, input.RevNum);
                string currencyCode = quotationHeader!.CurrencyCode;
                CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                List<PricingMasterDC> pricingList = new List<PricingMasterDC>();
                List<string> itemCodes = copyToLines.Select(x => x.ItemCode).Distinct().ToList();
                List<ItemCodeDetailsDC> itemCodesDetails = _itemCodeRepository.GetItemCodeDetails(itemCodes, context);
                foreach (var line in copyToLines)
                {
                    List<PricingMasterDC> pricingMasters = _quotationRepository.GetPricingOptCode(line.ItemCode, copyOptions);
                    ItemCodeDetailsDC itemCodeDetail = itemCodesDetails.Where(x => x.ItemCode == line.ItemCode).First();
                    if (pricingMasters.Count > 0)
                    {
                        foreach (var pricing in pricingMasters)
                        {
                            QuotationOptCode optCode = new();
                            optCode.QuotationNum = input.QuotationNum;
                            optCode.RevNum = line.RevNum;
                            optCode.LineNum = line.LineNum;
                            optCode.UnitPrice = currencyCode == itemCodeDetail.CurrencyCode ? pricing.Price : CalculatePriceOnCurrency(quotationCurrency!, pricing, itemCodeDetail);
                            if (itemCodeDetail.IndexConvFactor != null) optCode.UnitPrice = optCode.UnitPrice * itemCodeDetail.IndexConvFactor.Value;
                            
                            optCode.OptCode = pricing.OptCode;
                            optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                        }
                        pricingList.AddRange(pricingMasters);
                    }
                }

                UpdateUnitPriceFromOptions(input.QuotationNum, input.RevNum, input.CopyToLines, context, pricingList);
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);

                _quotationRepository.Commit();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                return false;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationLineDC? InsertQuotationOptions(QuotationLineDC inputLine)
        {
            try
            {

                QMTContext context = _quotationRepository.BeginTransaction();
                List<string> optCodes = inputLine.optCodes!.Split(',').ToList();
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(inputLine.QuotationNum,inputLine.RevNum);
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(inputLine.QuotationNum, inputLine.LineNum, inputLine.RevNum);
                List<PricingMasterDC> pricingList = _quotationRepository.GetPricingOptCode(quotationLine.ItemCode, optCodes);
                string currencyCode = quotationHeader!.CurrencyCode;
                CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                ItemCodeDetailsDC itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(new List<string> { inputLine.ItemCode }, context).First();
                foreach (var item in pricingList)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = inputLine.RevNum;
                    optCode.LineNum = inputLine.LineNum;
                    optCode.UnitPrice = currencyCode == itemCodeDetails.CurrencyCode ? item.Price : CalculatePriceOnCurrency(quotationCurrency!, item, itemCodeDetails);
                    if (itemCodeDetails.IndexConvFactor != null) optCode.UnitPrice = optCode.UnitPrice * itemCodeDetails.IndexConvFactor.Value;

                    optCode.OptCode = item.OptCode;
                    optCode.IsNet = item.IsNet;
                    optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                }

                List<QuotationLineDC> linesDC = UpdateUnitPriceFromOptions(inputLine.QuotationNum, inputLine.RevNum, new List<int> { inputLine.LineNum }, context, pricingList);
                QuotationLineDC lineDC = linesDC.Where(x => x.LineNum == inputLine.LineNum).First();
                
                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                inputLine.CostItemLineValue = lines.Where(x => x.LineNum == inputLine.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.UnitPrice = lineDC.UnitPrice;
                inputLine.TtNetPrice = linesDC.First().TtNetPrice;
                inputLine.TtslsPriceWOVat = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtslsPriceWMargin =  CalculateMarginValue(inputLine.Margin, inputLine.TtslsPriceWOVat);
                inputLine.TtslsPrice = Math.Round(CalculateTotalValue(lineDC),2);
                

               _quotationRepository.Commit();
                return inputLine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationLineDC? RemoveQuotationOptions(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                List<string> optCodes = inputLine.optCodes!.Split(',').ToList();
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(inputLine.QuotationNum, inputLine.LineNum, inputLine.RevNum);
                List<PricingMasterDC> pricingList = _quotationRepository.GetPricingOptCode(quotationLine.ItemCode, optCodes);

                foreach (var item in pricingList)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = inputLine.RevNum;
                    optCode.LineNum = inputLine.LineNum;
                    //optCode.UnitPrice = CalculatePriceOnCurrency(currencyCode, item)
                    optCode.OptCode = item.OptCode;
                    _quotationRepository.RemoveQuotationOptCode(optCode, context);
                }
                List<QuotationLineDC> linesDC = UpdateUnitPriceFromOptions(inputLine.QuotationNum, inputLine.RevNum, new List<int> { inputLine.LineNum }, context, pricingList);
                QuotationLineDC lineDC = linesDC.Where(x => x.LineNum == inputLine.LineNum).First();
                //UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                inputLine.CostItemLineValue = lines.Where(x => x.LineNum == inputLine.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.UnitPrice = lineDC.UnitPrice;
                               
                inputLine.TtNetPrice = linesDC.First().TtNetPrice;
                inputLine.TtslsPriceWOVat = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtslsPriceWMargin = CalculateMarginValue(inputLine.Margin, inputLine.TtslsPriceWOVat);
                inputLine.TtslsPrice = Math.Round(CalculateTotalValue(lineDC), 2); // with VAT

                _quotationRepository.Commit();
                return inputLine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }

        public QuotationNonStandardOptCodeDC? InsertNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();

                var optCodeExists= _quotationRepository.GetQuotationOptCode(optCodeDC.QuotationNum, optCodeDC.RevNum,optCodeDC.LineNum,optCodeDC.OptCode,context);
                if(optCodeExists != null)
                {
                    throw new ValidationException(new List<string> {"Opt code:"+ optCodeDC.OptCode+" already exist for the line"});
                }
                QuotationOptCode optCode = new();
                optCode.QuotationNum = optCodeDC.QuotationNum;
                optCode.RevNum = optCodeDC.RevNum;
                optCode.LineNum = optCodeDC.LineNum;
                optCode.UnitPrice = optCodeDC.Price;
                optCode.OptCode = optCodeDC.OptCode;
                optCode.OptName = optCodeDC.OptName;
                optCode.OptType = OptionType.NonStandard.ToString();
                optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                

                UpdateUnitPriceFromOptions(optCodeDC.QuotationNum, optCodeDC.RevNum, new List<int> { optCodeDC.LineNum }, context);
                UpdateAllLinesCostItemValue(optCodeDC.QuotationNum, optCodeDC.RevNum, context);

                _quotationRepository.Commit();
                return optCodeDC;
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationNonStandardOptCodeDC? RemoveNonStandardOption(QuotationNonStandardOptCodeDC optCodeDC)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationOptCode optCode = new();
                optCode.QuotationNum = optCodeDC.QuotationNum;
                optCode.RevNum = optCodeDC.RevNum;
                optCode.LineNum = optCodeDC.LineNum;
                optCode.OptCode = optCodeDC.OptCode;
                _quotationRepository.RemoveQuotationOptCode(optCode, context);

                UpdateUnitPriceFromOptions(optCodeDC.QuotationNum, optCodeDC.RevNum, new List<int> { optCodeDC.LineNum }, context);
                UpdateAllLinesCostItemValue(optCodeDC.QuotationNum, optCodeDC.RevNum, context);

                _quotationRepository.Commit();
                return optCodeDC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }

        public List<QuotationOptCodeDC>? GetQuotationLinesNonStandardOptCodes(string Id, int revNum,int lineNum)
        {
            try
            {
                //QuotationHeader? header = _quotationRepository.GetQuotation(Id, revNum);
                var quotationOptCodes = _quotationRepository.GetQuotationLinesNonStandardOptions(Id, revNum, lineNum);
                return quotationOptCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        public List<QuotationOptCodeDC>? GetQuotationLinesOptCodes(string Id, int revNum)
        {
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id, revNum);
                var quotationOptCodes = _quotationRepository.GetQuotationLinesOptions(Id, header!.RevNum);
                return quotationOptCodes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }
        #endregion

        #region CostItems
        public List<QuotationCostItemDC> GetQuotationCostLines(string quotationNum, int revNum)
        {
            try
            {
                List<QuotationCostItemDC> quotationCostItems = _quotationRepository.GetQuotationCostLines(quotationNum, revNum);
                return quotationCostItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC)
        {
            JObject jobject = new();
            try
            {
                List<QuotationOptCode> optCodeList = _quotationRepository.GetQuotationOptCodes(quotationLineDC.QuotationNum, quotationLineDC.RevNum,null ,quotationLineDC.LineNum);
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(quotationLineDC.QuotationNum, quotationLineDC.LineNum, quotationLineDC.RevNum);
                jobject.Add("selectedOptons", JsonConvert.SerializeObject(optCodeList));
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(quotationLineDC.QuotationNum, quotationLineDC.RevNum);
                CurrencyMaster? brandCurrency = _itemCodeRepository.GetItemCodeCurrency(quotationLine.ItemCode);
                decimal oldConvFactor = brandCurrency!.ConvFactor;
                decimal newConvFactor = 0;

                //if (quotationHeader!.ConvFactor == null)
                //{
                  CurrencyMaster? currencyMaster =  _mastersRepository.GetCurrencyByCode(quotationHeader.CurrencyCode);
                  newConvFactor = currencyMaster!.ConvFactor;
                //}
                jobject.Add("allOptions", JsonConvert.SerializeObject(_quotationRepository.GetItemOptions(quotationLine.ItemCode, newConvFactor/ oldConvFactor)));

                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public QuotationCostItemDC InsertQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem costItem = new();
                costItem.QuotationNum = input.QuotationNum;
                costItem.RevNum = input.RevNum;
                costItem.CostItemType = input.CostItemType;
                costItem.CostItemId = input.CostItemId;                
                costItem.ProdTypeId = input.ProdTypeId;
                costItem.FreightRate = input.FreightRate;
                costItem.NoOfContainers = input.NoOfContainers;
                if (input.FreightRate != null && input.NoOfContainers != null)
                    costItem.CostItemValue = Math.Round(input.FreightRate.Value * input.NoOfContainers.Value,2);
                else
                    costItem.CostItemValue = input.CostItemValue;             
                costItem.QuotationCostItemGroupId = Guid.NewGuid().ToString();
                List<int> lineNums = input.quotationLineCostItems.Select(x => x.LineNum).ToList();                                                                                                                           
                List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(input.QuotationNum, input.RevNum, selectedLines: lineNums);
                decimal ttslsPrice = 0;
                List<QuotationCostItemLine> costItemLines = new();
                foreach (var _quotationLine in quotationLines)
                {
                    ttslsPrice += (_quotationLine.UnitPrice * _quotationLine.Mtlp * _quotationLine.Qty);
                }
                foreach (var _line in input.quotationLineCostItems)
                {
                    QuotationLineDC quotationLine = quotationLines.Where(x => x.LineNum == _line.LineNum).First();
                    QuotationCostItemLine costItemLine = new();
                    costItemLine.QuotationNum = costItem.QuotationNum;
                    costItemLine.RevNum = costItem.RevNum;
                    costItemLine.LineNum = _line.LineNum;
                    costItemLine.QuotationCostItemGroupId = costItem.QuotationCostItemGroupId;
                    if (costItem.CostItemType == CostItemType.ByVal.ToString())
                    {
                        costItemLine.CostItemLineValue = costItem.CostItemValue * ((quotationLine.UnitPrice * quotationLine.Mtlp * quotationLine.Qty) /ttslsPrice);
                    }
                    if (costItem.CostItemType == CostItemType.ByPercentage.ToString())
                    {
                        costItemLine.CostItemLineValue = (costItem.CostItemValue / 100 * ttslsPrice);
                    }
                    costItemLines.Add(costItemLine);

                }
                costItem = _quotationRepository.InsertQuotationCostItemLine(costItem, context);
                costItemLines = _quotationRepository.InUpdDelQuotationCostItemLines(input.QuotationNum, input.RevNum, input.QuotationCostItemGroupId, costItemLines, context);
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                
                _quotationRepository.Commit();
                return input;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationCostItemDC UpdateQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem costItem = _quotationRepository.GetQuotationCostItem(input.QuotationNum, input.RevNum,input.QuotationCostItemGroupId, context);
                costItem.CostItemValue = input.CostItemValue;
                costItem.CostItemType = input.CostItemType;
                costItem.CostItemId = input.CostItemId;
                costItem.FreightRate = input.FreightRate;
                costItem.NoOfContainers = input.NoOfContainers;
                costItem.QuotationCostItemGroupId = input.QuotationCostItemGroupId;
                if (input.FreightRate != null && input.NoOfContainers != null)
                    costItem.CostItemValue = Math.Round(input.FreightRate.Value * input.NoOfContainers.Value, 2);
                else
                    costItem.CostItemValue = input.CostItemValue;
                costItem = _quotationRepository.UpdateCostItem(costItem, context);

                List<int> lineNums = input.quotationLineCostItems!.Select(x => x.LineNum).ToList();
                List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(input.QuotationNum, input.RevNum, selectedLines: lineNums);
                decimal ttslsPrice = 0;
                List<QuotationCostItemLine> costItemLines = new();
                foreach (var _quotationLine in quotationLines)
                {
                    ttslsPrice += (_quotationLine.UnitPrice * _quotationLine.Mtlp * _quotationLine.Qty);
                }
                foreach (var _line in input.quotationLineCostItems)
                {
                    QuotationLineDC quotationLine = quotationLines.Where(x => x.LineNum == _line.LineNum).First();
                    QuotationCostItemLine costItemLine = new();
                    costItemLine.QuotationNum = costItem.QuotationNum;
                    costItemLine.RevNum = costItem.RevNum;
                    costItemLine.LineNum = _line.LineNum;
                    costItemLine.QuotationCostItemGroupId = costItem.QuotationCostItemGroupId;
                    if (costItem.CostItemType == CostItemType.ByVal.ToString())
                    {
                        costItemLine.CostItemLineValue = costItem.CostItemValue * ((quotationLine.UnitPrice * quotationLine.Mtlp * quotationLine.Qty) / ttslsPrice);
                    }
                    if (costItem.CostItemType == CostItemType.ByPercentage.ToString())
                    {
                        costItemLine.CostItemLineValue = (costItem.CostItemValue / 100 * ttslsPrice);
                    }
                    costItemLines.Add(costItemLine);

                }

                costItemLines = _quotationRepository.InUpdDelQuotationCostItemLines(input.QuotationNum,input.RevNum,input.QuotationCostItemGroupId,costItemLines, context);

                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
                return input;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
        public QuotationCostItemDC DeleteQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem costItem = _quotationRepository.GetQuotationCostItem(input.QuotationNum, input.RevNum,input.QuotationCostItemGroupId, context);
                _quotationRepository.InUpdDelQuotationCostItemLines(input.QuotationNum, input.RevNum, input.QuotationCostItemGroupId, new List<QuotationCostItemLine> { }, context);
                costItem = _quotationRepository.DeleteCostItem(costItem, context);
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
                return input;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }


        #endregion

        public void DeleteQuotationLine(QuotationLineDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationLine _line = _quotationRepository.GetQuotationLine(input.QuotationNum, input.LineNum, input.RevNum, context);
                List<string> costItemGroupIds = _quotationRepository.GetQuotationCostItemLines(input.QuotationNum, input.LineNum, input.RevNum, context)
                                           .Select(x=>x.QuotationCostItemGroupId).Distinct().ToList();
                
                _quotationRepository.DeleteQuotationOptions(input.QuotationNum, input.LineNum, input.RevNum, context);
                _quotationRepository.DeleteCostItemLines(input.QuotationNum, input.LineNum, input.RevNum, context);
                _quotationRepository.DeleteCostItemGroup(input.QuotationNum, input.LineNum, input.RevNum, costItemGroupIds, context);
                _quotationRepository.DeleteQuotationLine(_line, context);
                
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }

        private List<QuotationLine> UpdateAllLinesCostItemValue(string quotationNum ,int revNum,QMTContext context)//,List<string>? groupIds = null
        {
            try
            {
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(quotationNum, revNum, context);

                List<QuotationCostItemLine> costItemLines = _quotationRepository.GetQuotationCostItemLines(quotationNum, revNum, context);

                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(quotationNum, revNum, context)
                                                    .Where(x => x.ActiveLine == true).ToList();

               
                Dictionary<string, decimal> groupIdTotalDict = new Dictionary<string, decimal>();
                foreach(var _costItem in costItems)
                {
                    List<QuotationCostItemLine> _costItemLines = costItemLines.Where(x=>x.QuotationCostItemGroupId == _costItem.QuotationCostItemGroupId).ToList();
                    List<int> lineNums = _costItemLines.Select(x => x.LineNum).Distinct().ToList();
                    decimal ttslsPrice = quotationLines.Where(x => lineNums.Contains(x.LineNum)).Select(x=>x.TtNetPrice).Sum();
                    if (!groupIdTotalDict.TryAdd(_costItem.QuotationCostItemGroupId, ttslsPrice))
                    {
                        groupIdTotalDict[_costItem.QuotationCostItemGroupId] += ttslsPrice; //prodcut wise total value 
                    }
                }

                quotationLines = _quotationRepository.UpdateCostValueOfAllQuotationLine(quotationLines, costItemLines, costItems, groupIdTotalDict, context);
                return quotationLines;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
                
            }
        }
        private List<QuotationLineDC> UpdateUnitPriceFromOptions(string quotatioNum, int revNum,List<int> lineNums,QMTContext context, List<PricingMasterDC>? pricingList = null)
        {
            try
            {
                List<QuotationLineDC> quotationLinesDC = new();
                foreach(int _lineNum in lineNums)
                {
                    decimal unitPrice = 0;
                    decimal totalNetPrice = 0;
                    QuotationLine quotationLine = _quotationRepository.GetQuotationLine(quotatioNum, _lineNum, revNum, context);
                    List<QuotationOptCode> quotationOptCodes = _quotationRepository.GetQuotationOptCodes(quotatioNum, revNum, context, _lineNum );
                    ItemCodeDetailsDC itemCodeDetailsDC = _itemCodeRepository.GetItemCodeDetails(new List<string> { quotationLine.ItemCode },context).First();
                    string? itemCode = null;
                    foreach (var _quoteOption in quotationOptCodes)
                    {
                        
                        unitPrice += (_quoteOption.UnitPrice ?? 0);
                        if(_quoteOption.IsNet.HasValue)
                        {
                            if (_quoteOption.IsNet.Value)
                                totalNetPrice += quotationLine.Qty * (_quoteOption.UnitPrice ?? 0);
                            else
                                totalNetPrice += quotationLine.Mtlp * quotationLine.Qty * (_quoteOption.UnitPrice ?? 0);
                        }
                        else
                            totalNetPrice += quotationLine.Mtlp * quotationLine.Qty * (_quoteOption.UnitPrice ?? 0);

                        if (pricingList != null)
                        {
                            if (pricingList.Any(x => x.OptCode == _quoteOption.OptCode))
                            {
                                PricingMasterDC pricingMaster = pricingList.Where(x => x.OptCode == _quoteOption.OptCode).First();
                                if (pricingMaster.IsItemCodeCreation)
                                {
                                    itemCode = _itemCodeService.CreateItemCode(itemCode ?? quotationLine.SubItemCode ?? quotationLine.ItemCode, _quoteOption.OptCode);
                                }
                            }
                        }
                        else
                        {

                        }
                        
                    }
                    QuotationLineDC lineDC = new();
                    lineDC.QuotationNum = quotatioNum;
                    lineDC.SubItemCode = itemCodeDetailsDC.ProdTypeId != "AHU" ? itemCode : quotationLine.SubItemCode;
                    lineDC.RevNum = revNum;
                    lineDC.LineNum = _lineNum;
                    lineDC.UnitPrice = unitPrice;
                    lineDC.ActiveLine = quotationLine.ActiveLine;
                    lineDC.Mtlp = quotationLine.Mtlp;
                    lineDC.Qty = quotationLine.Qty;
                    lineDC.CostItemLineValue = quotationLine.CostItemLineValue;
                    lineDC.Margin = quotationLine.Margin;
                    lineDC.Vat = quotationLine.Vat;
                    lineDC.TtNetPrice = totalNetPrice;
                    lineDC.TtslsPrice = CalculateTotalValue(lineDC);

                    lineDC.TtslsPriceWOVat = Math.Round(lineDC.TtNetPrice + (lineDC.CostItemLineValue ?? 0), 2);
                    lineDC.TtslsPriceWMargin = CalculateMarginValue(quotationLine.Margin, lineDC.TtslsPriceWOVat);

                    lineDC = _quotationRepository.UpdateQuotationLine(lineDC, context);
                    quotationLinesDC.Add(lineDC);
                }
                
                return quotationLinesDC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;

            }
        }

        private decimal CalculateTotalValue(QuotationLineDC inputLine)
        {
            decimal ttslsPriceWithoutVat = inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0);
            return Math.Round(ttslsPriceWithoutVat + (ttslsPriceWithoutVat * inputLine.Vat / 100),2);
        }

        private decimal CalculateMarginValue(decimal? margin , decimal totalPrice )
        {
            decimal marginValue = margin ?? 0;
            marginValue = (1 + marginValue / 100) * totalPrice;
            return marginValue;
        }

        private decimal CalculatePriceOnCurrency(CurrencyMaster quotationCurrency, PricingMasterDC pricing,ItemCodeDetailsDC item)
        {
            //CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(quotationCurrencyCode);
            return pricing.Price * (quotationCurrency.ConvFactor/ item.CAF);
        }

        private string GenerateQuotionNum(string areaCode ,int userId)
        {
            int num = _quotationRepository.GetQuotationLatestNum();
            UserMaster user = _mastersRepository.GetUserByUserId(userId);
            return "CHR"+user.FirstName.ToUpper()[0]+user.LastName.ToUpper()[0]+areaCode+ String.Format("{0:00000}", num);
        }


        public PriceBreakDownDC GetQuotationPBD(string quotationNum, int revNum)
        {
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(quotationNum, revNum);
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLinesDC(quotationNum, header!.RevNum);
                List<QuotationOptCode> optCodeList = _quotationRepository.GetQuotationOptCodes(quotationNum, revNum,null);
                List<string> productTypes = lines.Select(x => x.ProdTypeId).Distinct().ToList();
                List<QuotationCostItemLine> costItemLines = _quotationRepository.GetQuotationCostItemLines(quotationNum, revNum);
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(quotationNum, revNum);
                PriceBreakDownDC priceBreakDownDC = new();
                decimal totalSalePrice = 0;
                decimal totalCostValue = 0;
                decimal totalNetValue = 0;
                foreach (var productType in productTypes)
                {
                    ProductPrice productPrice = new ProductPrice();
                    productPrice.productType = productType;

                    DataTable dt = new();
                    dt.Columns.Add("LineNum");
                    dt.Columns.Add("ItemCode");
                    List<string> optcodes = _quotationRepository.GetQuotationOptions(quotationNum, revNum); // add productType
                    foreach (var optcode in optcodes)
                    {
                        dt.Columns.Add(optcode);
                    }
                    //int nuberOfEmptyCells = 30 - dt.Columns.Count;
                    //for(int i=0;i< nuberOfEmptyCells; i++)
                    //{
                    ///    dt.Columns.Add("&nbsp;");
                    //}
                    //int nuberOfCellsTillOptions = dt.Columns.Count;
                    dt.Columns.Add("UnitPrice");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("Mlp");
                    dt.Columns.Add("CostValue");
                    dt.Columns.Add("TtslsPrice");
                    dt.Columns.Add("VAT%");
                    dt.Columns.Add("VAT Amnt");
                    dt.Columns.Add("Total Amnt");
                    decimal totalSalePriceProduct = 0;
                    decimal totalCostValueProduct = 0;
                    decimal totalNetValueProduct = 0;
                    decimal totalQty = 0;
                    foreach (var lineDC in lines.Where(x=>x.ProdTypeId == productType))
                    {
                        totalSalePriceProduct += lineDC.TtslsPriceWOVat;
                        totalNetValue += lineDC.TtNetPrice;
                        totalCostValueProduct += (lineDC.CostItemLineValue ?? 0);
                        totalQty += lineDC.Qty;
                        List <QuotationOptCode> optCodeOfLinePrice = optCodeList.Where(x => x.LineNum == lineDC.LineNum).ToList();
                        DataRow dr = dt.NewRow();

                        dr[dt.Columns.IndexOf("LineNum")] = lineDC.LineNum;
                        dr[dt.Columns.IndexOf("ItemCode")] = lineDC.ItemCode;
                        foreach (var _opt in optCodeList)
                        {
                            QuotationOptCode? pricing = optCodeOfLinePrice.Where(x => x.OptCode == _opt.OptCode).FirstOrDefault();
                            dr[dt.Columns.IndexOf(_opt.OptCode)] = pricing != null ? Convert.ToString(pricing.UnitPrice) : "";
                        }
                        //for (int i = nuberOfCellsTillOptions; i < nuberOfEmptyCells; i++)
                        //{
                        //    dr[i] = "& nbsp; ";
                       // }

                        dr[dt.Columns.IndexOf("UnitPrice")] = lineDC.UnitPrice;
                        dr[dt.Columns.IndexOf("Qty")] = lineDC.Qty;
                        dr[dt.Columns.IndexOf("Mlp")] = lineDC.Mtlp;
                        dr[dt.Columns.IndexOf("CostValue")] = lineDC.CostItemLineValue;
                        dr[dt.Columns.IndexOf("TtslsPrice")] = lineDC.TtslsPriceWOVat;
                        dr[dt.Columns.IndexOf("VAT%")] = lineDC.Vat;
                        dr[dt.Columns.IndexOf("VAT Amnt")] = Math.Round(lineDC.Vat/100 * lineDC.TtslsPriceWOVat,2).ToString("#,##0.##"); 
                        dr[dt.Columns.IndexOf("Total Amnt")] = Math.Round(lineDC.TtslsPrice,2).ToString("#,##0.##");
                        dt.Rows.Add(dr);
                    }

                    DataTable dtProdTotals = new();
                    dtProdTotals.Columns.Add("TotalCostValue");
                    dtProdTotals.Columns.Add("TotalSlsPrice");
                    dtProdTotals.Columns.Add("TotalQty");
                    DataRow drProdTotal = dtProdTotals.NewRow();
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalCostValue")] = Math.Round(totalCostValueProduct,2).ToString("#,##0.##");
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalSlsPrice")] = Math.Round(totalSalePriceProduct,2).ToString("#,##0.##");
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalQty")] = Math.Round(totalQty, 2);
                    dtProdTotals.Rows.Add(drProdTotal);

                    productPrice.totals = dtProdTotals;
                    //dtTotals.Columns.Add("VatAmnt");
                    //dtTotals.Columns.Add("VatTotal");
                    totalSalePrice += totalSalePriceProduct;
                    totalCostValue += totalCostValueProduct;

                    productPrice.optionsPricing = dt;
                    priceBreakDownDC.productPrices.Add(productPrice);
                    List<int> lineNums = lines.Where(x => x.ProdTypeId == productType).Select(x => x.LineNum).ToList();
                    //Adding cost Item provsioning
                    DataTable dtCostProdItems = new();
                    dtCostProdItems.Columns.Add("CostItemCode");
                    dtCostProdItems.Columns.Add("TotCostProv");
                    dtCostProdItems.Columns.Add("Percentage");

                    var costItemLinesProd = costItemLines.Where(x => lineNums.Contains(x.LineNum));
                    List<string> costItemGroupIds = costItemLinesProd.Select(x => x.QuotationCostItemGroupId).Distinct().ToList();
                    foreach (var _costItem in costItems.Where(x=> costItemGroupIds.Contains(x.QuotationCostItemGroupId)))
                    {
                        DataRow drCostTotalProd = dtCostProdItems.NewRow();
                        decimal costLineValue = costItemLines.Where(x => x.QuotationCostItemGroupId == _costItem.QuotationCostItemGroupId)
                                            .Select(x=> x.CostItemLineValue).Sum();
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("CostItemCode")] = _mastersRepository.GetCostItemByCode(_costItem.CostItemId).CostItemName;
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("TotCostProv")] = Math.Round(costLineValue, 2).ToString("#,##0.##");
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("Percentage")] = Math.Round(costLineValue / totalSalePriceProduct * 100, 2);
                        dtCostProdItems.Rows.Add(drCostTotalProd);
                    }
                    productPrice.costItemProductWise = dtCostProdItems;

                }


                DataTable dtCostTotals = new();
                dtCostTotals.Columns.Add("CostItemCode");
                dtCostTotals.Columns.Add("TotCostProv");
                dtCostTotals.Columns.Add("Percentage");

                //List<CostItemBreakDownDC> costItemList = new List<CostItemBreakDownDC>();
                List<string> costItemCodes = costItems.Select(x => x.CostItemId).Distinct().ToList();
                foreach (var _costItemCode in costItemCodes)
                {
                    List<string> costItemGroupIds = costItems.Where(x => x.CostItemId == _costItemCode).Select(x=> x.QuotationCostItemGroupId).Distinct().ToList();
                    decimal costItemValue = costItemLines.Where(x => costItemGroupIds.Contains(x.QuotationCostItemGroupId))
                                           .Select(x => x.CostItemLineValue).Sum();
                    DataRow drTotal = dtCostTotals.NewRow();
                    drTotal[dtCostTotals.Columns.IndexOf("CostItemCode")] = _mastersRepository.GetCostItemByCode(_costItemCode).CostItemName;
                    drTotal[dtCostTotals.Columns.IndexOf("TotCostProv")] = Math.Round(costItemValue, 2).ToString("#,##0.##");
                    drTotal[dtCostTotals.Columns.IndexOf("Percentage")] = Math.Round(costItemValue / totalSalePrice * 100, 2);
                    dtCostTotals.Rows.Add(drTotal);
                }

                priceBreakDownDC.costItemBreakDownDCs = dtCostTotals;
                /*DataTable dtTotals = new();
                dtTotals.Columns.Add("TotalCostValue");
                dtTotals.Columns.Add("TotalSlsPrice");
                DataRow drTotal = dtTotals.NewRow();
                drTotal[dtTotals.Columns.IndexOf("TotalCostValue")] = totalCostValueProduct;
                drTotal[dtTotals.Columns.IndexOf("TotalSlsPrice")] = totalSalePriceProduct;*/
                priceBreakDownDC.quotationHeader = _quotationRepository.GetQuotationHeader(quotationNum,revNum)!;

                //List <QuotationCostItemDC> quotationCostItems = _quotationRepository.GetQuotationCostLines(quotationNum, revNum);
                return priceBreakDownDC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
