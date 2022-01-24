using ProcessLayer.Entities.HR;

namespace ProcessLayer.Entities.CnB
{
    public class CrewLoanDeductions
    {
        public long ID { get; set; }
        public long CrewPayrollID { get; set; }
        public PersonnelLoan PersonnelLoan { get; set; }
        public decimal Amount { get; set; }
        public bool Modified { get; set; } = false;
    }
}
