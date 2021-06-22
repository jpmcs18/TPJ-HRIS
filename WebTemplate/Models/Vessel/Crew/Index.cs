using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Vessel.Crew
{
    public class Index : BaseModel
    {
        public string Name { get; set; }
        public short? VesselID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? @EndDate { get; set; }
        public short? PositionID { get; set; }
        public int? DepartmentID { get; set; }
        public string Remarks { get; set; }

        public List<ProcessLayer.Entities.Crew> Crews { get; set; } = new List<ProcessLayer.Entities.Crew>();
    }
}