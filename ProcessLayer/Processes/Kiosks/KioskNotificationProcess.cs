using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public class KioskNotificationProcess
    {
        private static KioskNotificationProcess _instance;

        public static KioskNotificationProcess Instance
        {
            get { if (_instance == null) _instance = new KioskNotificationProcess();  return _instance; }
        }

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
