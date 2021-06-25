using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public class LateDeductionProcess : ILookupProcess<LateDeduction>
    {
        private static LateDeductionProcess _instance;
        public static LateDeductionProcess Instance
        {
            get { if (_instance == null) _instance = new LateDeductionProcess(); return _instance; }
        }
        internal LateDeduction Converter(DataRow dr)
        {
            return new LateDeduction
            {
                ID = dr["ID"].ToShort(),
                DeductedHours = dr["Deducted Hours"].ToNullableByte(),
                TimeIn = dr["TIme In"].ToNullableTimeSpan()
            };
        }

        public LateDeduction CreateOrUpdate(string item, int user)
        {
            var lateDed = JsonConvert.DeserializeObject<LateDeduction>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@DeductedHours", lateDed.DeductedHours },
                    { "@TimeIn", lateDed.TimeIn  },
                    { "@LogBy", user }
                };

                var outParameters = new List<OutParameters> {
                    { "@ID", SqlDbType.SmallInt, lateDed.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateLateDeduction", ref outParameters, parameters);
                lateDed.ID = outParameters.Get("@ID").ToShort();
                
                return lateDed;
            }
        }

        public void Delete(dynamic id, int user)
        {
            //using (var db = new DBTools())
            //{
            //    var parameters = new Dictionary<string, object>
            //    {
            //        { "@ID", id },
            //        { "@Delete", true },
            //        { "@LogBy", user }
            //    };
            //    db.ExecuteNonQuery("lookup.CreateOrUpdateLateDeduction", parameters);
            //}
        }

        public List<LateDeduction> Filter(string key, int page, int gridCount, out int pageCount)
        {
            pageCount = 0;
            return GetList();
        }

        public List<LateDeduction> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetLateDeduction"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public LateDeduction Get(dynamic id)
        {
            if (id == null) return new LateDeduction();
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetLateDeduction", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
