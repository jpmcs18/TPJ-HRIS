using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class WrittenExplanationStatusProcess
    {
        public static readonly Lazy<WrittenExplanationStatusProcess> Instance = new Lazy<WrittenExplanationStatusProcess>(() => new WrittenExplanationStatusProcess());
        private WrittenExplanationStatusProcess() { }
        internal WrittenExplanationStatus Converter(DataRow dr)
        {
            return new WrittenExplanationStatus
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
            };
        }
        public List<WrittenExplanationStatus> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetWrittenExplanationStatus"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public WrittenExplanationStatus Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetWrittenExplanationStatus", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
