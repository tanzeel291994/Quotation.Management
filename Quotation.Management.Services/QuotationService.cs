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
using System.Globalization;
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
                header.QuotationNum = inputHeader.QuotationNum ?? GenerateQuotionNum(header.AreaCode, header.Msp, header.QuotationDate.Year);

                header = _quotationRepository.InsertUpdateQuotation(header, inputHeader.UserId);
                return header;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public QuotationHeader? GetQuotation(string Id, int? revNum = null)
        {
            //JObject jobject = new();
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id, revNum);
                //List<QuotationLineDC> lines = _quotationRepository.GetQuotationLinesDC(Id, header!.RevNum);
                //dynamic products = _productMasterRepository.GetProductsofQuotations(Id, header!.RevNum);
                //List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(Id, header!.RevNum);
                //jobject.Add(new JProperty("header", JsonConvert.SerializeObject(header!, new JsonSerializerSettings
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //})));
                //jobject.Add(new JProperty("lines", JsonConvert.SerializeObject(lines, new JsonSerializerSettings
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver()
                //})));
                //jobject.Add(new JProperty("products", JsonConvert.SerializeObject(products)));

                return header;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public List<string> GetAllQuotationNums()
        {
            try
            {
                var data  = _quotationRepository.GetAllQuotationNums();
                return data;
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
                decimal adjustedConvfactor = 1;
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLinesDC(Id, revNum);
                List<string> itemCodes = lines.Select(x => x.BaseItemCode).Distinct().ToList();
                List<ItemCodeDetailsDC> itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(itemCodes);
                
                QuotationHeader ? quotationHeader = _quotationRepository.GetQuotation(Id, revNum);
                string currencyCode = quotationHeader!.CurrencyCode;
                CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                CurrencyMaster? oldQuotationCurrency = null;
                /*if (quotationHeader!.OldCurrencyCode != null)
                {
                    oldQuotationCurrency = _mastersRepository.GetCurrencyByCode(quotationHeader!.OldCurrencyCode);
                }*/
                
                foreach (var _line in lines)
                {
                    ItemCodeDetailsDC itemCodeDetail = itemCodeDetails.Where(x => x.ItemCode == _line.BaseItemCode).FirstOrDefault();
                    /*if (quotationHeader!.ConvFactor != null) //conversion factor specified by the user  therfore there is an old currencu as well
                    {
                        var convFactor = quotationHeader!.ConvFactor.Value;
                        adjustedConvfactor = convFactor * oldQuotationCurrency!.ConvFactor;
                    }
                    else
                    {
                        //if (oldQuotationCurrency != null) // user chnageed the currency but convfactor was takesas deafult one
                        //{
                        //    adjustedConvfactor = Math.Round(quotationCurrency!.ConvFactor / oldQuotationCurrency!.ConvFactor, 4);
                        //}
                        //else
                        //{
                        adjustedConvfactor = quotationCurrency!.ConvFactor;
                        //}
                    }*/

                    //_line.CAF = _line.CAF;//itemCodeDetail != null ? Math.Round(adjustedConvfactor / itemCodeDetail.CAF, 4) : 1;
                    _line.IndexValue = itemCodeDetail != null ? itemCodeDetail.IndexConvFactor : 1;
                    _line.ProductCurrencyCode = itemCodeDetail != null ? itemCodeDetail.CurrencyCode : "";
                }

                return lines;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public CurrencyDC GetCurrencyCode(string curencyCode,string oldCurrencyCode, string quotationNum ,int revNum)
        {
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(quotationNum,revNum);
                CurrencyMaster? currency = _mastersRepository.GetCurrencyByCode(curencyCode);
                CurrencyMaster? oldCurrency = _mastersRepository.GetCurrencyByCode(oldCurrencyCode);
                CurrencyDC currencyDC = new();
                List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(quotationNum,revNum);
                currencyDC.productCAFs = new List<ProductCAFCode>();
                foreach(var _line in quotationLines)
                {
                    if(!currencyDC.productCAFs.Any(x=> x.ProductCode == _line.ProdTypeId))
                    {
                        currencyDC.productCAFs.Add(new ProductCAFCode { CAF = _line.CAF, ProductCode = _line.ProdTypeId });
                    }
                }
                
                 //oldcurrency code 
                 //   USD 				AED  X X+f
                 //   AED                     OMR  Y  (X+f)AED/USD(USD/EUR)(EUR/OMR)
                 
                if(header!.OldCurrencyCode != null)
                {
                    currencyDC.ConvFactor = Math.Round(header!.ConvFactor!.Value * oldCurrency!.ConvFactor * (1 / currency!.ConvFactor),4);
                }
                else
                {
                    currencyDC.ConvFactor = Math.Round(currency!.ConvFactor * (1/oldCurrency!.ConvFactor),4);
                }

                currencyDC.OldCurrencyCode = oldCurrency!.CurrencyCode;
                currencyDC.CurrencyCode = currency!.CurrencyCode;
                 //Math.Round(currency!.ConvFactor / oldCurrency.ConvFactor, 4);
                /*if(header!.ConvFactor != null)
                {
                    currencyDC.ConvFactor = Math.Round((header!.ConvFactor.Value * oldCurrency.ConvFactor)/ currency!.ConvFactor , 4);
                }
                else
                  currencyDC.ConvFactor = Math.Round( currency!.ConvFactor /  oldCurrency.ConvFactor, 4);*/
                return currencyDC;
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
                CurrencyMaster? currencyMaster = _mastersRepository.GetCurrencyByCode(currencyDC.CurrencyCode);
                CurrencyMaster? oldCurrencyMaster = _mastersRepository.GetCurrencyByCode(currencyDC.OldCurrencyCode);
                QMTContext context = _quotationRepository.BeginTransaction();               
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(currencyDC.QuotationNum, currencyDC.RevNum, context);
                quotationHeader!.CurrencyCode = currencyDC.CurrencyCode;
                quotationHeader!.OldCurrencyCode = currencyDC.OldCurrencyCode;
                decimal convFactor = currencyDC.ConvFactor;
                //if (currencyDC.NewConvFactor != null)
                quotationHeader!.ConvFactor = convFactor;

                quotationHeader = _quotationRepository.UpdateQuotationHeader(quotationHeader, context);
                List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(currencyDC.QuotationNum, currencyDC.RevNum, _context: context);

                List<QuotationOptCode> optCodeList = _quotationRepository.GetQuotationOptCodes(currencyDC.QuotationNum, currencyDC.RevNum, context);

                foreach (var _optCode in optCodeList)
                {
                    QuotationLineDC quotationLineDC = quotationLines.Where(x => x.LineNum == _optCode.LineNum && x.RevNum == _optCode.RevNum).First();
                    if(currencyDC.productCAFs.Where(x=> x.ProductCode == quotationLineDC.ProdTypeId).Any())
                    {
                        decimal newCAF = currencyDC.productCAFs.Where(x => x.ProductCode == quotationLineDC.ProdTypeId).First().CAF;
                        _optCode.UnitPrice = (newCAF) * _optCode.UnitPrice;
                    }
                    else
                    {
                        _optCode.UnitPrice = (quotationLineDC.CAF) * _optCode.UnitPrice;
                    }                   
                    _quotationRepository.UpdateQuotationOptCodes(_optCode, context);
                }

                UpdateUnitPriceFromOptions(currencyDC.QuotationNum, currencyDC.RevNum, quotationLines.Select(x => x.LineNum).ToList(), context);
                
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(currencyDC.QuotationNum, currencyDC.RevNum, context);
                foreach (var _costItem in costItems)
                {
                    if (_costItem.CostItemType == CostItemType.ByVal.ToString())
                    {
                        _costItem.CostItemValue *= convFactor;
                        if(_costItem.FreightRate != null)
                            _costItem.FreightRate *= convFactor;

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
        public bool UpdateMultipleLines(UpdateMultipleLinesDC data)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(data.QuotationNum, data.RevNum,context);
                quotationLines = quotationLines.Where(x => data.Lines.Contains(x.LineNum)).ToList();
                if(data.TypeOfUpdate == "Delete")
                {
                    foreach (var _line in quotationLines)
                    {
                        List<string> costItemGroupIds = _quotationRepository.GetQuotationCostItemLines(_line.QuotationNum, _line.LineNum, _line.RevNum, context)
                                                   .Select(x => x.QuotationCostItemGroupId).Distinct().ToList();

                        _quotationRepository.DeleteQuotationOptions(_line.QuotationNum, _line.LineNum, _line.RevNum, context);
                        _quotationRepository.DeleteCostItemLines(_line.QuotationNum, _line.LineNum, _line.RevNum, context);
                        _quotationRepository.DeleteCostItemGroup(_line.QuotationNum, _line.LineNum, _line.RevNum, costItemGroupIds, context);
                        _quotationRepository.DeleteQuotationLine(_line, context);
                    }
                }
                else
                    _quotationRepository.UpdateMultipleLines(quotationLines, data.inputValue, data.TypeOfUpdate, context);

                UpdateUnitPriceFromOptions(data.QuotationNum, data.RevNum, data.Lines, context);
                UpdateAllLinesCostItemValue(data.QuotationNum, data.RevNum, context);

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
        public bool CopyQuotationLinesFromQuotation(CopyQuotationLineDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(input.FromQuotationNum, input.Lines, input.FromRevNum, context);
                List<string> allItemCodes = quotationLines.Select(x => x.ItemCode).Distinct().ToList(); ;
                List<ItemCodeDetailsDC> itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(allItemCodes, context);
                foreach (var _line in quotationLines)
                {
                    QuotationLineDC inputLine = new();
                    inputLine.QuotationNum = input.ToQuotationNum;
                    inputLine.ActiveLine = true; 
                    inputLine.Qty = _line.Qty;
                    inputLine.Mtlp = _line.Mtlp;
                    inputLine.UnitPrice = _line.UnitPrice;
                    inputLine.ItemCode = _line.ItemCode;
                    inputLine.Vat = _line.Vat;
                    inputLine.Margin = _line.Margin;
                    inputLine.UnitTag = _line.UnitTag;
                    inputLine.RevNum = input.ToRevNum;
                    ItemCodeDetailsDC? detailsDC = itemCodeDetails.Where(x => x.ItemCode == _line.ItemCode).FirstOrDefault();
                    
                    AddQuotationLine(inputLine, detailsDC!, context);
                }

                UpdateUnitPriceFromOptions(input.ToQuotationNum, input.ToRevNum, input.Lines, context);
                UpdateAllLinesCostItemValue(input.ToQuotationNum, input.ToRevNum, context);

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
        public QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                ItemCodeDetailsDC itemDetails = _itemCodeRepository.GetItemCodeDetails(new List<string> { inputLine.ItemCode }, context).First();
                QuotationLine line = AddQuotationLine(inputLine, itemDetails, context);
                decimal costItemValue = 0;
                if (itemDetails.ProdTypeId != "AHU")
                {
                    List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                    costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                }
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(inputLine.QuotationNum, inputLine.RevNum, context);
                string currencyCode = quotationHeader!.CurrencyCode;
                CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                inputLine.LineNum = line.LineNum;
                inputLine.UnitPrice = line.UnitPrice;
                inputLine.BaseItemCode = line.ItemCode;
                inputLine.TtNetPrice = line.TtNetPrice;
                inputLine.Margin = line.Margin;
                inputLine.ProductCurrencyCode = itemDetails != null ? itemDetails.CurrencyCode : null;
                inputLine.ActiveLine = line.ActiveLine;
                inputLine.CAF = itemDetails != null ? Math.Round(quotationCurrency!.ConvFactor / itemDetails.CAF, 4) : 1;
                inputLine.IndexValue = itemDetails.IndexConvFactor;
                inputLine.UnitTag = line.UnitTag ?? "";
                inputLine.CostItemLineValue = costItemValue;
                inputLine.ProdTypeId = itemDetails.ProdTypeId;
                inputLine.TtCostPrice = Math.Round((inputLine.TtNetPrice) + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtSlsPrice = Math.Round(CalculatetotalWithMargin(inputLine), 2);
                inputLine.TtSlsPriceWTVat = Math.Round(CalculateTotalValue(inputLine),2);

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
        
        private QuotationLine AddQuotationLine(QuotationLineDC inputLine,ItemCodeDetailsDC itemDetails, QMTContext context)
        {
            QuotationLine line = new();
            QuotationLine? latestLine = _quotationRepository.GetLatestQuotationLine(inputLine.QuotationNum, context);
            QuotationHeader? header = _quotationRepository.GetQuotation(inputLine.QuotationNum, inputLine.RevNum);
            CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(header!.CurrencyCode);
            CurrencyMaster? brandCurrency = _mastersRepository.GetCurrencyByCode(itemDetails.CurrencyCode);
           
            line.QuotationNum = inputLine.QuotationNum;
            line.ActiveLine = true; // BY DEFAULT ALL LINES ARE ACTIVE WHEN INSERTED
            line.Qty = inputLine.Qty;
            line.Mtlp = inputLine.Mtlp;
            line.UnitPrice = inputLine.UnitPrice;
            line.ItemCode = inputLine.ItemCode;
            line.Vat = inputLine.Vat;
            line.Margin = inputLine.Margin;
            line.UnitTag = inputLine.UnitTag;
            line.LineNum = latestLine != null ? latestLine.LineNum + 1 : 1;
            line.RevNum = inputLine.RevNum;
            line.UnitPrice = 0;
            line.TtNetPrice = 0;
            line.CAF = Math.Round(quotationCurrency!.ConvFactor / brandCurrency!.ConvFactor, 4);
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



                //string currencyCode = header!.CurrencyCode;
                //CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);

                line.UnitPrice = line.CAF * pricing.First().Price; //currencyCode == itemDetails.CurrencyCode ? pricing.First().Price : CalculatePriceOnCurrency(quotationCurrency!, pricing.First(), itemDetails);
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
                
            }
            return line;
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
                
                inputLine.TtCostPrice = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0),2);
                inputLine.TtSlsPrice = Math.Round(CalculatetotalWithMargin(inputLine), 2);
                inputLine.TtSlsPriceWTVat = Math.Round(CalculateTotalValue(inputLine),2);

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
                List<string> optCodesToBeAdded = inputLine.optCodes!.Split(',').ToList();
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(inputLine.QuotationNum,inputLine.RevNum);
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(inputLine.QuotationNum, inputLine.LineNum, inputLine.RevNum);
                List<PricingMasterDC> pricingList = _quotationRepository.GetPricingOptCode(quotationLine.ItemCode, optCodesToBeAdded);
                string currencyCode = quotationHeader!.CurrencyCode;
                CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                ItemCodeDetailsDC itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(new List<string> { inputLine.BaseItemCode }).First();
                QMTContext context = _quotationRepository.BeginTransaction();
                foreach (var item in pricingList)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = inputLine.RevNum;
                    optCode.LineNum = inputLine.LineNum;
                    optCode.UnitPrice = quotationLine.CAF * item.Price; //currencyCode == itemCodeDetails.CurrencyCode ? item.Price : CalculatePriceOnCurrency(quotationCurrency!, item, itemCodeDetails);
                    if (itemCodeDetails.IndexConvFactor != null) optCode.UnitPrice = optCode.UnitPrice * itemCodeDetails.IndexConvFactor.Value;

                    optCode.OptCode = item.OptCode;
                    optCode.IsNet = item.IsNet;
                    optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                }

                List<string> optCodesExisting = _quotationRepository.GetQuotationOptCodes(inputLine.QuotationNum, inputLine.RevNum, context, inputLine.LineNum)
                                                            .Select(x => x.OptCode).ToList();
                List<PricingMasterDC> pricingListNew = _quotationRepository.GetPricingOptCode(quotationLine.ItemCode, optCodesExisting);

                List<QuotationLineDC> linesDC = UpdateUnitPriceFromOptions(inputLine.QuotationNum, inputLine.RevNum, new List<int> { inputLine.LineNum }, context, pricingListNew);
                QuotationLineDC lineDC = linesDC.Where(x => x.LineNum == inputLine.LineNum).First();
                
                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                inputLine.CostItemLineValue = lines.Where(x => x.LineNum == inputLine.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.UnitPrice = lineDC.UnitPrice;
                inputLine.TtNetPrice = linesDC.First().TtNetPrice;
                inputLine.TtCostPrice = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtSlsPrice = Math.Round(CalculatetotalWithMargin(inputLine), 2);
                inputLine.TtSlsPriceWTVat = Math.Round(CalculateTotalValue(lineDC),2);
                

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
                List<string> optCodesToBeRemoved = inputLine.optCodes!.Split(',').ToList();
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(inputLine.QuotationNum, inputLine.LineNum, inputLine.RevNum);
               
                foreach (var item in optCodesToBeRemoved)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = inputLine.RevNum;
                    optCode.LineNum = inputLine.LineNum;
                    optCode.OptCode = item;
                    _quotationRepository.RemoveQuotationOptCode(optCode, context);
                }
                List<string> optCodesExisting = _quotationRepository.GetQuotationOptCodes(inputLine.QuotationNum, inputLine.RevNum, context, inputLine.LineNum)
                                                            .Select(x=>x.OptCode).ToList();
                List<PricingMasterDC> pricingList = _quotationRepository.GetPricingOptCode(quotationLine.ItemCode, optCodesExisting);
                List<QuotationLineDC> linesDC = UpdateUnitPriceFromOptions(inputLine.QuotationNum, inputLine.RevNum, new List<int> { inputLine.LineNum }, context, pricingList);
                QuotationLineDC lineDC = linesDC.Where(x => x.LineNum == inputLine.LineNum).First();

                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                inputLine.CostItemLineValue = lines.Where(x => x.LineNum == inputLine.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.UnitPrice = lineDC.UnitPrice;
                               
                inputLine.TtNetPrice = linesDC.First().TtNetPrice;
                inputLine.TtCostPrice = Math.Round(inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0), 2);
                inputLine.TtSlsPrice = Math.Round(CalculatetotalWithMargin(inputLine), 2);
                inputLine.TtSlsPriceWTVat = Math.Round(CalculateTotalValue(lineDC), 2); // with VAT

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
                ItemCodeDetailsDC? itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(new List<string> { optCodeDC.ItemCode! }).FirstOrDefault();
                QMTContext context = _quotationRepository.BeginTransaction();

                var optCodeExists= _quotationRepository.GetQuotationOptCode(optCodeDC.QuotationNum, optCodeDC.RevNum,optCodeDC.LineNum,optCodeDC.OptCode,context);
                if(optCodeExists != null)
                {
                    throw new ValidationException(new List<string> {"Opt code:"+ optCodeDC.OptCode+" already exist for the line"});
                }
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(optCodeDC.QuotationNum, optCodeDC.RevNum);
                string currencyCode = quotationHeader!.CurrencyCode;
                //CurrencyMaster? quotationCurrency = _mastersRepository.GetCurrencyByCode(currencyCode);
                QuotationLine quotationLine = _quotationRepository.GetQuotationLine(optCodeDC.QuotationNum, optCodeDC.LineNum, optCodeDC.RevNum,context);

                QuotationOptCode optCode = new();
                optCode.QuotationNum = optCodeDC.QuotationNum;
                optCode.RevNum = optCodeDC.RevNum;
                optCode.LineNum = optCodeDC.LineNum;
                optCode.UnitPrice = optCodeDC.Price * quotationLine.CAF;//optCodeDC.Price * (itemCodeDetails != null ? Math.Round(quotationCurrency!.ConvFactor / itemCodeDetails.CAF, 4) : 1);
                optCode.OptCode = optCodeDC.OptCode;
                optCode.OptName = optCodeDC.OptName;
                optCode.Baseprice = optCodeDC.Price;
                optCode.IsNet = optCodeDC.IsNet != null ? optCodeDC.IsNet.Value : false;
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
                //QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(quotationLineDC.QuotationNum, quotationLineDC.RevNum);
                //CurrencyMaster? brandCurrency = _itemCodeRepository.GetItemCodeCurrency(quotationLine.ItemCode);
                //decimal oldConvFactor = brandCurrency!.ConvFactor;
                //decimal newConvFactor = 0;


                //CurrencyMaster? currencyMaster =  _mastersRepository.GetCurrencyByCode(quotationHeader.CurrencyCode);
                //newConvFactor = currencyMaster!.ConvFactor;

                jobject.Add("allOptions", JsonConvert.SerializeObject(_quotationRepository.GetItemOptions(quotationLine.ItemCode, 1)));//newConvFactor/ oldConvFactor

                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public List<QuotationCostItemDC> InsertQuotationCostItem(List<QuotationCostItemDC> inputList)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                foreach (var input in inputList)
                {
                    QuotationCostItem costItem = new();
                    costItem.QuotationNum = input.QuotationNum;
                    costItem.RevNum = input.RevNum;
                    costItem.CostItemType = input.CostItemType;
                    costItem.CostItemId = input.CostItemId;
                    costItem.ProdTypeId = input.ProdTypeId;
                    costItem.FreightRate = input.FreightRate;
                    costItem.NoOfContainers = input.NoOfContainers;
                    if (input.FreightRate != null && input.NoOfContainers != null)
                        costItem.CostItemValue = Math.Round(input.FreightRate.Value * input.NoOfContainers.Value, 2);
                    else
                        costItem.CostItemValue = input.CostItemValue;
                    costItem.QuotationCostItemGroupId = Guid.NewGuid().ToString();
                    List<int> lineNums = input.quotationLineCostItems.Select(x => x.LineNum).ToList();
                    List<QuotationLineDC> quotationLines = _quotationRepository.GetQuotationLinesDC(input.QuotationNum, input.RevNum, selectedLines: lineNums);
                    decimal ttslsPrice = 0;
                    List<QuotationCostItemLine> costItemLines = new();
                    foreach (var _quotationLine in quotationLines)
                    {
                        ttslsPrice += _quotationLine.TtNetPrice;//(_quotationLine.UnitPrice * _quotationLine.Mtlp * _quotationLine.Qty);
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
                            costItemLine.CostItemLineValue = costItem.CostItemValue * (quotationLine.TtNetPrice / ttslsPrice); //(quotationLine.UnitPrice * quotationLine.Mtlp * quotationLine.Qty)
                        }
                        if (costItem.CostItemType == CostItemType.ByPercentage.ToString())
                        {
                            costItemLine.CostItemLineValue = costItem.CostItemValue / 100 * (quotationLine.TtNetPrice);
                        }
                        costItemLines.Add(costItemLine);

                    }
                    costItem = _quotationRepository.InsertQuotationCostItemLine(costItem, context);
                    costItemLines = _quotationRepository.InUpdDelQuotationCostItemLines(input.QuotationNum, input.RevNum, input.QuotationCostItemGroupId, costItemLines, context);
                  
                }
                 if(inputList.Count > 0) UpdateAllLinesCostItemValue(inputList[0].QuotationNum, inputList[0].RevNum, context);
                _quotationRepository.Commit();
                return inputList;
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

        #region Revisions
        public void SetActiveRevision(string quotationNum, int revNum)
        {
            try
            {
                _quotationRepository.SetActiveRevision(quotationNum, revNum);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public dynamic GetAllRevisions(string quotationNum)
        {
            try
            {
               return _quotationRepository.GetAllRevisions(quotationNum);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public int CreateRevision(string quotationNum,int revNum,int userId)
        {
            try
            {
                int newRevNum = _quotationRepository.GetNewRevNum(quotationNum);
                int result = _quotationRepository.CreateRevision(quotationNum,newRevNum,revNum);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
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

        private List<QuotationLine> UpdateAllLinesCostItemValue(string quotationNum ,int revNum,QMTContext context)
        {
            try
            {
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(quotationNum, revNum, context);

                List<QuotationCostItemLine> costItemLines = _quotationRepository.GetQuotationCostItemLines(quotationNum, revNum, context);

                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(quotationNum, revNum, context)
                                                    .Where(x => x.ActiveLine == true).ToList();
                List<MasterDC> costItemCodes = _mastersRepository.GetCostItems(context);

                string customDutyCostCode = costItemCodes.Where(x => x.Name == "Customs Duty").First().Code;
                string seaFreightCostCode = costItemCodes.Where(x => x.Name == "Sea Freight").First().Code;

                List<QuotationCostItem> customDutyItems = costItems.Where(x => x.CostItemId == customDutyCostCode).ToList();
               
                Dictionary<string, decimal> groupIdTotalDict = new();
                foreach(var _costItem in costItems)
                {
                    List<QuotationCostItemLine> _costItemLines = costItemLines.Where(x => x.QuotationCostItemGroupId == _costItem.QuotationCostItemGroupId).ToList();
                    List<int> lineNums = _costItemLines.Select(x => x.LineNum).Distinct().ToList();
                    decimal ttslsPrice = quotationLines.Where(x => lineNums.Contains(x.LineNum)).Select(x => x.TtNetPrice).Sum();
                    if (!groupIdTotalDict.TryAdd(_costItem.QuotationCostItemGroupId, ttslsPrice))
                    {
                        groupIdTotalDict[_costItem.QuotationCostItemGroupId] += ttslsPrice; //prodcut wise total value 
                    }
                }
                groupIdTotalDict = _quotationRepository.UpdateCostValueOfAllQuotationLine(quotationLines, costItemLines, costItems, groupIdTotalDict,seaFreightCostCode, customDutyCostCode, context);
                _quotationRepository.UpdateCustomDutyCostItemValue(customDutyItems, groupIdTotalDict,context);
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
                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(quotatioNum, lineNums, revNum, context);
                List<string> itemCodes = quotationLines.Select(x => x.ItemCode).Distinct().ToList();
                List<ItemCodeDetailsDC> itemCodeListDetailsDC = _itemCodeRepository.GetItemCodeDetails(itemCodes, context);
                foreach (var quotationLine in quotationLines)
                {
                    decimal unitPrice = 0;
                    decimal totalNetPrice = 0;
                    ItemCodeDetailsDC itemCodeDetailsDC = itemCodeListDetailsDC.Where(x => x.ItemCode == quotationLine.ItemCode).First();
                    List<QuotationOptCode> quotationOptCodes = _quotationRepository.GetQuotationOptCodes(quotatioNum, revNum, context, quotationLine.LineNum);
                    
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
                                //PricingMasterDC pricingMaster = pricingList.Where(x => x.OptCode == _quoteOption.OptCode).First();
                                if (quotationLine.ItemCode.IndexOfAny(new char[] { '°' }) > 0) //pricingMaster.IsItemCodeCreation
                                {
                                    itemCode = _itemCodeService.CreateItemCode(itemCode ??  quotationLine.ItemCode, _quoteOption.OptCode);
                                }
                            }
                        }
                        
                    }
                    QuotationLineDC lineDC = new();
                    lineDC.QuotationNum = quotatioNum;
                    lineDC.SubItemCode = itemCodeDetailsDC.ProdTypeId != "AHU" ? itemCode : quotationLine.SubItemCode;
                    lineDC.RevNum = revNum;
                    lineDC.LineNum = quotationLine.LineNum;
                    lineDC.UnitPrice = unitPrice;
                    lineDC.ActiveLine = quotationLine.ActiveLine;
                    lineDC.Mtlp = quotationLine.Mtlp;
                    lineDC.Qty = quotationLine.Qty;
                    lineDC.CostItemLineValue = quotationLine.CostItemLineValue;
                    lineDC.Margin = quotationLine.Margin;
                    lineDC.UnitTag = quotationLine.UnitTag;
                    lineDC.Vat = quotationLine.Vat;
                    lineDC.TtNetPrice = totalNetPrice;
                    lineDC.TtCostPrice = Math.Round(lineDC.TtNetPrice + (lineDC.CostItemLineValue ?? 0), 2);
                    lineDC.TtSlsPrice = Math.Round(CalculatetotalWithMargin(lineDC), 2);
                    lineDC.TtSlsPriceWTVat = Math.Round(CalculateTotalValue(lineDC),2);

                    

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
            decimal ttCostPrice = inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0);
            decimal marginPercentage = inputLine.Margin ?? 0;
            decimal totalWithMarginValue = ttCostPrice / (1 - (marginPercentage / 100));
            return Math.Round(((100 + inputLine.Vat) / 100) * totalWithMarginValue,2);
        }

        private decimal CalculatetotalWithMargin(QuotationLineDC inputLine)
        {
            decimal ttCostPrice = inputLine.TtNetPrice + (inputLine.CostItemLineValue ?? 0);
            decimal marginPercentage = inputLine.Margin ?? 0;
            decimal totalWithMarginValue = ttCostPrice / (1- (marginPercentage/100));
            return Math.Round(totalWithMarginValue, 2);
        }

        private decimal CalculatePriceOnCurrency(CurrencyMaster quotationCurrency, PricingMasterDC pricing,ItemCodeDetailsDC item)
        {
            return pricing.Price * (quotationCurrency.ConvFactor/ item.CAF);
        }

        private string GenerateQuotionNum(string areaCode ,int userId,int year)
        {
            int num = _quotationRepository.GetQuotationLatestNum(areaCode,userId,year);
            UserMaster user = _mastersRepository.GetUserByUserId(userId);
            return "CRH"+user.FirstName.ToUpper()[0]+user.LastName.ToUpper()[0]+areaCode+year.ToString().Substring(2,2)+ String.Format("{0:0000}", num);
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
                decimal totalCostPrice = 0;
                decimal totalSalePrice = 0;
                decimal totalCostValue = 0;
                decimal totalQty = 0;
                //decimal totalNetValue = 0;
                foreach (var productType in productTypes)
                {
                    ProductPrice productPrice = new();
                    productPrice.productType = productType;

                    DataTable dt = new();
                    dt.Columns.Add("LineNum");
                    dt.Columns.Add("ItemCode");
                    List<string> optcodes = _quotationRepository.GetQuotationOptions(quotationNum, revNum); // add productType
                    if(optcodes.Any(x=> x == "BASIC")) dt.Columns.Add("BASIC");
                    foreach (var optcode in optcodes.Where(x => x != "BASIC"))
                    {
                        dt.Columns.Add(optcode);
                    }
                    dt.Columns.Add(" ");
                    dt.Columns.Add("ListPrice");
                    dt.Columns.Add("Qty");
                    dt.Columns.Add("Mlp");
                    dt.Columns.Add("TotalNet");
                    dt.Columns.Add("CostValue");
                    dt.Columns.Add("TotalCost");
                    dt.Columns.Add("Margin%");
                    dt.Columns.Add("TotalPrice");
                    dt.Columns.Add("VAT%");
                    dt.Columns.Add("Total Amnt");
                    decimal totalCostPriceProduct = 0;
                    decimal totalSalesPriceProduct = 0;
                    decimal totalCostValueProduct = 0;
                    decimal totalNetValueProduct = 0;
                    decimal totalQtyofProduct = 0;
                    decimal totalAmntOfProduct = 0;
                    foreach (var lineDC in lines.Where(x=>x.ProdTypeId == productType))
                    {
                        decimal totalMargin = CalculatetotalWithMargin(lineDC);
                        decimal totalAmt = CalculateTotalValue(lineDC);
                        totalCostPriceProduct += lineDC.TtCostPrice;
                        totalNetValueProduct += lineDC.TtNetPrice;
                        totalCostValueProduct += (lineDC.CostItemLineValue ?? 0);
                        totalQtyofProduct += lineDC.Qty;
                        totalSalesPriceProduct += totalMargin;
                        totalAmntOfProduct += totalAmt;
                        List <QuotationOptCode> optCodeOfLinePrice = optCodeList.Where(x => x.LineNum == lineDC.LineNum).ToList();
                        DataRow dr = dt.NewRow();

                        dr[dt.Columns.IndexOf("LineNum")] = lineDC.LineNum;
                        dr[dt.Columns.IndexOf("ItemCode")] = lineDC.ItemCode;

                        foreach (var _opt in optCodeList)
                        {
                            QuotationOptCode? pricing = optCodeOfLinePrice.Where(x => x.OptCode == _opt.OptCode).FirstOrDefault();
                            dr[dt.Columns.IndexOf(_opt.OptCode)] = pricing != null ? Math.Round(pricing.UnitPrice!.Value, 2).ToString("#,##0.##") : "";
                        }
                        dr[dt.Columns.IndexOf(" ")] = " ";
                        dr[dt.Columns.IndexOf("ListPrice")] = Math.Round(lineDC.UnitPrice, 2).ToString("#,##0.##");
                        dr[dt.Columns.IndexOf("Qty")] = lineDC.Qty;
                        dr[dt.Columns.IndexOf("Mlp")] = lineDC.Mtlp;
                        dr[dt.Columns.IndexOf("TotalNet")] = lineDC.TtNetPrice;
                        dr[dt.Columns.IndexOf("CostValue")] = lineDC.CostItemLineValue.HasValue? Math.Round(lineDC.CostItemLineValue.Value, 2).ToString("#,##0.##"):0;
                        dr[dt.Columns.IndexOf("TotalCost")] = Math.Round(lineDC.TtCostPrice, 2).ToString("#,##0.##");
                        dr[dt.Columns.IndexOf("Margin%")] = lineDC.Margin ?? 0;
                        dr[dt.Columns.IndexOf("TotalPrice")] = totalMargin.ToString("#,##0.##");
                        dr[dt.Columns.IndexOf("VAT%")] = lineDC.Vat;
                        dr[dt.Columns.IndexOf("Total Amnt")] = totalAmt.ToString("#,##0.##");
                        dt.Rows.Add(dr);
                    }
                    DataRow drtotal = dt.NewRow();
                    foreach (var columnName in dt.Columns)
                    {
                      
                        if (columnName.ToString() == "ItemCode")
                            drtotal[dt.Columns.IndexOf("ItemCode")] = "Totals";
                        else if (columnName.ToString() == "Qty")
                            drtotal[dt.Columns.IndexOf("Qty")] = Math.Round(totalQtyofProduct, 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "TotalNet")
                            drtotal[dt.Columns.IndexOf("TotalNet")] = Math.Round(totalNetValueProduct, 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "CostValue")
                            drtotal[dt.Columns.IndexOf("CostValue")] = Math.Round(totalCostValueProduct, 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "TotalCost")
                            drtotal[dt.Columns.IndexOf("TotalCost")] = Math.Round(totalCostPriceProduct, 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "Margin%")
                            drtotal[dt.Columns.IndexOf("Margin%")] = Math.Round(100 * (1 - (totalCostPriceProduct / totalSalesPriceProduct)), 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "TotalPrice")
                            drtotal[dt.Columns.IndexOf("TotalPrice")] = Math.Round(totalSalesPriceProduct, 2).ToString("#,##0.##");
                        else if (columnName.ToString() == "Total Amnt")
                            drtotal[dt.Columns.IndexOf("Total Amnt")] = Math.Round(totalAmntOfProduct, 2).ToString("#,##0.##");
                        else
                            drtotal[dt.Columns.IndexOf(columnName.ToString())] = "";
                       
                    }
                    dt.Rows.Add(drtotal);

                    /*DataTable dtProdTotals = new();
                    dtProdTotals.Columns.Add("TotalCostValue");
                    dtProdTotals.Columns.Add("TotalCost");
                    dtProdTotals.Columns.Add("TotalPrice");
                    dtProdTotals.Columns.Add("TotalQty");
                    dtProdTotals.Columns.Add("TotalMargin");
                    DataRow drProdTotal = dtProdTotals.NewRow();
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalCostValue")] = Math.Round(totalCostValueProduct,2).ToString("#,##0.##");
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalCost")] = Math.Round(totalCostPriceProduct, 2).ToString("#,##0.##");
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalPrice")] = Math.Round(totalSalesPriceProduct, 2).ToString("#,##0.##");
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalQty")] = Math.Round(totalQtyofProduct, 2);
                    drProdTotal[dtProdTotals.Columns.IndexOf("TotalMargin")] = Math.Round(100*(1-(totalNetValueProduct/ totalSalesPriceProduct)), 2);
                    dtProdTotals.Rows.Add(drProdTotal);

                    productPrice.totals = dtProdTotals;*/
                    //dtTotals.Columns.Add("VatAmnt");
                    //dtTotals.Columns.Add("VatTotal");
                    totalCostPrice += totalCostPriceProduct;
                    totalCostValue += totalCostValueProduct;
                    totalSalePrice += totalSalesPriceProduct;
                    totalQty += totalQtyofProduct;

                    productPrice.optionsPricing = dt;
                    priceBreakDownDC.productPrices.Add(productPrice);
                    List<int> lineNums = lines.Where(x => x.ProdTypeId == productType).Select(x => x.LineNum).ToList();
                    //Adding cost Item provsioning
                    DataTable dtCostProdItems = new();
                    dtCostProdItems.Columns.Add("CostItemCode");
                    dtCostProdItems.Columns.Add("TotCostProv");
                    dtCostProdItems.Columns.Add("Value");

                    var costItemLinesProd = costItemLines.Where(x => lineNums.Contains(x.LineNum));
                    List<string> costItemGroupIds = costItemLinesProd.Select(x => x.QuotationCostItemGroupId).Distinct().ToList();
                    foreach (var _costItem in costItems.Where(x=> costItemGroupIds.Contains(x.QuotationCostItemGroupId)))
                    {
                        DataRow drCostTotalProd = dtCostProdItems.NewRow();
                        decimal costLineValue = costItemLines.Where(x => x.QuotationCostItemGroupId == _costItem.QuotationCostItemGroupId)
                                            .Select(x=> x.CostItemLineValue).Sum();
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("CostItemCode")] = _mastersRepository.GetCostItemByCode(_costItem.CostItemId).CostItemName;
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("TotCostProv")] = Math.Round(costLineValue, 2).ToString("#,##0.##");
                        drCostTotalProd[dtCostProdItems.Columns.IndexOf("Value")] = _costItem.CostItemValue + (_costItem.CostItemType == CostItemType.ByPercentage.ToString() ? "%" : "");  //Math.Round(costLineValue / totalSalePriceProduct * 100, 2);
                        dtCostProdItems.Rows.Add(drCostTotalProd);
                    }
                    productPrice.costItemProductWise = dtCostProdItems;

                }


                DataTable dtCostTotals = new();
                dtCostTotals.Columns.Add("CostItemCode");
                dtCostTotals.Columns.Add("TotCostProv");
                //dtCostTotals.Columns.Add("Percentage");

                List<string> costItemCodes = costItems.Select(x => x.CostItemId).Distinct().ToList();
                foreach (var _costItemCode in costItemCodes)
                {
                    List<string> costItemGroupIds = costItems.Where(x => x.CostItemId == _costItemCode).Select(x=> x.QuotationCostItemGroupId).Distinct().ToList();
                    decimal costItemValue = costItemLines.Where(x => costItemGroupIds.Contains(x.QuotationCostItemGroupId))
                                           .Select(x => x.CostItemLineValue).Sum();
                    DataRow drCostTotal = dtCostTotals.NewRow();
                    drCostTotal[dtCostTotals.Columns.IndexOf("CostItemCode")] = _mastersRepository.GetCostItemByCode(_costItemCode).CostItemName;
                    drCostTotal[dtCostTotals.Columns.IndexOf("TotCostProv")] = Math.Round(costItemValue, 2).ToString("#,##0.##");
                    //drCostTotal[dtCostTotals.Columns.IndexOf("Percentage")] = Math.Round(costItemValue / totalSalePrice * 100, 2);
                    dtCostTotals.Rows.Add(drCostTotal);
                }

                priceBreakDownDC.costItemBreakDownDCs = dtCostTotals;

                DataTable dtTotals = new();
                dtTotals.Columns.Add("TotalCostValue");
                dtTotals.Columns.Add("TotalCost");
                dtTotals.Columns.Add("TotalPrice");
                dtTotals.Columns.Add("TotalQty");
                DataRow drTotal = dtTotals.NewRow();
                drTotal[dtTotals.Columns.IndexOf("TotalCostValue")] =  Math.Round(totalCostValue, 2).ToString("#,##0.##");
                drTotal[dtTotals.Columns.IndexOf("TotalCost")] =   Math.Round(totalCostPrice, 2).ToString("#,##0.##");
                drTotal[dtTotals.Columns.IndexOf("TotalPrice")] =   Math.Round(totalSalePrice, 2).ToString("#,##0.##");
                drTotal[dtTotals.Columns.IndexOf("TotalQty")] = totalQty;
                dtTotals.Rows.Add(drTotal);
                priceBreakDownDC.totalValueDCs = dtTotals;

                priceBreakDownDC.quotationHeader = _quotationRepository.GetQuotationHeader(quotationNum,revNum)!;
                return priceBreakDownDC;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void ImportQuotationLines(DataSet ds,string quotationNum , int revNum)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("Lines");
                List<QuotationLineDC> quotationLineDCList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    var columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray().ToList();
                    List<string> validItemCodes = new();
                    List<string?> allItemCodes = dt.Rows.Cast<DataRow>().Select(x => x.Field<string>("ItemCode")).ToArray().ToList();
                    List<string> requiredColumns = new List<string>() { "ItemCode", "UnitTag","Qty", "Mtlp", "Margin", "Vat%" };
                    foreach(var col in requiredColumns)
                    {
                        if (!columnNames.Contains(col)) validationMessages.Add(col+" column is not existing in the file");
                    }

                    validationMessages.AddRange(_itemCodeRepository.ValidateAllItemCodes(allItemCodes, out validItemCodes));
                    if (validationMessages.Count > 0) throw new ValidationException(validationMessages);
                    
                    QMTContext context = _quotationRepository.BeginTransaction();
                    List<ItemCodeDetailsDC> itemCodeDetails = _itemCodeRepository.GetItemCodeDetails(allItemCodes, context);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string? itemCode = dt.Rows[i].Field<string>("ItemCode");
                        //string? quotationNum = dt.Rows[i].Field<string>("QuotationNum");
                        //string? revNum = Convert.ToString(dt.Rows[i].Field<object>("RevNum"));
                        string? unitTag = dt.Rows[i].Field<string>("UnitTag");
                        string? qty = Convert.ToString(dt.Rows[i].Field<object>("Qty"));
                        string? mtlp = Convert.ToString(dt.Rows[i].Field<object>("Mtlp"));
                        string? vat =  Convert.ToString(dt.Rows[i].Field<object>("Vat%"));
                        string? margin = Convert.ToString(dt.Rows[i].Field<object>("Margin"));


                        ValidateCellValue(itemCode, "ItemCode",i,false,out string? message);
                        if (message != null) validationMessages.Add(message);
                        ItemCodeDetailsDC? detailsDC = itemCodeDetails.Where(x => x.ItemCode == itemCode).FirstOrDefault();
                        if (detailsDC == null) validationMessages.Add(itemCode + " ItemCode on Index " + i + " is missing.");
                        
                        ValidateCellValue(qty, "Qty", i, true, out string? message1);
                        if (message1 != null) validationMessages.Add(message1);

                        /*ValidateCellValue(quotationNum, "QuotationNum", i, false, out string? message4);
                        if (message4 != null) validationMessages.Add(message4);

                        ValidateCellValue(revNum, "RevNum", i, true, out string? message5);
                        if (message5 != null) validationMessages.Add(message5);*/

                        ValidateCellValue(vat, "Vat%", i, true, out string? message2);
                        if (message2 != null) validationMessages.Add(message2);

                        if(detailsDC!.Mtlp == null)
                        {
                            ValidateCellValue(mtlp, "Mtlp", i, true, out string? message6);
                            if (message6 != null) validationMessages.Add(message6);
                        }
                        if(detailsDC != null)
                        {
                            if(detailsDC!.ProdTypeId == "AHU")
                            {
                                ValidateCellValue(unitTag, "UnitTag", i, false, out string? message3);
                                if (message3 != null) validationMessages.Add(message3);
                            }
                        }
                        if (validationMessages.Count > 0) throw new ValidationException(validationMessages);
                        if(detailsDC!.ProdTypeId == "AHU")
                        {
                            itemCode = quotationNum + unitTag + itemCode;
                        }
                        //
                        QuotationLineDC inputLine = new();
                        inputLine.QuotationNum = quotationNum!;
                        inputLine.ActiveLine = true; // BY DEFAULT ALL LINES ARE ACTIVE WHEN INSERTED
                        inputLine.Qty = Convert.ToDecimal(qty!);
                        inputLine.Mtlp = detailsDC!.Mtlp ?? Convert.ToDecimal(mtlp!);
                        inputLine.UnitPrice = inputLine.UnitPrice;
                        inputLine.ItemCode = itemCode;
                        inputLine.Vat = Convert.ToDecimal(vat!);
                        inputLine.Margin = margin != null && margin != "" ? Convert.ToDecimal(margin):null;
                        inputLine.UnitTag = unitTag;
                        inputLine.RevNum = revNum;//Convert.ToInt32(revNum);

                        AddQuotationLine(inputLine, detailsDC!,context);

                    }
                    _quotationRepository.Commit();
                }
            }
            catch (ValidationException ex)
            {
                _quotationRepository.RollBack();
                _logger.LogError(ex,JsonConvert.SerializeObject(ex._messages));
                throw;
            }
            catch (Exception ex)
            {
                _quotationRepository.RollBack();
                _logger.LogError(ex, ex.Message);
                throw;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }

        private void ValidateCellValue(string? val ,string col,int index ,bool isNumber,out string? message)
        {
            message = null;
            if (val == null)
            {
                message=(col + " is missing on Index " + index);
            }
            if(isNumber)
            {
                bool isDataType = decimal.TryParse(val, out decimal decimalValue);
                if (!isDataType)
                {
                    message=(col+ " is not a number on Index " + index);
                }
            }
           
        }


        public void ImportData(DataSet ds)
        {
            int num1 = 0;
            try
            {
                int index = ds.Tables.IndexOf("Quotation");
                List<PricingMaster> pricingList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    
                    QMTContext context = _quotationRepository.BeginTransaction();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        num1 = i;
                        if (num1 == 112)
                        {
                            var a = num1;
                        }
                        string quotationNum = dt.Rows[i].Field<string>("QuotationNum");
                        int revNum = 0;
                        string ProjectName = dt.Rows[i].Field<string>("ProjectName");
                        string Customer = dt.Rows[i].Field<string>("Customer");
                        string MSP = dt.Rows[i].Field<string>("ASM");
                        string ASP = dt.Rows[i].Field<string>("AE");
                        string AreaCode = dt.Rows[i].Field<string>("AreaCode");
                        DateTime QuotationDate = dt.Rows[i].Field<DateTime>("QuotationDate");
                        DateTime ExpectedDeliveryDate = dt.Rows[i].Field<DateTime>("ExpectedDeliveryDate");
                        string DeliveryTerm = dt.Rows[i].Field<string>("DeliveryTerms");
                        string PaymentTerm = dt.Rows[i].Field<string>("PaymentTerms");
                        string Status = dt.Rows[i].Field<string>("Status");
                        double Probability = dt.Rows[i].Field<double>("Probability");
                        DateTime BookingDate = dt.Rows[i].Field<DateTime>("BookingDate");
                        //string Industry = dt.Rows[i].Field<string>("Industry");
                        string Currency = dt.Rows[i].Field<string>("Currency");

                        CustomerMaster customerMaster = context.CustomerMasters.Where(x => x.CustomerName.ToLower() == Customer.ToLower()).FirstOrDefault();
                        if(customerMaster == null)
                        {
                           int num =context.CustomerMasters.Where(x => x.CustomerCode.StartsWith("C"+AreaCode)).Count();
                           customerMaster = new CustomerMaster();
                           customerMaster.CustomerCode = "C"+AreaCode + "" + String.Format("{0:0000}", num+1);
                           customerMaster.CustomerName = Customer;
                           context.CustomerMasters.Add(customerMaster);
                            //context.SaveChanges();
                        }
                        QuotationHeader quotationHeader = new();

                        UserMaster mspUser = context.UserMasters.Where(x => x.FirstName.ToLower() + " " + x.LastName.ToLower() == MSP.ToLower()).First();
                        UserMaster aspUser = context.UserMasters.Where(x => x.FirstName.ToLower() + " " + x.LastName.ToLower() == ASP.ToLower()).First();
                        DeliveryTermMaster deliveryTermMaster =context.DeliveryTermMasters.Where(x => x.DeliveryTermName.ToLower() == DeliveryTerm.ToLower()).First();
                        PaymentTermMaster paymentTermMaster =context.PaymentTermMasters.Where(x => x.PaymentTermName == PaymentTerm).First();
                        QuotationStatusMaster statusMaster =context.QuotationStatusMasters.Where(x => x.StatusName.ToLower() == Status.ToLower()).FirstOrDefault();
                        //IndustryMaster industryMaster =context.IndustryMasters.Where(x => x.Name.ToLower() == Industry.ToLower()).First();
                        if(statusMaster == null)
                        {
                            statusMaster = new();
                            int num = context.QuotationStatusMasters.Count();
                            statusMaster.StatusName = Status;
                            statusMaster.StatusId = num+1;
                            context.QuotationStatusMasters.Add(statusMaster);
                        }


                        quotationHeader.QuotationNum = quotationNum;
                        quotationHeader.RevNum = revNum;
                        quotationHeader.Msp = mspUser.Id;
                        quotationHeader.Asp = aspUser.Id;
                        quotationHeader.DeliveryTermId = deliveryTermMaster.Id;
                        quotationHeader.PaymentTermId = paymentTermMaster.Id;
                        quotationHeader.Probability =  (int)Probability;
                        quotationHeader.StatusId = statusMaster.StatusId;
                        quotationHeader.AreaCode = AreaCode;
                        quotationHeader.CustomerCode = customerMaster.CustomerCode;
                        quotationHeader.ProjectName = ProjectName;
                        //quotationHeader.IndustryId = industryMaster.Id;
                        quotationHeader.QuotationDate = QuotationDate;// DateTime.ParseExact(QuotationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        quotationHeader.BookingDate = BookingDate;// DateTime.ParseExact(BookingDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        quotationHeader.ExpectedDeliveryDate = ExpectedDeliveryDate;// DateTime.ParseExact(ExpectedDeliveryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        quotationHeader.IsActiveRevision = true;
                        quotationHeader.CurrencyCode = Currency;


                        context.QuotationHeaders.Add(quotationHeader);
                        context.SaveChanges();
                    }
                    _quotationRepository.Commit();

                }
                //return validationMessages;
            }
            catch (Exception ex)
            {
                _quotationRepository.RollBack();
                _logger.LogError(ex, ex.Message+num1);
                //validationMessages.Add("Error in saving :" + ex.Message);
                //return validationMessages;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }
    }
}
