using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelDeduction : PersonnelBase
    {
        [DisplayName("Deduction")]
        public int? DeductionID { get; set; }
        [DisplayName("Currency")]
        public int? CurrencyID { get; set; }
        [DisplayName("Amount")]
        public decimal? Amount { get; set; }

        public Deduction _Deduction { get; set; } = new Deduction();
        public Lookup _Currency { get; set; } = new Lookup();
    }
}
