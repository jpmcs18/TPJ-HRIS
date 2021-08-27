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
    public sealed class LeaveTypeProcess : ILookupProcess<LeaveType>
    {
        public static readonly Lazy<LeaveTypeProcess> Instance = new Lazy<LeaveTypeProcess>(() => new LeaveTypeProcess());
        private LeaveTypeProcess() { }
        internal LeaveType Converter(DataRow dr)
        {
            return new LeaveType
            {
                ID = dr["ID"].ToByte(),
                Description = dr["Description"].ToString(),
                MaxAllowedDays = dr["Max Allowed Days"].ToNullableInt(),
                BulkUse = dr["Bulk Use"].ToNullableBoolean(),
                DaysBeforeRequest = dr["Days Before Request"].ToNullableInt(),
                HasDocumentNeeded = dr["Has Document Needed"].ToNullableBoolean()
            };
        }

        public List<LeaveType> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeaveType"))
                {
                    var l = ds.GetList(Converter);

                    return l;
                }
            }           
        }

        public List<LeaveType> GetLeaveTypesThatHasDocumentNeeded()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.[GetLeaveTypeThatNeedDocument]"))
                {
                    var l = ds.GetList(Converter);

                    return l;
                }
            }
        }

        public List<LeaveType> GetLeavesWithCredits(long personnelID, short year)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeavesWithCredits", new Dictionary<string, object> { 
                    { "@PersonnelID", personnelID }, 
                    { "@Year", year } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public List<LeaveType> Filter(string key, int page, int gridCount, out int pageCount)
        {
            using (var db = new DBTools())
            {
                var outParameters = new List<OutParameters> {
                    { "@PageCount", SqlDbType.Int }
                };

                var parameters = new Dictionary<string, object>
                {
                    { "@Filter", key },
                    { "@PageNumber", page},
                    { "@GridCount", gridCount }
                };

                using (var ds = db.ExecuteReader("lookup.FilterLeaveType", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public LeaveType Get(dynamic id)
        {
            if (id == null) return new LeaveType();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeaveType", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public LeaveType CreateOrUpdate(string item, int user)
        {
            var leave = JsonConvert.DeserializeObject<LeaveType>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Description", leave.Description },
                    { "@MaxAllowedDays", leave.MaxAllowedDays },
                    { "@BulkUse", leave.BulkUse },
                    { "@DaysBeforeRequest", leave.DaysBeforeRequest },
                    { "@HasDocumentNeeded", leave.HasDocumentNeeded },
                    { "@LogBy", user }
                };

                var outParameters = new List<OutParameters>
                {
                    { "@ID", SqlDbType.TinyInt, leave.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateLeaveType", ref outParameters, parameters);
                leave.ID = outParameters.Get("@ID").ToByte();
                return leave;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateLeaveType", new Dictionary<string, object> { { "@ID", id }, { "@Delete", true }, { "@LogBy", user } });
            }
        }
    }
}
