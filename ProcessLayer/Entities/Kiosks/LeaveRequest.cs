using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities.Kiosk
{
    public class LeaveRequest : KioskBase
    {
        public byte? LeaveTypeID { get; set; }
        public float? ApprovedLeaveCredits { get; set; }
        public float? ComputedLeaveCredits { get; set; }
        public float NoofDays { get; set; }
        public DateTime RequestedDate { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && ((DateTime.Now - CreatedOn).Value.TotalHours >= 48) && false; } }
        public int? NotedBy { get; set; }
        public DateTime? NotedOn { get; set; }
        public bool Noted { get { return (_LeaveType?.CNBNoteFirst ?? false) && (NotedBy != null); } }
        public string Remarks
        {
            get
            {
                if (!Noted)
                    return "Must be noted first.";

                if (Noted && !(Approved ?? false) && (_LeaveType?.HasDocumentNeeded ?? false) && string.IsNullOrEmpty(File))
                    return "Noted, Waiting for approval.";

                if ((Approved ?? false) && (_LeaveType?.HasDocumentNeeded ?? false) && string.IsNullOrEmpty(File))
                    return "Partialy approved, Waiting for document upload";

                if (IsExpired)
                    return "Exceeded 48 hours upon creation date.";
                if (Cancelled != null && Cancelled.Value)
                    return CancellationRemarks;

                return "";
            }
        }
            
        public LeaveType _LeaveType { get; set; }
        public List<ComputedLeaveCredits> _ComputedLeaveCredits { get; set; }
    }
}
