namespace ProcessLayer.Entities
{
    public class PersonnelPosition : PersonnelHistoryBase
    {
        public int? PositionID { get; set; }

        public Position _Position { get; set; } = new Position();
    }
}
