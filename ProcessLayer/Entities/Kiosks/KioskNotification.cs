using ProcessLayer.Helpers.Enumerable;

namespace ProcessLayer.Entities.Kiosk
{
    public class KioskNotification
    {
        //1 Leave
        //2 OT
        //3 Time edit
        //4 High Risk
        public KioskNotoficationType Type { get; set; }

        //1 Pending
        //2 Expired
        public KioskNotificationFilter Filter { get; set; }
        public int Count { get; set; }
    }
}
