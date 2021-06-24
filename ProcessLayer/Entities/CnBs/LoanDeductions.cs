using ProcessLayer.Entities.HR;
using System;

namespace ProcessLayer.Entities.CnB
{
    public class LoanDeductions
    {
        public long ID { get; set; }
        public DateTime CutoffStart { get; set; }
        public DateTime CutoffEnd { get; set; }
        public PersonnelLoan PersonnelLoan { get; set; }
        public decimal Amount { get; set; }
    }
}
