namespace WebTemplate.Models.Maintenance.Lookup.ApproverPosition
{
    public class Index
    {
        public System.Collections.Generic.List<ProcessLayer.Entities.ApproverPosition> ItemList { get; set; }
        public string Filter { get; set; }
        public bool? IsApprover { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; set; } = Properties.Settings.Default.GridCount;
        public int ItemCount { get; set; }
    }
}