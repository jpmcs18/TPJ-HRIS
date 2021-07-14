using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities.HR
{
    public class PersonnelLoan : PersonnelBase
    {
        public byte? LoanID { get; set; }
        public decimal? Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal? Amortization { get; set; }
        public int? PaymentTerms { get; set; }
        public bool? PayrollDeductible { get; set; }
        public byte? WhenToDeduct { get; set; }
        public string Remarks { get; set; }
        public long? PayrollID { get; set; }

        public Loan _Loan { get; set; }
    }
}
