using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.CrewMovement
{
    public class CrewMovement
    {
        public DateTime StartingDate { get; set; } = DateTime.Now.AddYears(-1);
        public DateTime EndingDate { get; set; } = DateTime.Now.AddHours(5);
        public long PersonnelID { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();
        public List<ProcessLayer.Entities.CrewMovement> CrewMovements { get; set; } = new List<ProcessLayer.Entities.CrewMovement>();
    }
}