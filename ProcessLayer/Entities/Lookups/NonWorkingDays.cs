using System;

namespace ProcessLayer.Entities
{
    public class NonWorkingDays : Lookup<long>
    {
        public short? Year { get; set; }
        public DateTime? Day { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public byte? NonWorkingType { get; set; }
        public bool? IsGlobal { get; set; }
        public Lookup Type { get; set; }
        public bool? Yearly { get; set; }

        public int? LocationID { get; set; }
        public Location Location { get; set; }

    }
}
