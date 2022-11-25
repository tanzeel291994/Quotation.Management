using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class SeriesMaster
    {
        public SeriesMaster()
        {
            ItemMasters = new HashSet<ItemMaster>();
        }

        public int SeriesId { get; set; }
        public string? SeriesName { get; set; }
        public int? GroupId { get; set; }
        public int? BrandId { get; set; }

        public virtual BrandMaster? Brand { get; set; }
        public virtual ItemGroupMaster? Group { get; set; }
        public virtual ICollection<ItemMaster> ItemMasters { get; set; }
    }
}
