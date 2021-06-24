namespace ProcessLayer.Entities.CnB
{
    public class PayrollDeductions
    {
        public long ID { get; set; }
        public Deduction Deduction { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PS { get; set; }
        public decimal? ES { get; set; }
        public decimal? EC { get; set; }
    }
}
