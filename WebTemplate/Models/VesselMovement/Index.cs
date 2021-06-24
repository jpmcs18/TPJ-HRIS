using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace WebTemplate.Models.VesselMovement
{
    public class Index : BaseModel
    {
        //public Index()
        //{
        //    GridCount = 10;
        //}

        public string Code { get; set; }
        public string Description { get; set; }
        public List<ProcessLayer.Entities.Vessel> VesselList = new List<ProcessLayer.Entities.Vessel>();
    }
}