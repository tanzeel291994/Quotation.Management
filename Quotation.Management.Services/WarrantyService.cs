using DocumentFormat.OpenXml.Spreadsheet;
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
    public class WarrantyService  : IWarrantyService
    {
        private readonly IWarrantyRepository _warrantyRepository;
        private readonly ILogger<WarrantyService> _logger;
        public WarrantyService(IWarrantyRepository warrantyRepository, ILogger<WarrantyService> logger)
        {
            _warrantyRepository = warrantyRepository ?? throw new ArgumentNullException(nameof(warrantyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        #region Header
        public WarrantyHeader? InsertWarrantyHeader(WarrantyHeaderDC inputHeader)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                WarrantyHeader header = new ();
                header.JobDetailsId = inputHeader.JobDetailsId;
                header.ProjectName = inputHeader.ProjectName;
                header.SalesOrderReference = inputHeader.SalesOrderReference;
                header.ClientCode = inputHeader.ClientCode;
                header.ConsultantCode = inputHeader.ConsultantCode;
                header.CustomerCode = inputHeader.CustomerCode;
                header.CustomersOrderReference = inputHeader.CustomersOrderReference;
                header.PaymentTermsId = inputHeader.PaymentTermsId;
                header.PaymentStatus = inputHeader.PaymentStatus;
                header.AreaCode = inputHeader.AreaCode;
                header.SalesRepresentativeId = inputHeader.SalesRepresentativeId;
                header.WarrantyProvisionCurrency = inputHeader.WarrantyProvisionCurrency;
                header.WarrantyProvisionPartsTotal = inputHeader.WarrantyProvisionPartsTotal;
                header.WarrantyProvisionPartsUtilized = inputHeader.WarrantyProvisionPartsUtilized;
                header.WarrantyProvisionPartsBalance = inputHeader.WarrantyProvisionPartsBalance;
                header.WarrantyProvisionPartsReversed = inputHeader.WarrantyProvisionPartsReversed;
                header.WarrantyProvisionLabourTotal = inputHeader.WarrantyProvisionLabourTotal;
                header.WarrantyProvisionLabourUtilized = inputHeader.WarrantyProvisionLabourUtilized;
                header.WarrantyProvisionLabourReversed = inputHeader.WarrantyProvisionLabourReversed;
                header.WarrantyProvisionLabourBalance = inputHeader.WarrantyProvisionLabourBalance;

                header = _warrantyRepository.InsertUpdateWarranty(header, inputHeader.UserId);
                return header;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        #endregion

        #region Lines
        public void InsertWarrantyLine(WarrantyLineDC inputLine)
        {
            try
            {
                WarrantyLine warrantyLine = new();
                warrantyLine.OurDoreference = inputLine.OurDOReference;
                warrantyLine.JobDetailsId = inputLine.JobDetailsId;
                warrantyLine.Dodate = inputLine.DoDate;
                warrantyLine.InvoiceReference = inputLine.InvoiceReference;
                warrantyLine.InvoiceDate = inputLine.InvoiceDate;
                warrantyLine.Product = inputLine.Product;
                warrantyLine.Manufacturer = inputLine.Manufacturer;
                warrantyLine.Model = inputLine.Model;
                warrantyLine.ProductSerialNumber = inputLine.ProductSerialNumber;
                warrantyLine.CommissioningDate = inputLine.CommissioningDate;
                warrantyLine.WarrantyCommitment = inputLine.WarrantyCommitment;
                warrantyLine.WarrantyPeriodUnitStartDate = inputLine.WarrantyPeriodUnitStartDate;
                warrantyLine.WarrantyPeriodUnitEndDate = inputLine.WarrantyPeriodUnitEndDate;
                warrantyLine.WarrantyPeriodComponentsStartDate = inputLine.WarrantyPeriodComponentsStartDate;
                warrantyLine.WarrantyPeriodComponentsEndDate = inputLine.WarrantyPeriodComponentsEndDate;
                warrantyLine.ManufacturersOrderReference = inputLine.ManufacturersOrderReference;
                warrantyLine.ManufacturersInvoiceReference = inputLine.ManufacturersInvoiceReference;
                warrantyLine.ManufacturersInvoiceDate = inputLine.ManufacturersInvoiceDate;
                warrantyLine.ManufacturersWarrantyPeriodUnitStartDate = inputLine.ManufacturersWarrantyPeriodUnitStartDate;
                warrantyLine.ManufacturersWarrantyPeriodUnitEndDate = inputLine.ManufacturersWarrantyPeriodUnitEndDate;
                warrantyLine.ManufacturersWarrantyPeriodComponentsStartDate = inputLine.ManufacturersWarrantyPeriodComponentsStartDate;
                warrantyLine.ManufacturersWarrantyPeriodComponentsEndDate = inputLine.ManufacturersWarrantyPeriodComponentsEndDate;
                warrantyLine.Remarks = inputLine.Remarks;


                _warrantyRepository.InsertUpdateWarrantyLine(warrantyLine);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        #endregion

        public List<string> ImportWarrantyData(DataSet ds,int userId)
        {
            List<string> validationMessages = new List<string>();
            try
            {
                int index = ds.Tables.IndexOf("Header");
                int lineIndex = ds.Tables.IndexOf("Lines");

                List<PricingMaster> pricingList = new();
                if (index != -1)
                {
                    DataTable dt = ds.Tables[index];
                    //string[] columnNames = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                    //List<string> itemCodes = new();
                    //List<string> validItemCodes = new();
                    //foreach (var columnName in columnNames)
                    //    if (columnName != "OptCode" && columnName != "OptName")
                     //       itemCodes.Add(columnName);
                    //List<string> messages = new();
                   // if (messages.Count > 0) return messages;

                    //itemCodes = itemCodes.Where(x => validItemCodes.Contains(x)).ToList();
                    QMTContext context = _warrantyRepository.BeginTransaction();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string? jobDetailsId = dt.Rows[i].Field<string>("JobDetailsId");
                        string? projectName = dt.Rows[i].Field<string>("ProjectName");
                        string? salesOrderReference = dt.Rows[i].Field<string>("SalesOrderReference");
                        string? clientName = dt.Rows[i].Field<string>("ClientName");
                        string? consultantName = dt.Rows[i].Field<string>("ConsultantName");
                        string? customerName = dt.Rows[i].Field<string>("CustomerName");
                        string? customersOrderReference = dt.Rows[i].Field<string>("CustomersOrderReference");
                        string? paymentTerms = dt.Rows[i].Field<string>("PaymentTerms");
                        string? paymentStatus = dt.Rows[i].Field<string>("PaymentStatus");
                        string? areaCode = dt.Rows[i].Field<string>("AreaCode");
                        string? salesRepresentativeName = dt.Rows[i].Field<string>("SalesRepresentativeName");
                        string? warrantyProvisionCurrency = dt.Rows[i].Field<string>("WarrantyProvisionCurrency");
                        double? warrantyProvisionPartsTotal = dt.Rows[i].Field<double?>("WarrantyProvisionPartsTotal");
                        double? warrantyProvisionPartsUtilized = dt.Rows[i].Field<double?>("WarrantyProvisionPartsUtilized");
                        double? warrantyProvisionPartsBalance = dt.Rows[i].Field<double?>("WarrantyProvisionPartsBalance");
                        double? warrantyProvisionPartsReversed = dt.Rows[i].Field<double?>("WarrantyProvisionPartsReversed");
                        double? warrantyProvisionLabourTotal = dt.Rows[i].Field<double?>("WarrantyProvisionLabourTotal");
                        double? warrantyProvisionLabourUtilized = dt.Rows[i].Field<double?>("WarrantyProvisionLabourUtilized");
                        double? warrantyProvisionLabourReversed = dt.Rows[i].Field<double?>("WarrantyProvisionLabourReversed");
                        double? warrantyProvisionLabourBalance = dt.Rows[i].Field<double?>("WarrantyProvisionLabourBalance");


                        SalesArea? salesArea = !string.IsNullOrEmpty(areaCode)
                           ? context.SalesAreas.FirstOrDefault(x => x.AreaCode.ToLower() == areaCode.ToLower())
                           : null;

                        UserMaster? aspUser = !string.IsNullOrEmpty(salesRepresentativeName)
                            ? context.UserMasters.FirstOrDefault(x => (x.FirstName + " " + x.LastName).ToLower() == salesRepresentativeName.ToLower())
                            : null;

                        CustomerMaster? client = !string.IsNullOrEmpty(clientName)
                            ? context.CustomerMasters.FirstOrDefault(x => x.Type == 5 && x.Name.ToLower() == clientName.ToLower())
                            : null;

                        CustomerMaster? customer = !string.IsNullOrEmpty(customerName)
                            ? context.CustomerMasters.FirstOrDefault(x => x.Type == 4 && x.Name.ToLower() == customerName.ToLower())
                            : null;

                        CustomerMaster? consultant = !string.IsNullOrEmpty(consultantName)
                            ? context.CustomerMasters.FirstOrDefault(x => x.Type == 6 && x.Name.ToLower() == consultantName.ToLower())
                            : null;

                        PaymentTermMaster? paymentTerm = !string.IsNullOrEmpty(paymentTerms)
                            ? context.PaymentTermMasters.FirstOrDefault(x => x.PaymentTermName.ToLower() == paymentTerms.ToLower())
                            : null;
                        if(paymentTerm == null && (paymentTerms != null || paymentTerms != ""))
                        {
                            validationMessages.Add($"Payments term {paymentTerms} not found in master db missing on Index :{i + 1}");
                            continue;
                        }
                        if (consultant == null && (consultantName != null || consultantName != ""))
                        {
                            validationMessages.Add($"Consultant Name {consultantName} not found in master db missing on Index :{i + 1}");
                            continue;
                        }
                        if (customer == null && (customerName != null || customerName != ""))
                        {
                            validationMessages.Add($"Customer Name {customerName} not found in master db missing on Index :{i + 1}");
                            continue;
                        }
                        if (client == null && (clientName != null || clientName != ""))
                        {
                            validationMessages.Add($"Client Name {clientName} not found in master db missing on Index :{i + 1}");
                            continue;
                        }
                        if (salesArea == null && (areaCode != null || areaCode != ""))
                        {
                            validationMessages.Add($"Area Code {areaCode} not found in master db missing on Index :{i + 1}");
                            continue;
                        }
                        if (aspUser == null && (salesRepresentativeName != null || salesRepresentativeName != ""))
                        {
                            validationMessages.Add($"SR name {salesRepresentativeName} not found in master db missing on Index :{i+1}");
                            continue;
                        }
                        if (jobDetailsId == null)
                        {
                            validationMessages.Add($"JobDetailsId missing on Index :{i + 1}");
                            continue;
                        }

                        if (projectName == null)
                        {
                            validationMessages.Add($"ProjectName missing on Index :{i + 1}");
                            continue;
                        }
                        WarrantyHeader? warrantyHeader = context.WarrantyHeaders.Where(x => x.JobDetailsId.ToLower() == jobDetailsId.ToLower()).FirstOrDefault();

                        if(warrantyHeader != null)
                        {
                            validationMessages.Add($"JobDetailsId : {jobDetailsId} already exist in db from Index:" + i);
                            continue;
                        }
                        
                        WarrantyHeader header = new();
                        header.JobDetailsId = jobDetailsId;
                        header.ProjectName = projectName;
                        header.SalesOrderReference = salesOrderReference;
                        header.ClientCode = client != null  ? client.Code : null;
                        header.ConsultantCode = consultant != null ? consultant.Code : null ;
                        header.CustomerCode = customer != null ? customer.Code : null; 
                        header.CustomersOrderReference = customersOrderReference;
                        header.PaymentTermsId = paymentTerm != null ? paymentTerm.Id : null ;
                        header.PaymentStatus = paymentStatus;
                        header.AreaCode = salesArea != null ? salesArea.AreaCode : null; ;
                        header.SalesRepresentativeId = aspUser != null ? aspUser.Id : null ;
                        header.WarrantyProvisionCurrency = warrantyProvisionCurrency;
                        header.WarrantyProvisionPartsTotal = (decimal?)warrantyProvisionPartsTotal;
                        header.WarrantyProvisionPartsUtilized = (decimal?)warrantyProvisionPartsUtilized;
                        header.WarrantyProvisionPartsBalance = (decimal?)warrantyProvisionPartsBalance;
                        header.WarrantyProvisionPartsReversed = (decimal?)warrantyProvisionPartsReversed;
                        header.WarrantyProvisionLabourTotal = (decimal?)warrantyProvisionLabourTotal;
                        header.WarrantyProvisionLabourUtilized = (decimal?)warrantyProvisionLabourUtilized;
                        header.WarrantyProvisionLabourReversed = (decimal?)warrantyProvisionLabourReversed;
                        header.WarrantyProvisionLabourBalance = (decimal?)warrantyProvisionLabourBalance;

                        header = _warrantyRepository.InsertUpdateWarranty(header, userId, context);
                    }


                    if(validationMessages.Count == 0)
                    {
                        if (lineIndex != -1)
                        {
                            DataTable dt1 = ds.Tables[lineIndex];
                            DateTime? ParseDateTime(DataRow row, string columnName)
                            {
                                if (row[columnName] != DBNull.Value)
                                {
                                    if (DateTime.TryParse(row[columnName].ToString(), out DateTime result))
                                    {
                                        return result;
                                    }
                                    validationMessages.Add($"Unable to parse {columnName}");
                                }
                                return null;
                            }

                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                WarrantyLine warrantyLine = new();
                                warrantyLine.OurDoreference = dt1.Rows[i].Field<string>("OurDOReference");
                                warrantyLine.JobDetailsId = dt1.Rows[i].Field<string>("JobDetailsId");
                                warrantyLine.Dodate = ParseDateTime(dt1.Rows[i], "DODate");
                                warrantyLine.InvoiceReference = dt1.Rows[i].Field<string>("InvoiceReference");
                                warrantyLine.InvoiceDate = ParseDateTime(dt1.Rows[i], "InvoiceDate");
                                warrantyLine.Product = dt1.Rows[i].Field<string>("Product");
                                warrantyLine.Manufacturer = dt1.Rows[i].Field<string>("Manufacturer");
                                warrantyLine.Model = dt1.Rows[i].Field<string>("Model");
                                warrantyLine.ProductSerialNumber = dt1.Rows[i].Field<string>("ProductSerialNumber");
                                warrantyLine.CommissioningDate = ParseDateTime(dt1.Rows[i], "CommissioningDate");
                                warrantyLine.WarrantyCommitment = dt1.Rows[i].Field<string>("WarrantyCommitment");
                                warrantyLine.WarrantyPeriodUnitStartDate = ParseDateTime(dt1.Rows[i], "WarrantyPeriodUnitStartDate");
                                warrantyLine.WarrantyPeriodUnitEndDate = ParseDateTime(dt1.Rows[i], "WarrantyPeriodUnitEndDate");
                                warrantyLine.WarrantyPeriodComponentsStartDate = ParseDateTime(dt1.Rows[i], "WarrantyPeriodComponentsStartDate");
                                warrantyLine.WarrantyPeriodComponentsEndDate = ParseDateTime(dt1.Rows[i], "WarrantyPeriodComponentsEndDate");
                                warrantyLine.ManufacturersOrderReference = dt1.Rows[i].Field<string>("ManufacturersOrderReference");
                                warrantyLine.ManufacturersInvoiceReference = dt1.Rows[i].Field<string>("ManufacturersInvoiceReference");
                                warrantyLine.ManufacturersInvoiceDate = ParseDateTime(dt1.Rows[i], "ManufacturersInvoiceDate");
                                warrantyLine.ManufacturersWarrantyPeriodUnitStartDate = ParseDateTime(dt1.Rows[i], "ManufacturersWarrantyPeriodUnitStartDate");
                                warrantyLine.ManufacturersWarrantyPeriodUnitEndDate = ParseDateTime(dt1.Rows[i], "ManufacturersWarrantyPeriodUnitEndDate");
                                warrantyLine.ManufacturersWarrantyPeriodComponentsStartDate = ParseDateTime(dt1.Rows[i], "ManufacturersWarrantyPeriodComponentsStartDate");
                                warrantyLine.ManufacturersWarrantyPeriodComponentsEndDate = ParseDateTime(dt1.Rows[i], "ManufacturersWarrantyPeriodComponentsEndDate");
                                warrantyLine.Remarks = dt1.Rows[i].Field<string>("Remarks");

                                if(warrantyLine.JobDetailsId  == null || warrantyLine.JobDetailsId == "")
                                {
                                    validationMessages.Add($"JobDetailsId missing on Index  in Lines:{i + 1}");
                                    continue;
                                }
                                if (warrantyLine.OurDoreference == null || warrantyLine.OurDoreference== "")
                                {
                                    validationMessages.Add($"OurDoreference missing on Index  in Lines:{i + 1}");
                                    continue;
                                }
                                var header = context.WarrantyHeaders.Where(x => x.JobDetailsId.ToLower() == warrantyLine.JobDetailsId.ToLower()).FirstOrDefault();
                                if(header == null)
                                {
                                    validationMessages.Add($"JobDetailsId doesnt exist  on Index :{i+1}");
                                    continue;
                                }

                                _warrantyRepository.InsertUpdateWarrantyLine(warrantyLine, context);

                            }


                        }
                        }
                    }
                if (validationMessages.Count == 0)
                    _warrantyRepository.Commit();
                else
                    _warrantyRepository.RollBack();

                return validationMessages;
            }
            catch (Exception ex)
            {
                _warrantyRepository.RollBack();
                _logger.LogError(ex, ex.Message);
                validationMessages.Add("Error in saving :" + ex.Message);
                return validationMessages;
            }
            finally
            {
                _warrantyRepository.DisposeConnection();
            }
        }

      
        public dynamic? GetWarranty(string Id)
        {
            JObject jobject = new();
            try
            {
                WarrantyHeader? header = _warrantyRepository.GetWarranty(Id);
                List<WarrantyLine> lines = _warrantyRepository.GetWarrantyLines(Id);

                jobject.Add(new JProperty("header", JsonConvert.SerializeObject(header, new JsonSerializerSettings
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
                throw;
            }
        }

        public dynamic SearchQuotations(WarrantySearchDC warrantySearch)
        {
            JObject jobject = new();
            try
            {
                dynamic result = _warrantyRepository.GetWarrantySearch(warrantySearch);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        public List<string> GetAllJobRefs()
        {
            try
            {
                var data = _warrantyRepository.GetAllJobRefs();
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
