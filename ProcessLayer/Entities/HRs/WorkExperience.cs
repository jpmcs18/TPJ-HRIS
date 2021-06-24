using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class WorkExperience : PersonnelBase
    {
        public string Company { get; set; }
        public string Position { get; set; }
        [DisplayName("Employment Type")]
        public string EmploymentType { get; set; }
        [DisplayName("From Year")]
        public int? FromYear { get; set; }
        [DisplayName("To Year")]
        public int? ToYear { get; set; }
        [DisplayName("To Month")]
        public int? ToMonth { get; set; }
        [DisplayName("From Month")]
        public int? FromMonth { get; set; }
    }
}
