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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class QuotationService : IQuotationService
    {
        private readonly IQuotationRepository _quotationRepository;
        private readonly IProductMasterRepository<ProductMaster> _productMasterRepository;
        private readonly ILogger<QuotationService> _logger;
        public QuotationService(IProductMasterRepository<ProductMaster> productMasterRepository, IQuotationRepository quotationRepository, ILogger<QuotationService> logger)
        {
            _quotationRepository = quotationRepository ?? throw new ArgumentNullException(nameof(quotationRepository));
            _productMasterRepository = productMasterRepository ?? throw new ArgumentNullException(nameof(productMasterRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public QuotationHeader? InsertQuotationHeader(QuotationHeaderDC inputHeader)
        {
            List<string> validationMessages = new List<string>();
            try 
            {
                QuotationHeader header = new QuotationHeader();
                header.RevNum = 0;
                header.QuotationDate = DateTime.Now;
                header.AreaCode = inputHeader.AreaCode;
                header.CurrencyCode = inputHeader.CurrencyCode;
                header.CustomerCode = inputHeader.CustomerCode;
                header.DeliveryTermId = inputHeader.DeliveryTermId;
                header.ExpectedDeliveryDate = inputHeader.ExpectedDeliveryDate;
                header.Msp = inputHeader.Msp;
                header.PaymentTermId = inputHeader.PaymentTermId;
                header.Probability = inputHeader.Probability;
                header.StatusId = inputHeader.StatusId;
                header.ProjectName = inputHeader.ProjectName;
                header.QuotationNum = inputHeader.QuotationNum;

                header = _quotationRepository.InsertQuotation(header);
                return header;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public QuotationLineDC? InsertQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationLine line = new();
               
                line.QuotationNum = inputLine.QuotationNum;
                line.ActiveLine = true;//inputLine.ActiveLine; CHANGE THIS 
                line.Qty = inputLine.Qty;
                line.Mtlp = inputLine.Mtlp;
                line.UnitPrice = inputLine.UnitPrice;
                line.ItemCode = inputLine.ItemCode;

                PricingMaster? pricing = _quotationRepository.GetPricingOptCode(inputLine.ItemCode,"BASIC");
                line.UnitPrice = pricing != null ? (decimal)pricing.Price! : 0;
               
                QuotationLine? latestLine  = _quotationRepository.GetLatestQuotationLine(inputLine.QuotationNum) ;
                QuotationHeader? header = _quotationRepository.GetQuotation(inputLine.QuotationNum,inputLine.RevNum);

                line.LineNum = latestLine != null ? latestLine.LineNum + 1 : 1;
                line.RevNum = header != null ? header.RevNum : 0 ;
                line = _quotationRepository.InsertQuotationLine(line, context);
                inputLine.LineNum = line.LineNum;
                inputLine.UnitPrice = line.UnitPrice;

                if (pricing != null)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = line.RevNum;
                    optCode.LineNum = line.LineNum;
                    optCode.UnitPrice = pricing.Price;
                    optCode.OptCode = pricing.OptCode;
                    optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                }
                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                decimal costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.TtslsPrice = (inputLine.UnitPrice * inputLine.Mtlp * inputLine.Qty) + costItemValue;

                _quotationRepository.Commit();
                return inputLine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                return null;
            }
            finally
            {
                _quotationRepository.DisposeConnection();
            }
        }

        private List<QuotationLine> UpdateAllLinesCostItemValue(string quotatioNum , int revNum,QMTContext context)
        {
            try
            {
                //get all cost items of quotaion and revnum
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(quotatioNum, revNum, context);
                //List<QuotationLineDC> quotationLineDCs = _quotationRepository.GetQuotationLines(quotatioNum, revNum);
                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(quotatioNum, revNum, context);
                List<string> itemCodes = quotationLines.Select(x => x.ItemCode).Distinct().ToList();
                List<ProdItemTotal> itemProdList = _productMasterRepository.GetProductsFromItemCodes(itemCodes);
                Dictionary<string, decimal> prodTotalDict = new Dictionary<string, decimal>();
                foreach (var _line in quotationLines)
                {
                    ProdItemTotal prodItem = itemProdList.Where(x => x.ItemCode == _line.ItemCode).FirstOrDefault();
                    decimal ttslsPrice = _line.Qty * _line.Mtlp * _line.UnitPrice;
                    if (!prodTotalDict.TryAdd(prodItem!.ProdTypeId, ttslsPrice))
                    {
                        prodTotalDict[prodItem!.ProdTypeId] += ttslsPrice; //prodcut wise total value 
                    }
                }
                quotationLines = _quotationRepository.UpdateCostValueOfAllQuotationLine(quotationLines, costItems, prodTotalDict, context);
                return quotationLines;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
                
            }
        }

        public QuotationCostItem? InsertQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem costItem = new();
                costItem.QuotationNum = input.QuotationNum;
                costItem.RevNum = input.RevNum;
                costItem.CostItemType = input.CostItemType;
                costItem.CostItemId = input.CostItemId;
                costItem.CostItemValue = input.CostItemValue;
                costItem.ProdTypeId = input.ProdTypeId;

                costItem = _quotationRepository.InsertQuotationCostItemLine(costItem,context);
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
                return costItem;
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

        public QuotationCostItem? UpdateQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem? costItem = _quotationRepository.GetQuotationCostItem(input.QuotationNum,input.RevNum,input.ProdTypeId,input.CostItemId,context);
                costItem!.CostItemValue = input.CostItemValue;
                costItem = _quotationRepository.UpdateCostItemLine(costItem, context);
                
                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
                return costItem;
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

        public QuotationCostItem? DeleteQuotationCostItem(QuotationCostItemDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationCostItem? costItem = _quotationRepository.GetQuotationCostItem(input.QuotationNum, input.RevNum, input.ProdTypeId, input.CostItemId, context);
                costItem = _quotationRepository.DeleteCostItemLine(costItem!, context);

                UpdateAllLinesCostItemValue(input.QuotationNum, input.RevNum, context);
                _quotationRepository.Commit();
                return costItem;
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

        public bool CopyOptionLine(QuotationCopyOptionDC input)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();

                List<string> copyOptions = _quotationRepository.GetQuotationOptCodes(input.QuotationNum,input.from,input.RevNum).Select(x=>x.OptCode).ToList();
                List<QuotationLineDC> copyToLines = _quotationRepository.GetQuotationLines(input.QuotationNum, input.RevNum, selectedLines: input.to);
                                        
                foreach(var line in copyToLines)
                {
                    List<PricingMaster> pricingMasters = _quotationRepository.GetPricingOptCode(line.ItemCode, copyOptions);
                    if(pricingMasters.Count > 0)
                    {
                        foreach(var pricing in pricingMasters)
                        {
                            QuotationOptCode optCode = new();
                            optCode.QuotationNum = input.QuotationNum;
                            optCode.RevNum = line.RevNum;
                            optCode.LineNum = line.LineNum;
                            optCode.UnitPrice = pricing.Price;
                            optCode.OptCode = pricing.OptCode;
                            optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                        }
                        _quotationRepository.UpdateQuotationOptCodes(input.QuotationNum, line.LineNum, line.RevNum, context);
                    }
                }
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
                QuotationLine? line = _quotationRepository.GetQuotationLine(inputLine.QuotationNum,inputLine.LineNum,inputLine.RevNum);

                List<string> optCodes = inputLine.optCodes.Split(',').ToList();
                decimal unitPrice = line!.UnitPrice + (_quotationRepository.GetSumOfOptPrice(inputLine.ItemCode, optCodes) ?? 0);

                List<PricingMaster> pricingList = _quotationRepository.GetPricingOptCode(inputLine.ItemCode, optCodes);
                
                foreach(var item in pricingList)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = line!.RevNum;
                    optCode.LineNum = line!.LineNum;
                    optCode.UnitPrice = item.Price;
                    optCode.OptCode = item.OptCode;
                    optCode = _quotationRepository.InsertQuotationOptCode(optCode, context);
                }
                line.UnitPrice = unitPrice;
                inputLine.UnitPrice = unitPrice;

                _quotationRepository.UpdateQuotationLine(line, context);
                UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                decimal costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.TtslsPrice = (inputLine.UnitPrice * inputLine.Mtlp * inputLine.Qty) + costItemValue;

                _quotationRepository.Commit();
                return inputLine;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                _quotationRepository.RollBack();
                return null;
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
                QuotationLine? line = _quotationRepository.GetQuotationLine(inputLine.QuotationNum, inputLine.LineNum, inputLine.RevNum);

                List<string> optCodes = inputLine.optCodes.Split(',').ToList();
                decimal unitPrice = line!.UnitPrice - (_quotationRepository.GetSumOfOptPrice(inputLine.ItemCode, optCodes) ?? 0);

                List<PricingMaster> pricingList = _quotationRepository.GetPricingOptCode(inputLine.ItemCode, optCodes);

                foreach (var item in pricingList)
                {
                    QuotationOptCode optCode = new();
                    optCode.QuotationNum = inputLine.QuotationNum;
                    optCode.RevNum = line!.RevNum;
                    optCode.LineNum = line!.LineNum;
                    optCode.UnitPrice = item.Price;
                    optCode.OptCode = item.OptCode;
                    _quotationRepository.RemoveQuotationOptCode (optCode, context);
                }
                line.UnitPrice = unitPrice;
                inputLine.UnitPrice = unitPrice;

                _quotationRepository.UpdateQuotationLine(line, context);

                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                decimal costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.TtslsPrice = (inputLine.UnitPrice * inputLine.Mtlp * inputLine.Qty) + costItemValue;


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


        public QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationLine line = new();

                line.QuotationNum = inputLine.QuotationNum;
                line.ActiveLine = true;//inputLine.ActiveLine; CHANGE THIS 
                line.Qty = inputLine.Qty;
                line.Mtlp = inputLine.Mtlp;
                line.UnitPrice = inputLine.UnitPrice;
                line.ItemCode = inputLine.ItemCode;
                line.LineNum = inputLine.LineNum;
                line.RevNum = inputLine.RevNum;

                _quotationRepository.UpdateQuotationLine(line, context);

                List<QuotationLine> lines = UpdateAllLinesCostItemValue(inputLine.QuotationNum, inputLine.RevNum, context);
                decimal costItemValue = lines.Where(x => x.LineNum == line.LineNum).Select(x => x.CostItemLineValue).FirstOrDefault() ?? 0;
                inputLine.TtslsPrice = (inputLine.UnitPrice * inputLine.Mtlp * inputLine.Qty) + costItemValue;

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

        public bool UpdateQuotationCurrency(CurrencyDC currencyDC)
        {
            try
            {
                QMTContext context = _quotationRepository.BeginTransaction();
                QuotationHeader? quotationHeader = _quotationRepository.GetQuotation(currencyDC.QuotationNum,currencyDC.RevNum,context);
                quotationHeader!.CurrencyCode = currencyDC.CurrencyCode;
                if(currencyDC.NewConvFactor != null)
                    quotationHeader!.ConvFactor = currencyDC.NewConvFactor;

                quotationHeader = _quotationRepository.UpdateQuotationHeader(quotationHeader, context);
                List<QuotationLine> quotationLines = _quotationRepository.GetQuotationLines(currencyDC.QuotationNum, currencyDC.RevNum, context);

                foreach (var _line in quotationLines)
                {
                    _line.UnitPrice = (decimal)(currencyDC.NewConvFactor ?? currencyDC.ConvFactor) * _line.UnitPrice;
                    _line.CostItemLineValue = (decimal)(currencyDC.NewConvFactor ?? currencyDC.ConvFactor) * _line.CostItemLineValue;
                }
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(currencyDC.QuotationNum,currencyDC.RevNum,context);
                foreach (var _costItem in costItems)
                {
                   if(_costItem.CostItemType == CostItemType.ByVal.ToString())
                   {
                        _costItem.CostItemValue = _costItem.CostItemValue * currencyDC.NewConvFactor;
                        _quotationRepository.UpdateCostItemLine(_costItem,context);
                   }
                }
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


        public JObject? GetQuotation(string Id, int? revNum=null)
        {
            JObject jobject = new();
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id);
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLines(Id, header!.RevNum);
                List<QuotationCostItem> costItems = _quotationRepository.GetQuotationCostItems(Id, header!.RevNum);
                jobject.Add(new JProperty("header", JsonConvert.SerializeObject(header!, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })));
                jobject.Add(new JProperty("lines", JsonConvert.SerializeObject(lines, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })));
                jobject.Add(new JProperty("costItems", JsonConvert.SerializeObject(costItems, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })));

                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

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

        public List<QuotationLineDC>? GetQuotationLinesOptCodes(string Id,int revNum)
        {
            JObject jobject = new();
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id, revNum);
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLines(Id, header!.RevNum);
                lines = _quotationRepository.GetQuotationLinesOptions(lines);
                return lines;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public JObject? GetQuotationOptCodes(QuotationLineDC quotationLineDC)
        {
            JObject jobject = new();
            try
            {
                List<QuotationOptCode> optCodeList = _quotationRepository.GetQuotationOptCodes(quotationLineDC.QuotationNum,quotationLineDC.LineNum,quotationLineDC.RevNum);
                jobject.Add("selectedOptons", JsonConvert.SerializeObject(optCodeList));
                jobject.Add("allOptions", JsonConvert.SerializeObject(_quotationRepository.GetItemOptions(quotationLineDC.ItemCode)));
                
                return jobject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }


    }
}
