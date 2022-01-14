using System;

namespace ProcessLayer.Entities
{
    public class TaxTable
    {
        public long ID { get; set; }
        public decimal? MinimumIncome { get; set; }
        public decimal? MaximumIncome { get; set; }
        public decimal? FixedTax { get; set; }
        public decimal? AdditionalTax { get; set; }
        public decimal? ExcessOver { get; set; }
        public int? TaxScheduleID { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }

        public TaxSchedule TaxSchedule { get; set; }

    }
    public class TaxSchedule : Lookup<int> { }

}
