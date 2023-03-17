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
                    Name = x.Name,
                    Code = x.Code
                }).ToList();
            }
        }
        public List<MasterDC> GetBuyers(int type)
        {
            using (var context = new QMTContext())
            {
                return context.CustomerMasters.Where(x=> x.Type == type).Select(x => new
                MasterDC
                {
                    Name = x.Name,
                    Code = x.Code
                }).ToList();
            }
        }
        public List<MasterDC> GetCustomers(string searchString)
        {
            using (var context = new QMTContext())
            {
                return context.CustomerMasters.Where(x=>x.Code.ToLower().Contains(searchString.ToLower()) && x.Type == (int)MasterEnum.CUSTOMER ).Select(x => new
                MasterDC
                {
                    Name = x.Name,
                    Code = x.Code
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

        public void InsertCustomer(string code, string name , int type)
        {
            using (var context = new QMTContext())
            {
                var _data= context.CustomerMasters.Where(x => x.Name.ToLower() == name.ToLower() && x.Type == type).FirstOrDefault();
                if (_data != null)
                {
                    throw new ValidationException(new List<string> { "Name already exists" });
                }
                string prefix = "";
                switch ((MasterEnum)type)
                {
                    case MasterEnum.CUSTOMER:
                        prefix = "C";
                        break;
                    case MasterEnum.CLIENT:
                        prefix = "CL";
                        break;
                    case MasterEnum.CONSULTANT:
                        prefix = "CNS";
                        break;
                }
                int num = context.CustomerMasters.Where(x => x.Code.StartsWith(prefix + code)).Count(); //here _data.customerCode is areaCode
                
                CustomerMaster toBeInserted = new();
                toBeInserted.Code = prefix + code + "" + String.Format("{0:0000}", num + 1);
                toBeInserted.Name = name;
                toBeInserted.Type = type;
                context.CustomerMasters.Add(toBeInserted);
                context.SaveChanges();
            }
        }

        public void InsertMaster(string code, string name, MasterEnum type)
        {
            using (var context = new QMTContext())
            {
                if(type == MasterEnum.STATUS)
                {
                    var _data = context.QuotationStatusMasters.Where(x => x.StatusName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "Status already exists" });
                    }
                    int num = context.QuotationStatusMasters.Select(x => x.StatusId).Max();
                    QuotationStatusMaster tobeInserted = new();
                    tobeInserted.StatusId = (num + 1);
                    tobeInserted.StatusName = name;
                    context.QuotationStatusMasters.Add(tobeInserted);
                }
                else if (type == MasterEnum.DELIVERY_TERM)
                {
                    var _data = context.DeliveryTermMasters.Where(x => x.DeliveryTermName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "Delivery Term already exists" });
                    }
                    int num = context.DeliveryTermMasters.Select(x => x.Id).Max();
                    DeliveryTermMaster tobeInserted = new();
                    tobeInserted.Id = (num + 1);
                    tobeInserted.DeliveryTermName = name;
                    context.DeliveryTermMasters.Add(tobeInserted);
                }
                else if (type == MasterEnum.SALES_AREA)
                {
                    var _data = context.SalesAreas.Where(x => x.AreaName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "Sales area already exists" });
                    }
                    SalesArea tobeInserted = new();
                    tobeInserted.AreaCode = code;
                    tobeInserted.AreaName = name;
                    context.SalesAreas.Add(tobeInserted);
                }
                else if (type == MasterEnum.PAYMENT_TERM)
                {
                    var _data = context.PaymentTermMasters.Where(x => x.PaymentTermName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "PaymentTermName already exists" });
                    }
                    int num = context.DeliveryTermMasters.Select(x => x.Id).Max();
                    PaymentTermMaster tobeInserted = new();
                    tobeInserted.Id = (num + 1);
                    tobeInserted.PaymentTermName = name;
                    context.PaymentTermMasters.Add(tobeInserted);
                }
                else if (type == MasterEnum.INDUSTRY)
                {
                    var _data = context.IndustryMasters.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "Industry already exists" });
                    }
                    int num = context.IndustryMasters.Select(x => x.Id).Max();
                    IndustryMaster tobeInserted = new();
                    tobeInserted.Id = (num + 1);
                    tobeInserted.Name = name;
                    context.IndustryMasters.Add(tobeInserted);
                }
                else if (type == MasterEnum.COSTITEMS)
                {
                    var _data = context.CostItemCodes.Where(x => x.CostItemName.ToLower() == name.ToLower()).FirstOrDefault();
                    if (_data != null)
                    {
                        throw new ValidationException(new List<string> { "Cost Item already exists" });
                    }
                    int num = context.CostItemCodes.Select(x => Convert.ToInt32(x.CostItemId.Replace("C00", ""))).Max();
                    CostItemCode costItemCode = new();
                    costItemCode.CostItemId = "C00" + (num + 1);
                    costItemCode.CostItemName = name;
                    context.CostItemCodes.Add(costItemCode);
                }

                context.SaveChanges();
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
