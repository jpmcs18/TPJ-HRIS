namespace ProcessLayer.Entities
{
    public class PersonnelAssignedLocation : PersonnelHistoryBase
    {
        public int? LocationID { get; set; }

        public Location _Location { get; set; } = new Location();
    }

}
