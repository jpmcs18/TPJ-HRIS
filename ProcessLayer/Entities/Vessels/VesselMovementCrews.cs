namespace ProcessLayer.Entities
{
    public class VesselMovementCrews
    {
        public long ID { get; set; }
        public long VesselMovementID { get; set; }
        public long PersonnelID { get; set; }
        public int? DepartmentID { get; set; }
        public int PositionID { get; set; }
        public decimal? DailyRate { get; set; }
        public string Remarks { get; set; }


        public Position Position { get; set; }
        public Personnel Personnel { get; set; }
        public Department Department { get; set; }
    }
}
