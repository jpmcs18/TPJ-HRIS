using System;

namespace ProcessLayer.Entities
{
    public class CrewMovement
    {
        public long ID { get; set; }
        public string TransactionNo { get; set; }
        public long PersonnelID { get; set; }
        public long? PreviousCrewMovementID { get; set; }
        public int? DepartmentID { get; set; }
        public int? PositionID { get; set; }
        public int? VesselID { get; set; }
        public decimal? DailyRate { get; set; }
        public int? SNPositionID { get; set; }
        public int? SNVesselID { get; set; }
        public decimal? SNDailyRate { get; set; }
        public DateTime? OnboardDate { get; set; }
        public DateTime? OffboardDate { get; set; }
        public string Remarks { get; set; }
        public bool DryDock { get; set; }

        public int Status { get; set; }

        public int? CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public int? CheckedBy { get; set; }
        public int? NotedBy { get; set; }
        public int? PostedBy { get; set; }
        public int? CancelledBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? CheckedDate { get; set; }
        public DateTime? NotedDate { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? CancelledDate { get; set; }

        public Personnel _Personnel { get; set; }
        public CrewMovement _PreviousCrewMovement { get; set; }
        public Department _Department { get; set; }
        public Position _Position { get; set; }
        public Vessel _Vessel { get; set; }
        public Position _SNPosition { get; set; }
        public Vessel _SNVessel { get; set; }
        
        public string _Creator { get; set; }
        public string _Modify { get; set; }
        public string _Check { get; set; }
        public string _Note { get; set; }
        public string _Post { get; set; }
        public string _Cancelled { get; set; }
        public bool IsLast { get; set; }
    }
}
