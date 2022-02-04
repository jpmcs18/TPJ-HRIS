using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class Index : BaseModel
    {
        public int? EmploymentStatusID { get; set; }
        public int? DepartmentID { get; set; }
        public int? PersonnelTypeID { get; set; }
        public int? LocationID { get; set; }
        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public List<ProcessLayer.Entities.Personnel> BirthdayCelebrantsRecent { get; set; } = new List<ProcessLayer.Entities.Personnel>();
        public List<ProcessLayer.Entities.Personnel> BirthdayCelebrantsToday { get; set; } = new List<ProcessLayer.Entities.Personnel>();
        public List<ProcessLayer.Entities.Personnel> BirthdayCelebrantsUpcoming { get; set; } = new List<ProcessLayer.Entities.Personnel>();

        public List<PersonnelLicense> ExpiringLicensesRecent { get; set; } = new List<PersonnelLicense>();
        public List<PersonnelLicense> ExpiringLicensesToday { get; set; } = new List<PersonnelLicense>();
        public List<PersonnelLicense> ExpiringLicensesUpcoming { get; set; } = new List<PersonnelLicense>();

        public List<KioskNotification> KioskNotifications { get; set; } = new List<KioskNotification>();
    }
}