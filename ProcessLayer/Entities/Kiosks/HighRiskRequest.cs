using System;

namespace ProcessLayer.Entities.Kiosk
{
    public class HighRiskRequest : KioskBase
    {
        public DateTime RequestDate { get; set; }
        public bool IsExpired { get { return Approved != true && Cancelled != true && CreatedOn != null && RequestDate != null && ((DateTime.Now - RequestDate).TotalHours >= 48) && false/*For Bypassing Expiration*/; } }
    }
}
