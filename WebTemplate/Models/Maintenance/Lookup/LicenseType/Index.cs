namespace WebTemplate.Models.Maintenance.Lookup.LicenseType
{
    public class Index
    {
        public System.Collections.Generic.List<ProcessLayer.Entities.LicenseType> ItemList { get; set; }
        public string Filter { get; set; }
        public bool? IsPerpetual { get; set; } = false;
        public int PageNumber { get; set; }
        public int GridCount { get; set; } = Properties.Settings.Default.GridCount;
        public int ItemCount { get; set; }
    }
}