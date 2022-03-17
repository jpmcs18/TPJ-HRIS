using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class VesselMovement
    {
        public long ID { get; set; }
        public int VesselID { get; set; }
        //public int MovementTypeID { get; set; }
        public DateTime MovementDate { get; set; }
        public DateTime VoyageStartDate { get; set; }
        public DateTime? VoyageEndDate { get; set; }
        public int VoyageStatusID { get; set; }
        public int OriginLocationID { get; set; }
        public int? DestinationLocationID { get; set; }
        public DateTime? ETA { get; set; }
        public DateTime? ETD { get; set; }
        public string VoyageDetails { get; set; }
        public Vessel _Vessel { get; set; } = new Vessel();
        //public Lookup _VesselMovementType { get; set; } = new Lookup();
        public int MovementStatusID { get; set; }
        public string MovementStatus
        {
            get
            {
                switch (MovementStatusID)
                {
                    case 0: return "Cancel";
                    case 1: return "Pending";
                    case 2: return "Checked";
                    case 3: return "Approved";
                    default: return "";
                }
            }
        }

        public int CreatedBy { get; set; }
        public string Creator { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy  { get; set; }
        public string Modifier { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ApprovedBy { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? CheckedBy { get; set; }
        public string Checker { get; set; }
        public DateTime? CheckedDate { get; set; }

        public Location OriginLocation { get; set; }
        public Location DestinationLocation { get; set; }

        public List<VesselMovementCrews> VesselMovementCrewList { get; set; } = new List<VesselMovementCrews>();
        public List<VesselCrews> CrewList { get; set; } = new List<VesselCrews>();
    }  

}
