using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class InfractionStatusProcess
    {
        public static readonly InfractionStatusProcess Instance = new InfractionStatusProcess();
        private InfractionStatusProcess() { }
        internal InfractionStatus Converter(DataRow dr)
        {
            return new InfractionStatus
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
            };
        }
        public List<InfractionStatus> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetInfractionStatus"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public InfractionStatus Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetInfractionStatus", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
