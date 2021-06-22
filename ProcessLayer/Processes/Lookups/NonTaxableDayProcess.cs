using DBUtilities;
using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Processes.Lookups;
using Newtonsoft.Json;
using System;

namespace ProcessLayer.Processes
{
    public class NonTaxableDayProcess : ILookupProcess<NonTaxableDay>
    {
        private static NonTaxableDayProcess _instance;
        public static NonTaxableDayProcess Instance
        {
            get { if (_instance == null) _instance = new NonTaxableDayProcess(); return _instance; }
        }

        public NonTaxableDay Converter(DataRow dr)
        {
            var n = new NonTaxableDay
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                StartDate = dr["Start Date"].ToNullableDateTime(),
                EndDate = dr["End Date"].ToNullableDateTime(),
                LocationID = dr["Location ID"].ToNullableInt(),
                IsGlobal = dr["Is Global"].ToNullableBoolean(),
            };
            n.Location = LocationProcess.Instance.Get(n.LocationID);
            return n;
        }
        public NonTaxableDay CreateOrUpdate(string item, int user)
        {
            var nonTaxable = JsonConvert.DeserializeObject<NonTaxableDay>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParams = new List<OutParameters>
                {
                    { "@ID", SqlDbType.Int, nonTaxable.ID }
                };

                var parameters = new Dictionary<string, object> {
                    { "@Description", nonTaxable.Description },
                    { "@StartDate", nonTaxable.StartDate },
                    { "@EndDate", nonTaxable.EndDate },
                    { "@LocationID", nonTaxable.LocationID },
                    { "@IsGlobal", nonTaxable.IsGlobal },
                    { "@LogBy", user },
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateNonTaxableDay", ref outParams, parameters);
                nonTaxable.ID = outParams.Get("@ID").ToInt();
                return nonTaxable;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateNonTaxableDay", parameters);
            }
        }

        public List<NonTaxableDay> Filter(string key, int page, int gridCount, out int pageCount)
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
                using (var ds = db.ExecuteReader("lookup.FilterNonTaxableDay", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public NonTaxableDay Get(dynamic id)
        {
            if (id == null) return new NonTaxableDay();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetNonTaxableDay", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public List<NonTaxableDay> GetList(DateTime? start, DateTime? end)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetNonTaxableDay", new Dictionary<string, object> { 
                    { "@StartDate", start }, 
                    { "@EndDate", end }
                }))
                {
                    return ds.GetList(Converter);
                }
            }
        }
    }
}
