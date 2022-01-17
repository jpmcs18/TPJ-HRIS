namespace ProcessLayer.Entities
{
    public class CrewPositionSalary
    {
        public int ID { get; set; }
        public int PositionID { get; set; }
        public decimal? FishingGroundRate { get; set; }
        public decimal? StandbyGroundRate { get; set; }

        public Position Position { get; set; }
    }
}
