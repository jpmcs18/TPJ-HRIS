using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class OTRequest : KioskBase
    {
        public bool IsOffice { get; set; }
        public bool IsEarlyOT { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsExpired { get { return (Approved == true || Cancelled == true ||  CreatedOn == null) ? false : ((DateTime.Now - CreatedOn).Value.TotalHours >= 48); } } 
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
