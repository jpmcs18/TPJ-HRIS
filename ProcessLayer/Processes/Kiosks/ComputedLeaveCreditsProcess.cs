using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public sealed class ComputedLeaveCreditsProcess
    {
        public static readonly ComputedLeaveCreditsProcess Instance = new ComputedLeaveCreditsProcess();
        private ComputedLeaveCreditsProcess() { }

        internal ComputedLeaveCredits Converter(DataRow dr)
        {
            return new ComputedLeaveCredits
            {
                ID = dr["ID"].ToLong(),
                ComputedDate = dr["Computed Date"].ToDateTime(),
                LeaveRequestID = dr["Leave Request ID"].ToLong(),
                LeaveCreditUsed = dr["Leave Credit Used"].ToFloat()
            };
        }

        public List<ComputedLeaveCredits> GetList(long requestId, DateTime? start, DateTime? end)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object> { 
                    { "@LeaveRequestID", requestId }, 
                    { "@StartDate", start },
                    { "@EndDate", end }
                };
                using (var ds = db.ExecuteReader("kiosk.GetComputedLeaveCredits", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public void CreateOrUpdate(DBTools db, ComputedLeaveCredits computedLeaveCredits, int userId)
        {
            List<OutParameters> outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, computedLeaveCredits.ID }
            };

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@LeaveRequestID", computedLeaveCredits.LeaveRequestID },
                { "@ComputedDate", computedLeaveCredits.ComputedDate },
                { "@LeaveCreditUsed", computedLeaveCredits.LeaveCreditUsed },
                { "@LogBy", userId }
            };

            db.ExecuteNonQuery("kiosk.CreateOrUpdateComputedLeaveCredits", ref outparameters, parameters);
            computedLeaveCredits.ID = outparameters.Get("@ID").ToLong();
        }
        public void Delete(DBTools db, long id, int userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@ID", id },
                { "@Delete", true },
                { "@LogBy", userId }
            };

            db.ExecuteNonQuery("kiosk.CreateOrUpdateComputedLeaveCredits", parameters);
        }
    }
}
