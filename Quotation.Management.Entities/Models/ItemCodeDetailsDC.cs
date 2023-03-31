using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Entities.Models
{
    public class ItemCodeDetailsDC
    {
        [Key]
        public string ItemCode { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string ProdName { get; set; } = null!;
        public string ProdTypeId { get; set; } = null!;
        public int BrandId { get; set; }
        public string CurrencyCode { get; set; } = null!;
        public decimal CAF { get; set; }
        public decimal? IndexConvFactor { get; set; }
        public decimal? Mtlp { get; set; }
    }
}
