using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities.Kiosk
{
    public class LeaveRequest : KioskBase
    {
        public int? LeaveTypeID { get; set; }
        public float? ApprovedLeaveCredits { get; set; }
        public float? ComputedLeaveCredits { get; set; }
        public float? NoofDays { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && ((DateTime.Now - CreatedOn).Value.TotalHours >= 48); } }
        public int? NotedBy { get; set; }
        public DateTime? NotedOn { get; set; }
        public bool? Noted { get; set; } = false;
        public bool? IsAbsent { get; set; } = false;
        public bool? IsHalfDay { get; set; } = false;
        public bool? IsMorning { get; set; } = false;
        public bool? IsAfternoon { get; set; } = false;

        public string Remarks
        {
            get
            {
                if (!(Approved ?? false))
                    return "Waiting for approval.";

                if ((_LeaveType?.HasDocumentNeeded ?? false) && (Approved ?? false) && string.IsNullOrEmpty(File))
                    return "Approved, Waiting for document upload.";

                if ((_LeaveType?.HasDocumentNeeded ?? false) && (Approved ?? false) && !string.IsNullOrEmpty(File) && (_LeaveType?.HasDocumentNeeded ?? false) && !(Noted ?? false))
                    return "Document attached, Waiting to be noted.";

                if (Cancelled != null && Cancelled.Value)
                    return CancellationRemarks;

                if (IsExpired)
                    return "Exceeded 48 hours upon creation date.";

                return "";
            }
        }

        public LeaveType _LeaveType { get; set; }
        public List<ComputedLeaveCredits> _ComputedLeaveCredits { get; set; }

        //only for medicard
        public string Hospital { get; set; }
        public string Location { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }

        public string _Noted { get; set; }
    }
}
