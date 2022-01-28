using System;

namespace ProcessLayer.Entities
{
    public class NonWorkingDays : Lookup<long>
    {
        public short? Year { get; set; }
        public DateTime? Day { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public int? NonWorkingType { get; set; }
        public bool? IsGlobal { get; set; } = false;
        public Lookup Type { get; set; }
        public bool? Yearly { get; set; } = false;

        public int? LocationID { get; set; }
        public Location Location { get; set; }

    }
}
