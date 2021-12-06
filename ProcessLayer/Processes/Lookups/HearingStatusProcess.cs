using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class HearingStatusProcess
    {
        public static readonly Lazy<HearingStatusProcess> Instance = new Lazy<HearingStatusProcess>(() => new HearingStatusProcess());
        private HearingStatusProcess() { }
        internal HearingStatus Converter(DataRow dr)
        {
            return new HearingStatus
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
            };
        }
        public List<HearingStatus> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetHearingStatus"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public HearingStatus Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetHearingStatus", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
