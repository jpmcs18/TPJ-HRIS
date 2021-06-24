using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelCompensation : PersonnelBase
    {
        [DisplayName("Compensation")]
        public int? CompensationID { get; set; }
        [DisplayName("Currency")]
        public int? CurrencyID { get; set; }
        [DisplayName("Amount")]
        public decimal? Amount { get; set; }

        public Compensation _Compensation { get; set; } = new Compensation();
        public Lookup _Currency { get; set; } = new Lookup();
    }
}
