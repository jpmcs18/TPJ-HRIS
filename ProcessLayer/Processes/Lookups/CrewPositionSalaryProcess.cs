using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class CrewPositionSalaryProcess : ILookupProcess<CrewPositionSalary>
    {
        public static readonly CrewPositionSalaryProcess Instance = new CrewPositionSalaryProcess();
        private CrewPositionSalaryProcess() { }
        internal CrewPositionSalary Converter(DataRow dr)
        {
            var p = new CrewPositionSalary
            {
                ID = dr["ID"].ToInt(),
                PositionID = dr["Position ID"].ToInt(),
                FishingGroundRate = dr["Fishing Ground Rate"].ToNullableDecimal(),
                StandbyGroundRate = dr["Standby Ground Rate"].ToNullableDecimal()
            };

            p.Position = PositionProcess.Instance.Get(p.PositionID);
            return p;
        }

        public CrewPositionSalary CreateOrUpdate(string item, int user)
        {
            var ps = JsonConvert.DeserializeObject<CrewPositionSalary>(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> {
                    { "@ID", SqlDbType.Int, ps.ID }
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdateCrewPositionSalary", ref outParam, new Dictionary<string, object> {
                    { "@PositionID", ps.PositionID},
                    { "@FishingGroundRate", ps.FishingGroundRate},
                    { "@StandbyGroundRate", ps.StandbyGroundRate},
                    { "@LogBy", user},
                });
                ps.ID = outParam.Get("@ID").ToInt();
            }
            return ps;
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateCrewPositionSalary", new Dictionary<string, object> {
                    { "@ID", id},
                    { "@LogBy", user},
                });
            }
        }

        public List<CrewPositionSalary> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var outParameters = new List<OutParameters>
            {
                { "@PageCount", SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.FilterCrewPositionSalary", ref outParameters, new Dictionary<string, object> {
                    { "@Filter", key},
                    { "@PageNumber", page },
                    { "@GridCount", gridCount } })
                )
                {
                    pageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public CrewPositionSalary Get(dynamic id)
        {
            if (id == null) return new CrewPositionSalary();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetCrewPositionSalary", new Dictionary<string, object> { { "@ID", id} }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public CrewPositionSalary GetDefaultSalary(int positionID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetDefaultCrewSalary", new Dictionary<string, object> { { "@PositionID", positionID } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
