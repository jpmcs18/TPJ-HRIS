using System;

namespace ProcessLayer.Entities
{
    public class Crew
    {
        public long ID { get; set; }
        public short? VesselID { get; set; }
        public long? PersonnelID { get; set; }
        public short? PositionID { get; set; }
        public int? DepartmentID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remarks { get; set; }

        public string CrewSearchKey { get; set; }

        public Vessel _Vessel { get; set; } = new Vessel();
        public Personnel _Personnel { get; set; } = new Personnel();
        public Position _Position { get; set; } = new Position();
        public Department _Department { get; set; } = new Department();
    }
}
