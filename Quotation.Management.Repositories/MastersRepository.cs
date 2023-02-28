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

        public List<MasterDC> GetIndustrys()
        {
            using (var context = new QMTContext())
            {
                return context.IndustryMasters.Select(x => new
                MasterDC
                {
                    Name = x.Name,
                    Id = x.Id
                }).ToList();
            }
        }
        public List<MasterDC> GetAllQuotationYears()
        {
            using (var context = new QMTContext())
            {
                return context.QuotationHeaders.Select(x=> x.QuotationDate.Year).Distinct().Select(x => new
                MasterDC
                {
                    Name = x.ToString(),
                    Id = x
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
        public List<MasterDC> GetCustomers(string searchString)
        {
            using (var context = new QMTContext())
            {
                return context.CustomerMasters.Where(x=>x.CustomerName.ToLower().Contains(searchString.ToLower())).Select(x => new
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

        public UserMaster? GetCurrentUserDetails(string email)
        {
            using (var context = new QMTContext())
            {
                return context.UserMasters.Where(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
            }
        }

        public CustomerMaster InsertCustomer(CustomerMaster customerMaster)
        {
            using (var context = new QMTContext())
            {
                var _customer = context.CustomerMasters.Where(x => x.CustomerName == customerMaster.CustomerName).FirstOrDefault();
                if (_customer != null)
                {
                    throw new ValidationException(new List<string> { "Customer name already exists" });
                }
                int num = context.CustomerMasters.Where(x => x.CustomerCode.StartsWith("C" + customerMaster.CustomerCode)).Count();
                customerMaster.CustomerCode = "C" + customerMaster.CustomerCode + "" + String.Format("{0:0000}", num + 1);
                customerMaster.CustomerName = customerMaster.CustomerName;
                context.CustomerMasters.Add(customerMaster);
                context.SaveChanges();
                return customerMaster;
            }
        }
        public void InsertCostItem(string name)
        {
            using (var context = new QMTContext())
            {
                var _data = context.CostItemCodes.Where(x => x.CostItemName.ToLower() == name.ToLower()).FirstOrDefault();
                if (_data != null)
                {
                    throw new ValidationException(new List<string> { "Cost Item already exists" });
                }
                int num = context.CostItemCodes.Select(x => Convert.ToInt32(x.CostItemId.Replace("C00",""))).Max();
                CostItemCode costItemCode = new();
                costItemCode.CostItemId = "C00" + (num+1);
                costItemCode.CostItemName = name;
                context.CostItemCodes.Add(costItemCode);
                context.SaveChanges();
            }
        }
        public void InsertPaymentTerm(string name)
        {
            using (var context = new QMTContext())
            {
                var _data = context.PaymentTermMasters.Where(x => x.PaymentTermName.ToLower() == name.ToLower()).FirstOrDefault();
                if (_data != null)
                {
                    throw new ValidationException(new List<string> { "PaymentTermName already exists" });
                }
                int num = context.PaymentTermMasters.Select(x => x.Id).Max();
                PaymentTermMaster paymentTermMaster  = new();
                paymentTermMaster.Id = (num + 1);
                paymentTermMaster.PaymentTermName = name;
                context.PaymentTermMasters.Add(paymentTermMaster);
                context.SaveChanges();
            }
        }
        public void InsertDeliveryTerm(string name)
        {
            using (var context = new QMTContext())
            {
                var _data = context.DeliveryTermMasters.Where(x => x.DeliveryTermName.ToLower() == name.ToLower()).FirstOrDefault();
                if (_data != null)
                {
                    throw new ValidationException(new List<string> { "DeliveryTermName already exists" });
                }
                int num = context.DeliveryTermMasters.Select(x => x.Id).Max();
                DeliveryTermMaster deliveryTermMaster = new();
                deliveryTermMaster.Id = (num + 1);
                deliveryTermMaster.DeliveryTermName = name;
                context.DeliveryTermMasters.Add(deliveryTermMaster);
                context.SaveChanges();
            }
        }
        /*public void InsertSalesArea(string name)
        {
            using (var context = new QMTContext())
            {
                var _data = context.SalesAreas.Where(x => x.a.ToLower() == name.ToLower()).FirstOrDefault();
                if (_data != null)
                {
                    throw new ValidationException(new List<string> { "DeliveryTermName already exists" });
                }
                int num = context.DeliveryTermMasters.Select(x => x.Id).Max();
                DeliveryTermMaster deliveryTermMaster = new();
                deliveryTermMaster.Id = (num + 1);
                deliveryTermMaster.DeliveryTermName = name;
                context.DeliveryTermMasters.Add(deliveryTermMaster);
                context.SaveChanges();
            }
        }*/
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

        public List<MasterDC> GetCostItems(QMTContext? _context = null)
        {
            var context = _context ?? new QMTContext();
            List<MasterDC> costItems = context.CostItemCodes.Select(x => new
                MasterDC
                {
                    Name = x.CostItemName,
                    Code = x.CostItemId
                }).ToList();
            if (_context == null)
                context.Dispose();
            return costItems;
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
