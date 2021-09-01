using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class TimeEditRequest : KioskBase
    {
        public DateTime RequestDate { get; set; }
        public DateTime? LoginDateTime { get; set; }
        public DateTime? LogoutDateTime { get; set; }
        //public DateTime? StartDateTime { get; set; }
        //public DateTime? EndDateTime { get; set; }
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && ((DateTime.Now - CreatedOn).Value.TotalHours >= 48) && false; } }
        public string Remarks
        {
            get
            {
                if (IsExpired)
                {
                    return "Exceeded 48 hours upon request Start Date & Time.";
                }
                else if (Cancelled != null && Cancelled.Value)
                {
                    return CancellationRemarks;
                }
                return "";

            }
        }
    }
}
