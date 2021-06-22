namespace WebTemplate.Models.Personnel
{
    public class MergeAdditional
    {
        public long PersonnelID { get; set; }
        public long PersonnelLoanID { get; set; }
        public long AdditionalLoanID { get; set; }
        public decimal Amount { get; set; }
    }
}