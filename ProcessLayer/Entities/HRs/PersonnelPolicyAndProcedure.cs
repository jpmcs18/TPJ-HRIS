using System.Collections.Generic;

namespace ProcessLayer.Entities
{
    public class PersonnelPolicyAndProcedure
    {
        public long ID { get; set; }
        public long PolicyAndProcedureID { get; set; }
        public long? PersonnelID { get; set; }
        public int? VesselID { get; set; }
        public bool Acknowledge { get; set; }

        public Personnel Personnel { get; set; }
        public Vessel Vessel { get; set; }
    }
}
