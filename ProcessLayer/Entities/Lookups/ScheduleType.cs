using System;

namespace ProcessLayer.Entities
{
    public class ScheduleType : Lookup<int>
    {
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? BreakTime { get; set; }
        public int? BreakTimeHour { get; set; }
        public bool? AtHome { get; set; }
        public int? TotalWorkingHours { get; set; }
        public bool? MustBePresentOnly { get; set; }
    }
}
