using ProcessLayer.Entities.HR;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class PersonnelLeaveCredits
    {
        public long PersonnelID { get; set; }
        public List<PersonnelLeaveCredit> PersonnelLeaveCreditYear { get; set; } = new List<PersonnelLeaveCredit>();
        public List<PersonnelLeaveCredit> PersonnelLeaveCreditDate { get; set; } = new List<PersonnelLeaveCredit>();
    }
}