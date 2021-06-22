using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class CompensationsAndDeductions
    {
        public long PersonnelID { get; set; }
        public List<PersonnelCompensation> Compensation { get; set; } = new List<PersonnelCompensation>();
        //public List<PersonnelDeduction> Deduction { get; set; } = new List<PersonnelDeduction>();
        public List<AssumedPersonnelDeduction> AssumedDeductions { get; set; } = new List<AssumedPersonnelDeduction>();
    }
}