using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class PhilHealthProcess : ILookupProcess<PhilHealth>
    {
        public static readonly PhilHealthProcess Instance = new PhilHealthProcess();
        private PhilHealthProcess() { }
        internal PhilHealth Converter(DataRow dr)
        {
            return new PhilHealth()
            {
                ID = dr["ID"].ToShort(),
                MinSalary = dr["Min Salary"].ToNullableDecimal(),
                MaxSalary = dr["Max Salary"].ToNullableDecimal(),
                Share = dr["Share"].ToNullableDecimal(),
                Rate = dr["Rate"].ToNullableDecimal(),
                DateStart = dr["Date Start"].ToNullableDateTime(),
                DateEnd = dr["Date End"].ToNullableDateTime()
            };
        }

        public PhilHealth CreateOrUpdate(string item, int user)
        {
            var philhealth = JsonConvert.DeserializeObject<PhilHealth>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outparams = new List<OutParameters>() {
                    { "@ID", SqlDbType.SmallInt, philhealth.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdatePhilHealth", ref outparams, new Dictionary<string, object>() {
                    { "@MinSalary", philhealth.MinSalary },
                    { "@MaxSalary", philhealth.MaxSalary },
                    { "@Share", philhealth.Share },
                    { "@Rate", philhealth.Rate },
                    { "@DateStart", philhealth.DateStart },
                    { "@DateEnd", philhealth.DateEnd },
                    { "@LogBy", user },
                });

                philhealth.ID = outparams.Get("@ID").ToShort();

                return philhealth;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdatePhilHealth", new Dictionary<string, object>() {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }

        public List<PhilHealth> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var parameters = new Dictionary<string, object>
            {
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.FilterPhilHealth", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public PhilHealth Get(dynamic id)
        {
            if (id == null) return new PhilHealth();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetPhilHealth", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }

}
