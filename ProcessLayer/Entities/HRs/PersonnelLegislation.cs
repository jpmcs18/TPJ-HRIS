using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelLegislation : PersonnelBase
    {
        public string Title { get; set; }
        public string File { get; set; }
        [DisplayName("Legislation Date")]
        public DateTime? LegislationDate { get; set; }
        [DisplayName("Legislation Status")]
        public int? LegislationStatusID { get; set; }
        [DisplayName("Status Date")]
        public DateTime? StatusDate { get; set; }

        public Lookup _LegislationStatus { get; set; } = new Lookup();
    }
}
