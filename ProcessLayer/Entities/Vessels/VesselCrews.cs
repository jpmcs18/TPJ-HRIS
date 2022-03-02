using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class VesselCrews
    {
        public long CrewMovementID { get; set; }
        public int PositionID { get; set; }
        public long PersonnelID { get; set; }
        public decimal? BasicRate { get; set; }
        public DateTime? DateEmbarked { get; set; }
        public DateTime? DateDisembarked { get; set; }
        public Position Position { get; set; }
        public Personnel Personnel { get; set; }
        public CrewMovement CrewMovement { get; set; }
    }
}
