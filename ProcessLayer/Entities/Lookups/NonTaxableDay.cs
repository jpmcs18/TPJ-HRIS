using System;

namespace ProcessLayer.Entities
{
    public class NonTaxableDay : Lookup<int>
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsGlobal { get; set; }
        public int? LocationID { get; set; }
        public Location Location { get; set; }
    }
}
