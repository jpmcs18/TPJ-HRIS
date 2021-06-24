using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public class NonWorkingDaysProcess : ILookupProcess<NonWorkingDays>
    {
        private static NonWorkingDaysProcess _instance;
        public static NonWorkingDaysProcess Instance
        {
            get { if (_instance == null) _instance = new NonWorkingDaysProcess(); return _instance; }
        }

        protected NonWorkingDays Converter(DataRow dr)
        {
            var r = new NonWorkingDays
            {
                ID = dr["ID"].ToLong(),
                Year = dr["Year"].ToNullableShort(),
                Day = dr["Day"].ToNullableDateTime(),
                StartTime = dr["Start Time"].ToNullableTimeSpan(),
                EndTime = dr["End Time"].ToNullableTimeSpan(),
                Description = dr["Description"].ToString(),
                NonWorkingType = dr["Non-Working Type"].ToNullableByte(),
                LocationID = dr["Location ID"].ToNullableInt(),
                Yearly = dr["Yearly"].ToNullableBoolean(),
                IsGlobal = dr["Is Global"].ToNullableBoolean(),
            };
            r.Type = NonWorkingTypeProcess.Instance.Get(r.NonWorkingType ?? 0);
            r.Location = LocationProcess.Instance.Get(r.LocationID);
            return r;
        }

        public List<NonWorkingDays> GetNonWorkingDays(DateTime cutoffstart, DateTime cutoffend)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetNonWorkingDays", 
                    new Dictionary<string, object> { { "@CutOffStartDate", cutoffstart }, { "@CutOffEndDate", cutoffend } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public List<NonWorkingDays> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, key },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.FilterNonWorking", ref outParameters, parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public NonWorkingDays Get(dynamic id)
        {
            if (id == null) return new NonWorkingDays();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetSpecificNonWorkingDays", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public NonWorkingDays CreateOrUpdate(string item, int user)
        {
            var nonWorkingDays = JsonConvert.DeserializeObject<NonWorkingDays>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return SaveNonWorkingDays(nonWorkingDays);
        }

        private NonWorkingDays SaveNonWorkingDays(NonWorkingDays nonWorkingDays)
        {
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> {
                    { "@ID", SqlDbType.BigInt, nonWorkingDays.ID }
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdateNonWorkingDays"
                    , ref outParam
                    , new Dictionary<string, object> {
                        { "@Year", nonWorkingDays.Year},
                        { "@Day", nonWorkingDays.Day},
                        { "@StartTime", nonWorkingDays.StartTime},
                        { "@EndTime", nonWorkingDays.EndTime},
                        { "@Description", nonWorkingDays.Description},
                        { "@NonWorkingType", nonWorkingDays.NonWorkingType},
                        { "@LocationID", nonWorkingDays.LocationID },
                        { "@IsGlobal", nonWorkingDays.IsGlobal },
                        { "@Yearly", nonWorkingDays.Yearly},
                });
                nonWorkingDays.ID = outParam.Get("@ID").ToLong();
                return nonWorkingDays;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateNonWorkingDays"
                    , new Dictionary<string, object> {
                        { "@ID", id },
                        { "@Delete", true},
                });
            }
        }
    }

}
