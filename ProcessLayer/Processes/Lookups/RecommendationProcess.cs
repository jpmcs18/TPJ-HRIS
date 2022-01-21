using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class RecommendationProcess
    {
        public static readonly RecommendationProcess Instance = new RecommendationProcess();
        private RecommendationProcess() { }
        internal Recommendation Converter(DataRow dr)
        {
            return new Recommendation
            {
                ID = dr["ID"].ToShort(),
                Description = dr["Description"].ToString(),
            };
        }
        public List<Recommendation> GetList()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetRecommendation"))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public Recommendation Get(short id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("lookup.GetRecommendation", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
