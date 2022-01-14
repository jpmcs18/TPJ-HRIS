using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public sealed class LeaveDefaultCreditsProcess : ILookupProcess<LeaveDefaultCredits>
    {
        public static readonly Lazy<LeaveDefaultCreditsProcess> Instance = new Lazy<LeaveDefaultCreditsProcess>(() => new LeaveDefaultCreditsProcess());
        private LeaveDefaultCreditsProcess() { }
        internal LeaveDefaultCredits Converter(DataRow dr)
        {
            var leave = new LeaveDefaultCredits
            {
                ID = dr["ID"].ToInt(),
                LeaveTypeID = dr["Leave Type ID"].ToInt(),
                MinYearsInService = dr["Min Years In Service"].ToFloat(),
                MaxYearsInService = dr["Max Years In Service"].ToNullableFloat(),
                Credits = dr["Credits"].ToFloat()
            };
            leave.LeaveType = LeaveTypeProcess.Instance.Value.Get(leave.LeaveTypeID);
            return leave;
        }

        public LeaveDefaultCredits CreateOrUpdate(string item, int user)
        {
            LeaveDefaultCredits defaultCredits = JsonConvert.DeserializeObject<LeaveDefaultCredits>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (DBTools db = new DBTools())
            {
                List<OutParameters> outParams = new List<OutParameters> { { "@ID", SqlDbType.Int, defaultCredits.ID } };
                db.ExecuteNonQuery("lookup.CreateOrUpdateLeaveDefaultCredits", ref outParams, new Dictionary<string, object> {
                    { "@LeaveTypeID", defaultCredits.LeaveTypeID },
                    { "@MinYearsInService", defaultCredits.MinYearsInService },
                    { "@MaxYearsInService", defaultCredits.MaxYearsInService },
                    { "@Credits", defaultCredits.Credits },
                    { "@LogBy", user },
                });
                defaultCredits.ID = outParams.Get("@ID").ToInt();
                return defaultCredits;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateLeaveDefaultCredits", new Dictionary<string, object> {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }

        public List<LeaveDefaultCredits> Filter(string key, int page, int gridCount, out int pageCount)
        {
            using (DBTools db = new DBTools())
            {
                List<OutParameters> outParameters = new List<OutParameters>
                {
                    { "@PageCount", SqlDbType.Int }
                };
                
                using (DataSet ds = db.ExecuteReader("lookup.FilterLeaveDefaultCredits", ref outParameters, new Dictionary<string, object>
                {
                    { "@Filter", key},
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                }))
                {
                    pageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public LeaveDefaultCredits Get(dynamic id)
        {
            if (id == null) return new LeaveDefaultCredits();
            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader("lookup.GetLeaveDefaultCredits",
                    new Dictionary<string, object> {
                        { "@ID", (id ?? 0) },
                    }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<LeaveDefaultCredits> GetList(float yearsInService)
        {
            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader("lookup.GetLeaveDefaultCredits", new Dictionary<string, object> { { "@YearsInService", yearsInService } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }
    }
}
