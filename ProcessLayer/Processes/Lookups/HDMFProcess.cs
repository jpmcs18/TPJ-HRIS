using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class HDMFProcess : ILookupProcess<HDMF>
    {
        public static readonly Lazy<HDMFProcess> Instance = new Lazy<HDMFProcess>(() => new HDMFProcess());
        private HDMFProcess() { }

        internal HDMF Converter(DataRow dr)
        {
            return new HDMF()
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

        public HDMF CreateOrUpdate(string item, int user)
        {
            var HDMF = JsonConvert.DeserializeObject<HDMF>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outparams = new List<OutParameters>() {
                    { "@ID", SqlDbType.SmallInt, HDMF.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateHDMF", ref outparams, new Dictionary<string, object>() {
                    { "@MinSalary", HDMF.MinSalary },
                    { "@MaxSalary", HDMF.MaxSalary },
                    { "@EmployeeShare", HDMF.EmployeeShare },
                    { "@EmployerShare", HDMF.EmployerShare },
                    { "@EmployeePercentage", HDMF.EmployeePercentage },
                    { "@EmployerPercentage", HDMF.EmployerPercentage },
                    { "@DateStart", HDMF.DateStart },
                    { "@DateEnd", HDMF.DateEnd },
                    { "@LogBy", user },
                });

                HDMF.ID = outparams.Get("@ID").ToShort();

                return HDMF;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateHDMF", new Dictionary<string, object>() {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }

        public List<HDMF> Filter(string key, int page, int gridCount, out int pageCount)
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
                using (var ds = db.ExecuteReader("lookup.FilterHDMF", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public HDMF Get(dynamic id)
        {
            if (id == null) return new HDMF();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetHDMF", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }

}
