using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelDependent : PersonnelBase
    {
        [DisplayName("Relationship")]
        public int? RelationshipID { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Birth Date")]
        public DateTime? BirthDate { get; set; }

        public Lookup _Relationship { get; set; } = new Lookup();
    }
}
