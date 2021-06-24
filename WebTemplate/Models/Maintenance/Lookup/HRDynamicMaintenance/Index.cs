using System.Collections.Generic;

namespace WebTemplate.Models.Maintenance.Lookup.HRDynamicMaintenance
{
    public class Index
    {
        public List<ProcessLayer.Entities.Lookup> ItemList { get; set; }
        public int ItemCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int GridCount { get; } = Properties.Settings.Default.GridCount;
        public string Filter { get; set; }
        public string TableName { get; set; }
    }
}