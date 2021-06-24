using System;

namespace ProcessLayer.Entities
{
    public class ScheduleType : Lookup<byte>
    {
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        public TimeSpan? BreakTime { get; set; }
        public byte? BreakTimeHour { get; set; }
        public bool? AtHome { get; set; }
        public byte? TotalWorkingHours { get; set; }
        public bool? MustBePresentOnly { get; set; }
    }
}
