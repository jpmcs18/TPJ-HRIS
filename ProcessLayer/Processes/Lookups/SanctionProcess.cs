using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{

    public sealed class SanctionProcess
    {
        public static readonly Lazy<SanctionProcess> Instance = new Lazy<SanctionProcess>(() => new SanctionProcess());
        private SanctionProcess() { }
        internal Sanction Converter(DataRow dr)
        {
            return new Sanction
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
                WithDays = dr["With Days"].ToNullableBoolean(),
                WithDate = dr["With Date"].ToNullableBoolean(),
            };
        }
        public List<Sanction> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetSanction"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public Sanction Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetSanction", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
