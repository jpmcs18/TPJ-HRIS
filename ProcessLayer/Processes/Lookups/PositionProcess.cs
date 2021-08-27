using DBUtilities;
using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Processes.Lookups;
using Newtonsoft.Json;
using System;

namespace ProcessLayer.Processes
{
    public sealed class PositionProcess : ILookupProcess<Position>, ILookupSourceProcess<Position> 
    {
        public static readonly Lazy<PositionProcess> Instance = new Lazy<PositionProcess>(() => new PositionProcess());
        private PositionProcess() { }
        internal Position Converter(DataRow dr)
        {
            var p = new Position
            {
                ID = dr["ID"].ToByte(),
                Description = dr["Description"].ToString(),
                Abbreviation = dr["Abbreviation"].ToString(),
                PersonnelTypeID = dr["Personnel Type ID"].ToNullableInt(),
                AllowApprove = dr["Allow Approve"].ToNullableBoolean()
            };

            p.PersonnelType = PersonnelTypeProcess.Instance.Value.Get(p.PersonnelTypeID);
            return p;
        }

        public List<Position> GetList(string filter, int page, int gridCount, out int pageCount) {

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[FilterPosition]", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public IEnumerable<Position> GetByPersonnelType(int? personnelTypeId = null, bool HasDefault = false)
        {
            var lookups = new List<Position>();

            var Parameters = new Dictionary<string, object> {
                { "@PersonnelTypeID", personnelTypeId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetPositionBaseOnPersonnelType", Parameters))
                {
                    lookups = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                lookups.Insert(0, new Position() { ID = 0, Description = "N/A", Abbreviation = "N/A" });
            }

            return lookups;
        }
        public IEnumerable<Position> GetByDepartmentAndPersonnelType(int? departmentId, int? personnelTypeId, bool HasDefault = false)
        {
            var lookups = new List<Position>();

            var Parameters = new Dictionary<string, object> {
                { "@DepartmentID", departmentId },
                { "@PersonnelTypeID", personnelTypeId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetPositionByDepartmentBaseOnPersonnelType", Parameters))
                {
                    lookups = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                lookups.Insert(0, new Position() { ID = 0, Description = "N/A", Abbreviation = "N/A" });
            }

            return lookups;
        }
        public List<Position> GetList(bool HasDefault = false)
        {
            var lookups = new List<Position>();

            var Parameters = new Dictionary<string, object> {
                { LookupParameters.Table, Table.Position }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    lookups = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                lookups.Insert(0, new Position() { ID = 0, Description = "N/A", Abbreviation = "N/A" });
            }

            return lookups;
        }

        public Position SavePosition(Position position, int userid)
        {
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> {
                    { "@ID", SqlDbType.TinyInt, position.ID }
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdatePosition", ref outParam, new Dictionary<string, object> {
                    { "@Description", position.Description},
                    { "@Abbreviation", position.Abbreviation},
                    { "@AllowApprove", position.AllowApprove},
                    { "@PersonnelTypeID", position.PersonnelTypeID},
                    { "@LogBy", userid},
                });
                position.ID = outParam.Get("@ID").ToByte();
                return position;
            }

        }

        public void Delete(dynamic id, int userid)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdatePosition", new Dictionary<string, object> {
                    { "@ID", id},
                    { "@Delete", true},
                    { "@LogBy", userid},
                });
            }
        }

        public List<Position> Filter(string key, int page, int gridCount, out int PageCount)
        {
            return GetList(key, page, gridCount, out PageCount);
        }

        public Position Get(dynamic id)
        {
            try
            {
                if (id == null) return new Position();
                var Parameters = new Dictionary<string, object> {
                    { LookupParameters.Table, Table.Position },
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
            catch { return new Position(); }
        }

        public Position CreateOrUpdate(string item, int user)
        {
            var position = JsonConvert.DeserializeObject<Position>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return SavePosition(position, user);
        }
    }
}
