namespace WebTemplate.Models.Maintenance.Lookup.PositionSalary
{
    public class Index
    {
        public System.Collections.Generic.List<ProcessLayer.Entities.PositionSalary> ItemList { get; set; }
        public string Filter { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; set; } = Properties.Settings.Default.GridCount;
        public int ItemCount { get; set; }
    }
}