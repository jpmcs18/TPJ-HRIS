using ProcessLayer.Helpers.Enumerable;
using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class OTRequest : KioskBase
    {
        public bool IsOffice { get; set; }
        public OTType OTType { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public bool IsExpired { get { return (Approved == true || Cancelled == true ||  CreatedOn == null) ? false : ((DateTime.Now - CreatedOn).Value.TotalHours >= 48) && false /*For Bypassing Expirationo*/; } } 
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
