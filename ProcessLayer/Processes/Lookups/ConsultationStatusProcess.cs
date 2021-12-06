using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class ConsultationStatusProcess
    {
        public static readonly Lazy<ConsultationStatusProcess> Instance = new Lazy<ConsultationStatusProcess>(() => new ConsultationStatusProcess());
        private ConsultationStatusProcess() { }
        internal ConsultationStatus Converter(DataRow dr)
        {
            return new ConsultationStatus
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
            };
        }
        public List<ConsultationStatus> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetConsultationStatus"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public ConsultationStatus Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetConsultationStatus", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
