using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class EducationalBackground : PersonnelBase
    {
        [DisplayName("Educational Level")]
        public int? EducationalLevelID { get; set; }
        [DisplayName("School Name")]
        public string SchoolName { get; set; }
        [DisplayName("Course")]
        public string Course { get; set; }
        [DisplayName("From Year")]
        public int? FromYear { get; set; }
        [DisplayName("To Year")]
        public int? ToYear { get; set; }

        public Lookup _EducationalLevel { get; set; } = new Lookup();
    }
}
