using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelVaccine : PersonnelBase
    {
        [DisplayName("Vaccine Type")]
        public int? VaccineTypeID { get; set; }
        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        public Lookup _VaccineType { get; set; } = new Lookup();
    }

}
