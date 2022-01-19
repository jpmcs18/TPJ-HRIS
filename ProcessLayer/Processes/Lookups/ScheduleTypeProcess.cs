using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class ScheduleTypeProcess : ILookupProcess<ScheduleType>, ILookupSourceProcess<ScheduleType>
    {
        public static readonly ScheduleTypeProcess Instance = new ScheduleTypeProcess();
        private ScheduleTypeProcess() { }

        internal ScheduleType Converter(DataRow dr)
        {
            return new ScheduleType
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                TimeIn = dr["Time In"].ToNullableTimeSpan(),
                TimeOut = dr["Time Out"].ToNullableTimeSpan(),
                BreakTime = dr["Break Time"].ToNullableTimeSpan(),
                BreakTimeHour = dr["Break Time Hour"].ToNullableInt(),
                AtHome = dr["At Home"].ToNullableBoolean(),
                TotalWorkingHours = dr["Total Working Hours"].ToNullableInt(),
                MustBePresentOnly = dr["Must Be Present Only"].ToNullableBoolean()
            };
        }

        public ScheduleType CreateOrUpdate(string item, int user)
        {
            var scheduleType = JsonConvert.DeserializeObject<ScheduleType>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return SaveScheduleType(scheduleType, user);
        }

        private ScheduleType SaveScheduleType(ScheduleType scheduleType, int user)
        {
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> {
                    { "@ID", SqlDbType.TinyInt, scheduleType.ID }
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdateScheduleType", ref outParam, new Dictionary<string, object> {
                    { "@Description", scheduleType.Description},
                    { "@TimeIn", scheduleType.TimeIn},
                    { "@TimeOut", scheduleType.TimeOut},
                    { "@BreakTime", scheduleType.BreakTime},
                    { "@BreakTimeHour", scheduleType.BreakTimeHour},
                    { "@AtHome", scheduleType.AtHome},
                    { "@TotalWorkingHours", scheduleType.TotalWorkingHours},
                    { "@MustBePresentOnly", scheduleType.MustBePresentOnly},
                    { "@LogBy", user},
                });
                scheduleType.ID = outParam.Get("@ID").ToInt();
                return scheduleType;
            }

        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateScheduleType", new Dictionary<string, object> {
                    { "@ID", id},
                    { "@Delete", true},
                    { "@LogBy", user},
                });
            }
        }

        public List<ScheduleType> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, key },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.FilterScheduleType", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public ScheduleType Get(dynamic id)
        {
            if (id == null) return new ScheduleType();
            var Parameters = new Dictionary<string, object> {
                    { LookupParameters.Table, Table.ScheduleType },
                    { LookupParameters.Id, id }
                };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<ScheduleType> GetList(bool hasDefault = false)
        {
            var scheduleTypes = new List<ScheduleType>();

            var Parameters = new Dictionary<string, object> {
                { LookupParameters.Table, Table.ScheduleType }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    scheduleTypes = ds.GetList(Converter);
                }
            }

            if (hasDefault)
            {
                scheduleTypes.Insert(0, new ScheduleType { ID = 0, Description = "N/A" });
            }

            return scheduleTypes;
        }
    }
}
