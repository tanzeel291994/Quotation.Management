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
    public class MastersRepository : IMastersRepository
    {
        public MastersRepository()
        {

        }

        public List<MasterDC> GetAreas()
        {
            using (var context = new QMTContext())
            {
                return context.SalesAreas.Select(x=>new
                MasterDC{
                    Name = x.AreaName,
                    Code = x.AreaCode
                }).ToList();
            }
        }

        public List<MasterDC> GetDeliveryTerms()
        {
            using (var context = new QMTContext())
            {
                return context.DeliveryTermMasters.Select(x => new
                MasterDC
                {
                    Name = x.DeliveryTermName,
                    Id = x.Id
                }).ToList();
            }
        }

        public List<MasterDC> GetPaymentTerms()
        {
            using (var context = new QMTContext())
            {
                return context.PaymentTermMasters.Select(x => new
                MasterDC
                {
                    Name = x.PaymentTermName,
                    Id = x.Id
                }).ToList();
            }
        }

        public List<MasterDC> GetStatuses()
        {
            using (var context = new QMTContext())
            {
                return context.QuotationStatusMasters.Select(x => new
                MasterDC
                {
                    Name = x.StatusName,
                    Id = x.StatusId
                }).ToList();
            }
        }
        public List<MasterDC> GetBrands()
        {
            using (var context = new QMTContext())
            {
                return context.BrandMasters.Select(x => new
                MasterDC
                {
                    Name = x.BrandName,
                    Id = x.BrandId
                }).ToList();
            }
        }

        public List<MasterDC> GetCustomers()
        {
            using (var context = new QMTContext())
            {
                return context.CustomerMasters.Select(x => new
                MasterDC
                {
                    Name = x.CustomerName,
                    Code = x.CustomerCode
                }).ToList();
            }
        }

        public List<MasterDC> GetProjects()
        {
            using (var context = new QMTContext())
            {
                return context.QuotationHeaders.Select(x => new
                MasterDC
                {
                    Name = x.ProjectName,
                    Code = x.ProjectName
                }).Distinct().ToList();
            }
        }

        public List<MasterDC> GetQuotations()
        {
            using (var context = new QMTContext())
            {
                return context.QuotationHeaders.Where(x=>x.IsActiveRevision).Select(x => new
                MasterDC
                {
                    Name = x.QuotationNum,
                    Code = x.QuotationNum
                }).ToList();
            }
        }

        public List<MasterDC> GetUsers()
        {
            using (var context = new QMTContext())
            {
                return context.UserMasters.Select(x => new
                MasterDC
                {
                    Name = x.FirstName+' '+x.LastName,
                    Id = x.Id
                }).ToList();
            }
        }

        public CustomerMaster InsertCustomer(CustomerMaster customerMaster)
        {

            using (var context = new QMTContext())
            {
                context.CustomerMasters.Add(customerMaster);
                context.SaveChanges();
                return customerMaster;
            }
        }
        public List<MasterDC> GetProducts()
        {
            using (var context = new QMTContext())
            {
                return context.ProductMasters.Select(x => new
                MasterDC
                {
                    Name = x.ProdName,
                    Code = x.ProdTypeId
                }).ToList();
            }
        }

        public List<MasterDC> GetCostItems()
        {
            using (var context = new QMTContext())
            {
                return context.CostItemCodes.Select(x => new
                MasterDC
                {
                    Name = x.CostItemName,
                    Code = x.CostItemId
                }).ToList();
            }
        }

        public List<MasterDC> GetCurrency()
        {
            using (var context = new QMTContext())
            {
                return context.CurrencyMasters.Select(x => new
                MasterDC
                {
                    Name = x.CurrencyCode,
                    Code = x.CurrencyCode
                }).ToList();
            }
        }

        public CurrencyMaster? GetCurrencyByCode(string currencyCode)
        {
            using (var context = new QMTContext())
            {
                return context.CurrencyMasters.Where(x => x.CurrencyCode == currencyCode).FirstOrDefault();
            }
        }

        public CostItemCode GetCostItemByCode(string costItemId)
        {
            using (var context = new QMTContext())
            {
                return context.CostItemCodes.Where(x => x.CostItemId == costItemId).First();
            }
        }

        public UserMaster GetUserByUserId (int userId)
        {
            using (var context = new QMTContext())
            {
                return context.UserMasters.Where(x => x.Id == userId).First();
            }
        }

    }
}
