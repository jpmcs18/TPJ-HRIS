using ProcessLayer.Helpers;
using System.Collections.Generic;

namespace ProcessLayer.Entities.CnB
{
    public class CrewVessel
    {
        public long PayPeriodID { get; set; }
        public Vessel Vessel { get; set; }
        public List<CrewPayroll> CrewPayrolls { get; set; } = new List<CrewPayroll>();
        public List<CrewMovement> CrewMovement { get; set; } = new List<CrewMovement>();
    }
}

