using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class LeaveRequest : KioskBase
    {
        public byte? LeaveTypeID { get; set; }
        public float? ApprovedLeaveCredits { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string File { get; set; }
        public string FilePath { get; set; }
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && StartDateTime != null && ((DateTime.Now - CreatedOn).Value.TotalHours >= 48) && false /*For Bypassing Expirationo*/; } }
        public string Remarks
        {
            get
            {
                if ((Approved ?? false) && (((_LeaveType?.HasDocumentNeeded ?? false) && string.IsNullOrEmpty(File)) || !(_LeaveType?.HasDocumentNeeded ?? false)))
                    return "Partialy approved, Waiting for document to upload";

                if (IsExpired)
                    return "Exceeded 48 hours upon request Start Date & Time.";
                if (Cancelled != null && Cancelled.Value)
                    return CancellationRemarks;

                return "";
            }
        }

        public LeaveType _LeaveType { get; set; }
    }
}
