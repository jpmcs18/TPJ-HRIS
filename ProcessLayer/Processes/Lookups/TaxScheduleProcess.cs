using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public class TaxScheduleProcess : ILookupSourceProcess<TaxSchedule>
    {
        private static TaxScheduleProcess _instance;
        public static TaxScheduleProcess Instance
        {
            get { if (_instance == null) _instance = new TaxScheduleProcess(); return _instance; }
        }

        internal TaxSchedule Converter(DataRow dr)
        {
            return new TaxSchedule()
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString()
            };
        }

        public TaxSchedule Get(int id)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetTaxSchedule", new Dictionary<string, object>() { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public List<TaxSchedule> GetList(bool hasDefault = false)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetTaxSchedule"))
                {
                    var lst = ds.GetList(Converter);
                    if (lst == null) lst = new List<TaxSchedule>();
                    if (hasDefault) lst.Add(new TaxSchedule() { Description = "N/A" });
                    return lst;
                }
            }
        }
    }
}
