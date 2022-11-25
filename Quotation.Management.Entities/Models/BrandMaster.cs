using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class BrandMaster
    {
        public BrandMaster()
        {
            SeriesMasters = new HashSet<SeriesMaster>();
        }

        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;

        public virtual ICollection<SeriesMaster> SeriesMasters { get; set; }
    }
}
