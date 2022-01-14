using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class DepartmentProcess : ILookupSourceProcess<Department>
    {
        public static readonly Lazy<DepartmentProcess> Instance = new Lazy<DepartmentProcess>(() => new DepartmentProcess());
        private DepartmentProcess() { }
        internal Department Converter(DataRow dr)
        {
            var p = new Department
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                ShortDescription = dr["ShortDescription"].ToString(),
                Office = dr["Office"].ToNullableBoolean() ?? false
            };

            return p;
        }

        public List<Department> GetListOffice(bool HasDefault = false, bool? isOffice = null)
        {
            var lookups = new List<Department>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetDepartment", new Dictionary<string, object> {
                    { "@IsOffice",  isOffice}
                }))
                {
                    lookups = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                lookups.Insert(0, new Department() { ID = 0, Description = "N/A" });
            }

            return lookups;
        }

        public List<Department> Search(string key)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[SearchDepartment]", new Dictionary<string, object> {
                    { "@Key",  key}
                }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public Department Get(int id)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetDepartment", new Dictionary<string, object> {
                    { "@ID",  id}
                }))
                {
                    return ds.Get(Converter);
                }
            }

        }

        public List<Department> GetList(bool hasDefault = false)
        {
            return GetListOffice(hasDefault);
        }
    }
}
