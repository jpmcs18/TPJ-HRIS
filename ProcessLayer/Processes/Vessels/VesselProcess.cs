using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public sealed class VesselProcess : ILookupProcess<Vessel>
    {
        public static readonly Lazy<VesselProcess> Instance = new Lazy<VesselProcess>(() => new VesselProcess());
        private VesselProcess() { }
        internal Vessel Converter(DataRow dr)
        {
            var c = new Vessel
            {
                ID = dr[VesselFields.ID].ToShort(),
                Code = dr[VesselFields.Code].ToString(),
                Description = dr[VesselFields.Description].ToString(),
                GrossTon = dr[VesselFields.GrossTon].ToNullableDecimal(),
                NetTon = dr[VesselFields.NetTon].ToNullableDecimal(),
                HP = dr[VesselFields.HP].ToNullableDecimal(),
                Email = dr[VesselFields.Email].ToString()
            };
            
            return c;
        }
        public Vessel Get(dynamic Id)
        {
            if (Id == null) return new Vessel();
            var Parameters = new Dictionary<string, object>
            {
                { VesselParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselProcedures.Get, Parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<Vessel> GetList()
        {

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselProcedures.Get))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public List<Vessel> Filter(short? vesselID, string code, string description, int page, int gridCount, out int PageCount)
        {
            var Parameters = new Dictionary<string, object>
            {
                { VesselParameters.ID, vesselID},
                { VesselParameters.Code, code},
                { VesselParameters.Description, description},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselProcedures.Filter, ref outParameters, Parameters))
                {
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public List<Vessel> Search(string description)
        {
            var Parameters = new Dictionary<string, object>
            {
                { VesselParameters.Description, description}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselProcedures.Search, Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public Vessel CreateOrUpdate(Vessel vessel, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { VesselParameters.Code, vessel.Code },
                { VesselParameters.Description, vessel.Description },
                { VesselParameters.GrossTon, vessel.GrossTon },
                { VesselParameters.NetTon, vessel.NetTon },
                { VesselParameters.HP, vessel.HP },
                { VesselParameters.Email, vessel.Email },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { VesselParameters.ID, SqlDbType.BigInt, vessel.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                vessel.ID = OutParameters.Get(VesselParameters.ID).ToShort();
            }

            return vessel;
        }
        
        public void Delete(dynamic id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { VesselFields.ID, id },
                { "@Delete", true },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselProcedures.CreateOrUpdate, Parameters);
            }
        }

        public List<Vessel> Filter(string key, int page, int gridCount, out int pageCount)
        {
            return Filter(null, key, key, page, gridCount, out pageCount);
        }

        public Vessel CreateOrUpdate(string item, int user)
        {
            var vessel = JsonConvert.DeserializeObject<Vessel>(item, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return CreateOrUpdate(vessel, user);
        }
    }
}
