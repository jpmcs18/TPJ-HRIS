using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public sealed class KioskNotificationProcess
    {
        public static readonly Lazy<KioskNotificationProcess> Instance = new Lazy<KioskNotificationProcess>(() => new KioskNotificationProcess());
        private KioskNotificationProcess() { }

        public KioskNotification Converter(DataRow dr)
        {
            return new KioskNotification
            {
                Type = dr["Type"].ToKioskNotificationType(),
                Filter = dr["Filter"].ToKioskNotificationFilter(),
                Count = dr["Cnt"].ToInt()
            };
        }

        public List<KioskNotification> GetList(int userid)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("kiosk.[GetKioskNotification]", new Dictionary<string, object> { { "@Approver", userid } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public KioskNotification GetHighRisk()
        {

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("kiosk.[GetKioskNotificationHighRisk]"))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
