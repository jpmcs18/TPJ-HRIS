using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class DefaultLookupProcess
    {
        public static readonly Lazy<DefaultLookupProcess> Instance = new Lazy<DefaultLookupProcess>(() => new DefaultLookupProcess());
        private DefaultLookupProcess() { }
        internal Lookup Converter(DataRow dr)
        {
            return new Lookup
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString()
            };
        }
        public List<Lookup> GetLookupPage(string TableName, string Filter, int Page, int GridCount, out int Count)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, TableName },
                { LookupParameters.Filter, Filter },
                { LookupParameters.PageNumber, Page },
                { LookupParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookupPage, Parameters))
                {
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                    return ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }
        }

    }
}
