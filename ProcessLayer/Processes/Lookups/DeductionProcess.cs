using DBUtilities;
using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Processes.Lookups;
using Newtonsoft.Json;
using System;

namespace ProcessLayer.Processes
{
    public sealed class DeductionProcess : ILookupProcess<Deduction>
    {
        public static readonly DeductionProcess Instance = new DeductionProcess();
        private DeductionProcess() { }

        public Deduction Converter(DataRow dr)
        {
            var r = new Deduction {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                WhenToDeduct = dr["When to Deduct"].ToNullableInt(),
                GovernmentDeduction = dr["Government Deduction"].ToNullableBoolean(),
                AutoCompute = dr["Auto Compute"].ToNullableBoolean(),
                ComputedThruSalary = dr["Computed thru Salary"].ToNullableBoolean()
            };
            r.Deduct = WhenToDeductProcess.Instance.Get(r.WhenToDeduct ?? 0);
            return r;
        }

        public Deduction CreateOrUpdate(string item, int user)
        {
            var deduction = JsonConvert.DeserializeObject<Deduction>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (var db = new DBTools())
            {
                var outParam = new List<OutParameters> { { "@ID", SqlDbType.TinyInt,  deduction.ID} };
                var parameters = new Dictionary<string, object>
                {
                    { "@Description", deduction.Description },
                    { "@WhenToDeduct", deduction.WhenToDeduct },
                    { "@ComputeThruSalary", deduction.ComputedThruSalary },
                    { "@GovernmentDeduction", deduction.GovernmentDeduction },
                    { "@AutoCompute", deduction.AutoCompute },
                    { "@LogBy", user },
                };
                db.ExecuteNonQuery("lookup.CreateOrUpdateDeduction", ref outParam, parameters);
                deduction.ID = outParam.Get("@ID").ToInt();
                return deduction;
            }
        }

        public void Delete(dynamic id, int user)
        {
        }

        public List<Deduction> Filter(string filter, int page, int gridCount, out int pageCount)
        {
            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[FilterDeduction]", ref outParameters, Parameters))
                {
                    pageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public Deduction Get(dynamic id)
        {
            if (id == null) return new Deduction();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[lookup].[GetDeduction]",new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }
    }
}
