using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public sealed class LocationProcess : ILookupProcess<Location>, ILookupSourceProcess<Location>
    {
        public static readonly LocationProcess Instance = new LocationProcess();
        private LocationProcess() { }
        internal Location Converter(DataRow dr)
        {
            return new Location
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                Prefix = dr["Prefix"].ToString(),
                HazardRate = dr["Hazard Rate"].ToNullableDecimal() ?? 0,
                RequiredTimeLog = dr["Required Time Log"].ToNullableBoolean(),
                WithHolidayAndSunday = dr["With Holiday And Sunday"].ToNullableBoolean(),
                WithAdditionalForExtension = dr["With Additional For Extension"].ToNullableBoolean()
            };
        }

        public List<Location> GetList(bool HasDefault = false)
        {
            List<Location> locations = new List<Location>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup,
                    new Dictionary<string, object> {
                        { LookupParameters.Table, "Location" },
                        { LookupParameters.Schema, "lookup"},
                    }))
                {
                    locations = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                locations.Insert(0, new Location { ID = 0, Description = "N/A" });
            }
            return locations;
        }

        public List<Location> Filter(string key, int page, int gridCount, out int pageCount)
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
                using (var ds = db.ExecuteReader("[lookup].[FilterLocation]", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public Location Get(dynamic id)
        {
            if (id == null) return new Location();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup,
                    new Dictionary<string, object> {
                        { LookupParameters.Table, "Location" },
                        { LookupParameters.Id, (id ?? 0) },
                        { LookupParameters.Schema, "lookup"},
                    }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public Location CreateOrUpdate(string item, int user)
        {
            var location = JsonConvert.DeserializeObject<Location>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParams = new List<OutParameters> { { "@ID", SqlDbType.TinyInt, location.ID } };
                db.ExecuteNonQuery("lookup.CreateOrUpdateLocations", ref outParams, new Dictionary<string, object> {
                    { "@Description", location.Description },
                    { "@Prefix", location.Prefix },
                    { "@OfficeLocation", location.OfficeLocation },
                    { "@WarehouseLocation", location.WarehouseLocation },
                    { "@HazardRate", location.HazardRate },
                    { "@RequiredTimeLog", location.RequiredTimeLog },
                    { "@WithHolidayAndSunday", location.WithHolidayAndSunday },
                    { "@WithAdditionalForExtension", location.WithAdditionalForExtension },
                    { "@LogBy", user },
                });
                location.ID = outParams.Get("@ID").ToInt();
                return location;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateLocations", new Dictionary<string, object> { 
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }
    }
}
