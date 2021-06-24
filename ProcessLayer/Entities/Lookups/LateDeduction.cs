using System;

namespace ProcessLayer.Entities
{
    public class LateDeduction
    {
        public short ID { get; set; }
        public TimeSpan? TimeIn { get; set; }
        public byte? DeductedHours { get; set; }
    }
}
