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
        private readonly ILogger<QuotationService> _logger;
        public QuotationService(IQuotationRepository quotationRepository, ILogger<QuotationService> logger)
        {
            _quotationRepository = quotationRepository ?? throw new ArgumentNullException(nameof(quotationRepository));
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
                inputLine.TtslsPrice = inputLine.Qty * line.UnitPrice * inputLine.Mtlp;
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
                inputLine.TtslsPrice = unitPrice * line.Mtlp;
                _quotationRepository.UpdateQuotationLine(line, context);
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
                inputLine.TtslsPrice = unitPrice * line.Mtlp;
                _quotationRepository.UpdateQuotationLine(line, context);
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


        public QuotationLineDC? UpdateQuotationLine(QuotationLineDC inputLine)
        {
            try
            {
                QuotationLine line = new();

                line.QuotationNum = inputLine.QuotationNum;
                line.ActiveLine = true;//inputLine.ActiveLine; CHANGE THIS 
                line.Qty = inputLine.Qty;
                line.Mtlp = inputLine.Mtlp;
                line.UnitPrice = inputLine.UnitPrice;
                line.ItemCode = inputLine.ItemCode;
                line.LineNum = inputLine.LineNum;
                line.RevNum = inputLine.RevNum;
                

                var updatedQuotationLine = _quotationRepository.UpdateQuotationLine(line);
                if (updatedQuotationLine != null)
                {
                    inputLine.TtslsPrice = inputLine.Qty * line.UnitPrice * inputLine.Mtlp;
                    return inputLine;
                }
                else
                {
                    return null;
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return null;
            }
        }


        public JObject? GetQuotation(string Id, int revNum)
        {
            JObject jobject = new();
            try
            {
                QuotationHeader? header = _quotationRepository.GetQuotation(Id, revNum);
                List<QuotationLineDC> lines = _quotationRepository.GetQuotationLines(Id, revNum);
                jobject.Add(new JProperty("header", JsonConvert.SerializeObject(header!, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                })));
                jobject.Add(new JProperty("lines", JsonConvert.SerializeObject(lines, new JsonSerializerSettings
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
