using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class KioskBase : LogDetails
    {
        public long ID { get; set; }
        public long? PersonnelID { get; set; }
        public string Reasons { get; set; }
        public bool? Cancelled { get; set; } = false;
        public int? CancelledBy { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancellationRemarks { get; set; }

        public bool? Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedOn { get; set; }

        public string _Cancel { get; set; }
        public string _Approver { get; set; }
        public Personnel _Personnel { get; set; }
    }
}
