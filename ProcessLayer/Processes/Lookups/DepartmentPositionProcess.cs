using DBUtilities;
using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using ProcessLayer.Processes.Lookups;
using Newtonsoft.Json;
using System;

namespace ProcessLayer.Processes
{
    public sealed class DepartmentPositionProcess : ILookupProcess<DepartmentPosition>
    {
        public static readonly Lazy<DepartmentPositionProcess> Instance = new Lazy<DepartmentPositionProcess>(() => new DepartmentPositionProcess());
        private DepartmentPositionProcess() { }
        internal DepartmentPosition Converter(DataRow dr)
        {
            var dept = new DepartmentPosition
            {
                ID = dr["ID"].ToInt(),
                PositionID = dr["Position ID"].ToByte(),
                DepartmentID = dr["Department ID"].ToInt()
            };

            dept.Department = DepartmentProcess.Instance.Value.Get(dept.DepartmentID ?? 0);
            dept.Position = PositionProcess.Instance.Value.Get(dept.PositionID);
            
            return dept;
        }
        
        public List<DepartmentPosition> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, key},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[FilterDepartmentPosition]", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public List<DepartmentPosition> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetDepartmentPosition"))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public DepartmentPosition Get(dynamic id)
        {
            if (id == null) return new DepartmentPosition();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetDepartmentPosition", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public DepartmentPosition CreateOrUpdate(string item, int userid)
        {
            var departmentPosition = JsonConvert.DeserializeObject<DepartmentPosition>(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> {
                    { "@ID", SqlDbType.Int, departmentPosition.ID }
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdateDepartmentPosition", ref outParam, new Dictionary<string, object> {
                    { "@DepartmentID", departmentPosition.DepartmentID},
                    { "@PositionID", departmentPosition.PositionID},
                    { "@LogBy", userid},
                });
                departmentPosition.ID = outParam.Get("@ID").ToInt();
            }
            return departmentPosition;
        }

        public void Delete(dynamic id, int userid)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateDepartmentPosition", new Dictionary<string, object> {
                    { "@ID", id},
                    { "@Delete", true},
                    { "@LogBy", userid},
                });
            }
        }
    }
}
