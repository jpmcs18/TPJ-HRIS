using System;
using System.Collections.Generic;

namespace WebTemplate.Models.CrewMovement
{
    public class ActualMovement
    {
        public DateTime? StartingDate { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime? EndingDate { get; set; } = DateTime.Now.AddHours(5);
        public long PersonnelID { get; set; }
        public short? VesselID { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();
        public ProcessLayer.Entities.Vessel Vessel { get; set; } = new ProcessLayer.Entities.Vessel();
        public List<ProcessLayer.Entities.CrewMovement> CrewActualMovement { get; set; } = new List<ProcessLayer.Entities.CrewMovement>();

    }
}