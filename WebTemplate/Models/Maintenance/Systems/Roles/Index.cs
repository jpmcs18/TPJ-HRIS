namespace WebTemplate.Models.Maintenance.Systems.Roles
{
    public class Index
    {
        public System.Collections.Generic.List<DataAccessLayer.System.Role> ItemList { get; set; }
        public int ItemCount { get; set; }
        public int PageNumber { get; set; }
        public int GridCount { get; } = Properties.Settings.Default.GridCount;
        public string Filter { get; set; }
    }
}