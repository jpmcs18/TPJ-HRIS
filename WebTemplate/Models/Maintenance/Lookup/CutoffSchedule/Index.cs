namespace WebTemplate.Models.Maintenance.Lookup.CutoffSchedule
{
    public class Index
    {
        public System.Collections.Generic.List<ProcessLayer.Entities.CutoffSchedule> ItemList { get; set; }
        public byte? FilterMonth { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; set; } = Properties.Settings.Default.GridCount;
        public int ItemCount { get; set; }
    }
}