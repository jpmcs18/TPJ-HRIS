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
    public sealed class SSSProcess : ILookupProcess<SSS>
    {
        public static readonly SSSProcess Instance = new SSSProcess();
        private SSSProcess() { }

        internal SSS Converter(DataRow dr)
        {
            return new SSS()
            {
                ID = dr["ID"].ToInt(),
                MinSalary = dr["Min Salary"].ToNullableDecimal(),
                MaxSalary = dr["Max Salary"].ToNullableDecimal(),
                EmployeeShare = dr["Employee Share"].ToNullableDecimal(),
                EmployerShare = dr["Employer Share"].ToNullableDecimal(),
                EC = dr["EC"].ToNullableDecimal(),
                ProvES = dr["Prov ES"].ToNullableDecimal(),
                ProvPS = dr["Prov PS"].ToNullableDecimal(),
                DateStart = dr["Date Start"].ToNullableDateTime(),
                DateEnd = dr["Date End"].ToNullableDateTime()
            };
        }

        public SSS CreateOrUpdate(string item, int user)
        {
            var SSS = JsonConvert.DeserializeObject<SSS>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outparams = new List<OutParameters>() {
                    { "@ID", SqlDbType.SmallInt, SSS.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateSSS", ref outparams, new Dictionary<string, object>() {
                    { "@MinSalary", SSS.MinSalary },
                    { "@MaxSalary", SSS.MaxSalary },
                    { "@EmployeeShare", SSS.EmployeeShare },
                    { "@EmployerShare", SSS.EmployerShare },
                    { "@EC", SSS.EC },
                    { "@ProvES", SSS.ProvES },
                    { "@ProvPS", SSS.ProvPS },
                    { "@DateStart", SSS.DateStart },
                    { "@DateEnd", SSS.DateEnd },
                    { "@LogBy", user },
                });

                SSS.ID = outparams.Get("@ID").ToShort();

                return SSS;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateSSS", new Dictionary<string, object>() {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }

        public List<SSS> Filter(string key, int page, int gridCount, out int pageCount)
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
                using (var ds = db.ExecuteReader("lookup.FilterSSS", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public SSS Get(dynamic id)
        {
            if (id == null) return new SSS();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetSSS", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }

}
