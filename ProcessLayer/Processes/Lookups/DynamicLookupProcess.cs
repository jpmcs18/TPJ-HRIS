using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public class DynamicLookupProcess : ILookupProcess
    {
        private static DynamicLookupProcess _instance;

        public static DynamicLookupProcess Instance
        {
            get { if (_instance == null) _instance = new DynamicLookupProcess(); return _instance; }
        }
        internal Lookup Converter(DataRow dr)
        {
            return new Lookup
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString()
            };
        }

        public Lookup CreateOrUpdate(string table, string item, int user)
        {
            Lookup lookup = JsonConvert.DeserializeObject<Lookup>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@Table", table },
                    { "@Description", lookup.Description },
                    { "@LogBy", user }
                };

                var outParameters = new List<OutParameters> {
                    { "@ID", SqlDbType.Int, lookup.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateDynamicLookup", ref outParameters, parameters);
                lookup.ID = outParameters.Get("@ID").ToInt();

                return lookup;
            }
        }

        public void Delete(string table, dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@Table", table },
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateDynamicLookup", parameters);
            }
        }

        public List<Lookup> Filter(string table, string key, int page, int gridCount, out int pageCount)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Table", table },
                    { "@Filter", key },
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                };

                var outParameters = new List<OutParameters> {
                    { "@PageCount", SqlDbType.Int }
                };

                using (var ds = db.ExecuteReader("lookup.FilterDynamicLookup", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get("@PageCount").ToNullableInt() ?? 0;
                    return ds.GetList(Converter);
                }
            }
        }

        public Lookup Get(string table, dynamic id)
        {
            if (id == null) return new Lookup();
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Table", table },
                    { "@ID", id }
                };
                using (var ds = db.ExecuteReader("lookup.GetDynamicLookup", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }


    }

}
