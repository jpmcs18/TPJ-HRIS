using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities.CnB
{
    public class AdditionalLoanForApproval
    {
        public long ID { get; set; }
        public long PersonnelID { get; set; }
        public int LoanID { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }

        public Loan _Loan { get; set; }
    }
}
