using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class WarrantyLine
    {
        public string OurDoreference { get; set; } = null!;
        public int? JobDetailsId { get; set; }
        public DateTime? Dodate { get; set; }
        public string? InvoiceReference { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string? Product { get; set; }
        public string? Manufacturer { get; set; }
        public string? Model { get; set; }
        public string? ProductSerialNumber { get; set; }
        public DateTime? CommissioningDate { get; set; }
        public string? WarrantyCommitment { get; set; }
        public DateTime? WarrantyPeriodUnitStartDate { get; set; }
        public DateTime? WarrantyPeriodUnitEndDate { get; set; }
        public DateTime? WarrantyPeriodComponentsStartDate { get; set; }
        public DateTime? WarrantyPeriodComponentsEndDate { get; set; }
        public string? ManufacturersOrderReference { get; set; }
        public string? ManufacturersInvoiceReference { get; set; }
        public DateTime? ManufacturersInvoiceDate { get; set; }
        public DateTime? ManufacturersWarrantyPeriodUnitStartDate { get; set; }
        public DateTime? ManufacturersWarrantyPeriodUnitEndDate { get; set; }
        public DateTime? ManufacturersWarrantyPeriodComponentsStartDate { get; set; }
        public DateTime? ManufacturersWarrantyPeriodComponentsEndDate { get; set; }
        public string? Remarks { get; set; }

        public virtual WarrantyHeader? JobDetails { get; set; }
    }
}
