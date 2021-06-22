using System.Collections.Generic;

namespace WebTemplate.Models.Maintenance.Organization.Designation
{
    public class Index
    {
        public List<DataAccessLayer.Organization.Designation> ItemList { get; set; }
        public int ItemCount { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; } = Properties.Settings.Default.GridCount;
        public string Filter { get; set; }
    }
}