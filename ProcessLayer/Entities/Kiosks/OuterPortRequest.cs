using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class OuterPortRequest : KioskBase2
    {
        public byte? LocationID { get; set; }    
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsHighRisk { get; set; } = false;
        public bool? HasQuarantine { get; set; } = false;
        public DateTime? QuarantineDateEnd { get; set; }

        public Location _Location { get; set; }
    }
}
