using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class QuotationDefaultMultiplier
    {
        public int Id { get; set; }
        public string? BrandName { get; set; }
        public string? ProdName { get; set; }
        public string? ItemCode { get; set; }
        public decimal Mtlp { get; set; }
    }
}
