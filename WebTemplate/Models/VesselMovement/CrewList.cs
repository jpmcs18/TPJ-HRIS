using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.VesselMovement
{
    public class CrewList
    {
        public short VesselID { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }

        public ProcessLayer.Entities.Vessel Vessel = new();

        public List<ProcessLayer.Helpers.CrewDetails> Crews = new();
    }
}