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
    public class PhilHealthProcess : ILookupProcess<PhilHealth>
    {
        private static PhilHealthProcess _instance;

        public static PhilHealthProcess Instance
        {
            get { if (_instance == null) _instance = new PhilHealthProcess(); return _instance; }
        }

        internal PhilHealth Converter(DataRow dr)
        {
            return new PhilHealth()
            {
                ID = dr["ID"].ToShort(),
                MinSalary = dr["Min Salary"].ToNullableDecimal(),
                MaxSalary = dr["Max Salary"].ToNullableDecimal(),
                EmployeeShare = dr["Employee Share"].ToNullableDecimal(),
                EmployerShare = dr["Employer Share"].ToNullableDecimal(),
                EmployeePercentage = dr["Employee Percentage"].ToNullableDecimal(),
                EmployerPercentage = dr["Employer Percentage"].ToNullableDecimal(),
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
                    { "@EmployeeShare", philhealth.EmployeeShare },
                    { "@EmployerShare", philhealth.EmployerShare },
                    { "@EmployeePercentage", philhealth.EmployeePercentage },
                    { "@EmployerPercentage", philhealth.EmployerPercentage },
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
