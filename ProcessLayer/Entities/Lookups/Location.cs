namespace ProcessLayer.Entities
{
    public class Location : Lookup<int>
    {
        public string Prefix { get; set; }
        public bool? OfficeLocation { get; set; } = false;
        public bool? WarehouseLocation { get; set; } = false;
        public decimal? HazardRate { get; set; }
        public bool? RequiredTimeLog { get; set; } = false;
        public bool? WithHolidayAndSunday { get; set; } = false;
        public bool? WithAdditionalForExtension { get; set; } = false;
    }
}
