using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class TaxTableProcess : ILookupProcess<TaxTable>
    {
        public static readonly Lazy<TaxTableProcess> Instance = new Lazy<TaxTableProcess>(() => new TaxTableProcess());
        private TaxTableProcess() { }

        internal TaxTable Converter(DataRow dr)
        {
            var tax =  new TaxTable
            {
                ID = dr["ID"].ToLong(),
                MinimumIncome = dr["Minimum Income"].ToNullableDecimal(),
                MaximumIncome = dr["Maximum Income"].ToNullableDecimal(),
                FixedTax = dr["Fixed Tax"].ToNullableDecimal(),
                AdditionalTax = dr["Additional Tax"].ToNullableDecimal(),
                ExcessOver = dr["Excess Over"].ToNullableDecimal(),
                TaxScheduleID = dr["Tax Schedule ID"].ToNullableInt(),
                EffectiveStartDate = dr["Effective Start Date"].ToNullableDateTime(),
                EffectiveEndDate = dr["Effective End Date"].ToNullableDateTime()
            };

            tax.TaxSchedule = TaxScheduleProcess.Instance.Value.Get(tax.TaxScheduleID ?? 0);
            return tax;
        }

        public List<TaxTable> GetList(int? taxschedid = null, DateTime? date = null)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetTaxTable", new Dictionary<string, object> { { "@TaxSchedID", taxschedid }, { "@Date", date } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public List<TaxTable> Filter(string key, int page, int gridCount, out int pageCount)
        {
            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, key},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[FilterTaxTable]", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public TaxTable Get(dynamic id)
        {
            if (id == null) return new TaxTable();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetTaxTable", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public TaxTable CreateOrUpdate(string item, int user)
        {
            var taxtable = JsonConvert.DeserializeObject<TaxTable>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParams = new List<OutParameters>() {
                    { "@ID", SqlDbType.BigInt, taxtable.ID }
                };

                db.ExecuteNonQuery("lookup.CreateOrUpdateTaxTable", ref outParams, new Dictionary<string, object> { 
                    { "@MinimumIncome", taxtable.MinimumIncome }, 
                    { "@MaximumIncome", taxtable.MaximumIncome}, 
                    { "@FixedTax", taxtable.FixedTax }, 
                    { "@AdditionalTax", taxtable.AdditionalTax }, 
                    { "@ExcessOver", taxtable.ExcessOver }, 
                    { "@TaxScheduleID", taxtable.TaxScheduleID }, 
                    { "@EffectiveStartDate", taxtable.EffectiveStartDate }, 
                    { "@EffectiveEndDate", taxtable.EffectiveEndDate }, 
                    { "@LogBy", user }, 
                });

                taxtable.ID = outParams.Get("@ID").ToLong();

                return taxtable;
            }
        }

        public void Delete(dynamic id, int user)
        {
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateTaxTable", new Dictionary<string, object> {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy", user },
                });
            }
        }
    }
}
