namespace ProcessLayer.Entities.CnB
{
    public class CrewPayrollDeductions : IPayrollDeductions
    {
        public long ID { get; set; }
        public long CrewPayrollID { get; set; }
        public int DeductionID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PS { get; set; }
        public decimal? ES { get; set; }
        public decimal? EC { get; set; }
        public Deduction Deduction { get; set; }
        public bool Modified { get; set; } = false;
    }
}

