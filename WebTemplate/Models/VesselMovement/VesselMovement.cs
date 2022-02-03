using System;
using System.Collections.Generic;

namespace WebTemplate.Models.VesselMovement
{
    public class VesselMovement
    {
        public DateTime StartingDate { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime EndingDate { get; set; } = DateTime.Now.AddHours(5);
        public short VesselID { get; set; }
        public ProcessLayer.Entities.Vessel Vessel { get; set; } = new ProcessLayer.Entities.Vessel();
        public List<ProcessLayer.Entities.VesselMovement> VesselMovements { get; set; } = new List<ProcessLayer.Entities.VesselMovement>();
    }
}