using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class ComputedLeaveCredits
    {
        public long ID { get; set; }
        public long LeaveRequestID { get; set; }
        public DateTime ComputedDate { get; set; }
        public float LeaveCreditUsed { get; set; }

        public bool Modified { get; set; } = false;
    }
}
