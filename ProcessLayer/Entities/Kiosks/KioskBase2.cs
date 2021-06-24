using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class KioskBase2 : LogDetails
    {
        public long ID { get; set; }
        public long? PersonnelID { get; set; }
        public string Purpose { get; set; }
        public bool? Cancelled { get; set; }
        public int? CancelledBy { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string CancellationRemarks { get; set; }


        public string _Cancel { get; set; }
        public Personnel _Personnel { get; set; }
    }
}
