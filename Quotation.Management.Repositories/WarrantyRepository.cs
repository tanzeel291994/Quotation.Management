using Quotation.Management.Contracts;
using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class WarrantyRepository : BaseRepository<WarrantyHeader>, IWarrantyRepository
    { 
        public WarrantyRepository()
        {

        }

        public WarrantyHeader InsertUpdateWarranty(WarrantyHeader _warrantyHeader, int userId, QMTContext? _context = null)
        {

            var context = _context ?? new QMTContext();
            var header = context.WarrantyHeaders.Where(x => x.JobDetailsId == _warrantyHeader.JobDetailsId).FirstOrDefault();
            if (header != null)
            {
                foreach (var property in typeof(WarrantyHeader).GetProperties())
                {
                    if (property.CanRead && property.CanWrite && property.Name != "CreatedAt" && property.Name != "UpdatedAt" && property.Name != "CreatedBy")
                    {
                        property.SetValue(header, property.GetValue(_warrantyHeader));
                    }
                }
                header.UpdatedBy = userId;
                header.UpdatedAt = DateTime.Now;
               
            }
            else
            {
                _warrantyHeader.CreatedAt = DateTime.Now.Date;
                _warrantyHeader.CreatedBy = userId;
                context.WarrantyHeaders.Add(_warrantyHeader);
            }
            context.SaveChanges();
            if (_context == null)
            {
                context.Dispose();
            }
            return _warrantyHeader;

        }


        public WarrantyLine InsertUpdateWarrantyLine(WarrantyLine _warrantyLine, QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            var line = context.WarrantyLines.Where(x => x.OurDoreference == _warrantyLine.OurDoreference).FirstOrDefault();
            if (line != null)
            {
                foreach (var property in typeof(WarrantyLine).GetProperties())
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        property.SetValue(line, property.GetValue(_warrantyLine));
                    }
                }
                //line.UpdatedBy = userId;
                //line.UpdatedAt = DateTime.Now;

            }
            else
            {
                //_warrantyHeader.CreatedAt = DateTime.Now.Date;
                //_warrantyHeader.CreatedBy = userId;
                context.WarrantyLines.Add(_warrantyLine);
            }
            context.SaveChanges();
            if (_context == null)
            {
                context.Dispose();
            }
            return _warrantyLine;
        }

        public WarrantyHeader? GetWarranty(int _id)
        {
            using (var context = new QMTContext())
            {
                return context.WarrantyHeaders.Where(x => x.JobDetailsId == _id).FirstOrDefault();
            }
        }
        public List<WarrantyLine> GetWarrantyLines(int _id)
        {
            using (var context = new QMTContext())
            {
                return context.WarrantyLines.Where(x => x.JobDetailsId == _id).ToList();
            }
        }
        public List<string> GetAllJobRefs()
        {
            using (var context = new QMTContext())
            {
                return context.WarrantyHeaders.Select(x => x.JobReference).Distinct().ToList();
            }
        }
        public dynamic GetWarrantySearch(WarrantySearchDC input)
        {
            using (var context = new QMTContext())
            {

                var _data = (from qh in context.WarrantyHeaders
                             join ql in context.WarrantyLines on  qh.JobDetailsId equals  ql.JobDetailsId 
                             where (qh.JobReference == input.JobReference || input.JobReference == null) &&
                            (qh.CustomerCode == input.CustomerCode || input.CustomerCode == null) &&
                            (qh.ClientCode == input.ClientCode || input.ClientCode == null) &&
                            (qh.ConsultantCode == input.ConsultantCode || input.ConsultantCode == null) &&
                            (input.AreaCode.Select(x => x).Contains(qh.AreaCode) || input.AreaCode.Count == 0) &&
                            (input.SalesRepresentativeId.Select(x => x).Contains(qh.SalesRepresentativeId.Value) || input.SalesRepresentativeId.Count == 0) 
                             select new
                             {
                                 JobDetailsId = qh.JobDetailsId,
                                 JobReference = qh.JobReference,
                                 CustomerName = qh.CustomerCodeNavigation.Name,
                                 ConsultantName = qh.ConsultantCodeNavigation != null ? qh.ConsultantCodeNavigation!.Name : "",
                                 ClientName = qh.ClientCodeNavigation != null ? qh.ClientCodeNavigation!.Name : "",
                                 AreaName = qh.AreaCodeNavigation.AreaName,
                                 //OurDoreference = ql.OurDoreference,
                                 CustomersOrderReference = qh.CustomersOrderReference,
                                 PaymentTerms = qh.PaymentTerms.PaymentTermName,
                                 //InvoiceReference = ql.InvoiceReference,
                                 PaymentStatus = qh.PaymentStatus,
                                 //Manufacturer = ql.Manufacturer,
                                 //Model = ql.Model,
                                 //ProductSerialNumber = ql.ProductSerialNumber,
                                 CurrencyCode = qh.WarrantyProvisionCurrency,
                                 //Status = qh.Status.StatusName,
                                 SalesRep = qh.SalesRepresentative.FirstName + ' ' + qh.SalesRepresentative.LastName,
                                 //Remarks = ql.Remarks,
                                 WarrantyProvisionPartsTotal = qh.WarrantyProvisionPartsTotal,
                                 WarrantyProvisionPartsUtilized = qh.WarrantyProvisionPartsUtilized,
                                 WarrantyProvisionPartsReversed = qh.WarrantyProvisionPartsReversed,
                                 WarrantyProvisionPartsBalance = qh.WarrantyProvisionPartsBalance,
                                 WarrantyProvisionLabourTotal = qh.WarrantyProvisionLabourTotal,
                                 WarrantyProvisionLabourUtilized = qh.WarrantyProvisionLabourUtilized,
                                 WarrantyProvisionLabourReversed = qh.WarrantyProvisionLabourReversed,
                                 WarrantyProvisionLabourBalance = qh.WarrantyProvisionLabourBalance,


                                }).ToList();

                return _data;
            }
        }
    }
}
