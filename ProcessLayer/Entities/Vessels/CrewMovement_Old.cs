using System;

namespace ProcessLayer.Entities
{
    public class CrewMovement_Old
    {
        public long ID { get; set; }
        public string TransactionNo { get; set; }
        public long PersonnelID { get; set; }
        public int? PreviousDepartmentID { get; set; }
        public int? PreviousPositionID { get; set; }
        public int? PreviousVesselID { get; set; }
        public int? PreviousSNPositionID { get; set; }
        public int? PreviousSNVesselID { get; set; }
        public int? CurrentDepartmentID { get; set; }
        public int? CurrentPositionID { get; set; }
        public int? CurrentVesselID { get; set; }
        public int? CurrentSNPositionID { get; set; }
        public int? CurrentSNVesselID { get; set; }
        public DateTime MovementDate { get; set; }
        public string Remarks { get; set; }
        public int Status { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? CheckedBy { get; set; }
        public int? NotedBy { get; set; }
        public int? PostedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? NotedDate { get; set; }
        public DateTime? PostedDate { get; set; }

        public Personnel _Personnel { get; set; } = new Personnel();
        public Lookup _PreviousDepartment { get; set; } = new Lookup();
        public Position _PreviousPosition { get; set; } = new Position();
        public Vessel _PreviousVessel { get; set; } = new Vessel();
        public Position _PreviousSNPosition { get; set; } = new Position();
        public Vessel _PreviousSNVessel { get; set; } = new Vessel();
        public Lookup _CurrentDepartment { get; set; } = new Lookup();
        public Position _CurrentPosition { get; set; } = new Position();
        public Vessel _CurrentVessel { get; set; } = new Vessel();
        public Position _CurrentSNPosition { get; set; } = new Position();
        public Vessel _CurrentSNVessel { get; set; } = new Vessel();
        public string _Creator { get; set; }
        public string _Modify { get; set; }
        public string _Check { get; set; }  
        public string _Note { get; set; }
        public string _Post { get; set; }
    }
}
