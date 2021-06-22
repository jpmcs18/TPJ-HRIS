using ProcessLayer.Entities.CnB;
using System.Collections.Generic;

namespace WebTemplate.Models.Personnel
{
    public class PersonnelLoanDeductions
    {
        public long PersonnelLoanId { get; set; }
        public List<LoanDeductions> LoanDeductions { get; set; }
    }
}