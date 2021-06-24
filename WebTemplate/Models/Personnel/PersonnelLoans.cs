using ProcessLayer.Entities.HR;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class PersonnelLoans
    {
        public long PersonnelID { get; set; }
        public List<PersonnelLoan> PersonnelLoan { get; set; } = new List<PersonnelLoan>();
    }
}