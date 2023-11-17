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
