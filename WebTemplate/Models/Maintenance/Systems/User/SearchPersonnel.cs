using System.Collections.Generic;

namespace WebTemplate.Models.Maintenance.Systems.User
{
    public class SearchPersonnel
    {
        public List<ProcessLayer.Entities.Personnel> ItemList { get; set; }
        public int ItemCount { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; } = Properties.Settings.Default.GridCount;
        public string Filter { get; set; }
    }
}