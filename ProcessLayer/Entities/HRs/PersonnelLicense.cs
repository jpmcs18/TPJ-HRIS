using System;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelLicense : PersonnelBase
    {
        [DisplayName("License Type")]
        public int? LicenseTypeID { get; set; }
        [DisplayName("License No.")]
        public string LicenseNo { get; set; }
        [DisplayName("Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        public LicenseType _LicenseType { get; set; } = new LicenseType();
        public Personnel _Personnel { get; set; } = new Personnel();
    }
}
