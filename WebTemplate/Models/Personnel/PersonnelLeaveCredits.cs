using ProcessLayer.Entities.HR;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class PersonnelLeaveCredits
    {
        public long PersonnelID { get; set; }
        public List<PersonnelLeaveCredit> PersonnelLeaveCredit { get; set; } = new List<PersonnelLeaveCredit>();
    }
}