namespace ProcessLayer.Entities
{
    public class Loan : Lookup<int>
    {
        public bool? GovernmentLoan { get; set; }
        public bool? isPersonal { get; set; }
    }
}
