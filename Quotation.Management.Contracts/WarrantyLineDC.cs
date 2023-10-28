using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts
{
    public class WarrantyLineDC
    {
        public string OurDOReference { get; set; } // Primary key
        public int JobDetailsId { get; set; }
        public DateTime DoDate { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Product { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string ProductSerialNumber { get; set; }
        public DateTime CommissioningDate { get; set; }
        public string WarrantyCommitment { get; set; }
        public DateTime WarrantyPeriodUnitStartDate { get; set; }
        public DateTime WarrantyPeriodUnitEndDate { get; set; }
        public DateTime WarrantyPeriodComponentsStartDate { get; set; }
        public DateTime WarrantyPeriodComponentsEndDate { get; set; }
        public string ManufacturersOrderReference { get; set; }
        public string ManufacturersInvoiceReference { get; set; }
        public DateTime ManufacturersInvoiceDate { get; set; }
        public DateTime ManufacturersWarrantyPeriodUnitStartDate { get; set; }
        public DateTime ManufacturersWarrantyPeriodUnitEndDate { get; set; }
        public DateTime ManufacturersWarrantyPeriodComponentsStartDate { get; set; }
        public DateTime ManufacturersWarrantyPeriodComponentsEndDate { get; set; }
        public string Remarks { get; set; }

        public int? UserId { get; set; }

    }
}
