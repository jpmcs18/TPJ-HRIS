using System;

namespace ProcessLayer.Entities
{
    public class LateDeduction
    {
        public int ID { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public int? DeductedHours { get; set; }
    }
}
