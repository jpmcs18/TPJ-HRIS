using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities.Kiosk
{
    public class AbsenceRequest : KioskBase
    {
        public float? NoofDays { get; set; }
        public DateTime? RequestDate { get; set; }
        public bool? Noted { get; set; } = false;
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && ((DateTime.Now - CreatedOn).Value.TotalHours >= 48); } }
        public int? NotedBy { get; set; }
        public DateTime? NotedOn { get; set; }
        public bool? IsAbsent { get; set; } = false;
        public bool? IsHalfDay { get; set; } = false;
        public bool? IsMorning { get; set; } = false;
        public bool? IsAfternoon { get; set; } = false;
        public bool? IsUndertime { get; set; } = false;
        public DateTime? Time { get; set; }
        public string Remarks
        {
            get
            {
                if (IsExpired)
                    return "Exceeded 48 hours upon creation date.";

                if ((Cancelled ?? false))
                    return CancellationRemarks;

                if (!(Approved ?? false))
                    return "Waiting for approval.";

                return "";
            }
        }

        public string _Noted { get; set; }
    }
}
