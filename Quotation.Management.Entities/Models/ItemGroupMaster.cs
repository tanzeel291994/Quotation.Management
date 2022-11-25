using System;
using System.Collections.Generic;

namespace Quotation.Management.Entities.Models
{
    public partial class ItemGroupMaster
    {
        public ItemGroupMaster()
        {
            SeriesMasters = new HashSet<SeriesMaster>();
        }

        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? ProdTypeId { get; set; }

        public virtual ProductMaster? ProdType { get; set; }
        public virtual ICollection<SeriesMaster> SeriesMasters { get; set; }
    }
}
