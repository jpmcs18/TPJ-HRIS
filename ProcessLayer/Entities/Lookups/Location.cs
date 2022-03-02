namespace ProcessLayer.Entities
{
    public class Location : Lookup<int>
    {
        public string Prefix { get; set; }
        public bool? OfficeLocation { get; set; }
        public bool? WarehouseLocation { get; set; }
        public decimal? HazardRate { get; set; }
        public bool? RequiredTimeLog { get; set; }
        public bool? WithHolidayAndSunday { get; set; }
        public bool? WithAdditionalForExtension { get; set; }
        public bool? InternationalLocation { get; set; }
    }
}
