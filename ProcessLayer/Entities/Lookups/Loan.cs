namespace ProcessLayer.Entities
{
    public class Loan : Lookup<int>
    {
        public bool? GovernmentLoan { get; set; } = false;
        public bool? IsPersonal { get; set; } = false;
    }
}
