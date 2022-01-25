namespace ProcessLayer.Entities.CnB
{
    public interface IPayrollDeductions
    {
        long ID { get; set; }
        Deduction Deduction { get; set; }
        decimal? Amount { get; set; }
        decimal? PS { get; set; }
        decimal? ES { get; set; }
        decimal? EC { get; set; }
        bool Modified { get; set; }
    }
}
