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
        public short VesselID { get; set; }
        public string Place { get; set; }
        public int MovementTypeID { get; set; }
        public DateTime MovementDate { get; set; }

        public Vessel _Vessel { get; set; } = new Vessel();
        public Lookup _VesselMovementType { get; set; } = new Lookup();
    }
}
