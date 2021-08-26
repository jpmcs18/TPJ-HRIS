using DBUtilities;
using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public sealed class ParametersProcess : ILookupProcess<Parameters> 
    {
        public static readonly Lazy<ParametersProcess> Instance = new Lazy<ParametersProcess>(() => new ParametersProcess());
        private ParametersProcess() { }
        internal Parameters Converter(DataRow dr)
        {
            return new Parameters
            {
                Description = dr["Description"].ToString(),
                Value = dr["Value"],
                Tag = dr["Tag"].ToString(),
                DisplayName = dr["Display Name"].ToString(),
                Order = dr["Order"].ToInt()

            };
        }

        public List<Parameters> GetParameters(string tag)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetParameter", new Dictionary<string, object> { { "@Tag", tag } }))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public Parameters SaveParameter(Parameters parameter)
        {
            using (var db = new DBTools())
            {
                var param = new Dictionary<string, object> {
                            { "@Tag", ParametersTag.Payroll },
                            { "@Value", parameter.Value },
                            { "@Description", parameter.ID },
                        };
                db.ExecuteNonQuery("lookup.UpdateParameters", param);
            }
            return parameter;
        }

        public void SaveParameters(List<Parameters> parameters)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try {
                    foreach (var parameter in parameters)
                    {
                        var param = new Dictionary<string, object> { 
                            { "@Tag", parameter.Tag },
                            { "@Value", parameter.Value },
                            { "@Description", parameter.Description },
                            { "@Order", parameter.Order }
                        };
                    }
                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollBackTransaction();
                    throw ex;
                }
            }
        }

        public List<Parameters> Filter(string key, int page, int gridCount, out int PageCount)
        {
            PageCount = 1;
            return GetParameters(ParametersTag.Payroll);
        }

        public Parameters Get(dynamic id)
        {
            if (id == null) return new Parameters();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetParameter", new Dictionary<string, object> { { "@Tag", ParametersTag.Payroll}, { "@Description", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public Parameters CreateOrUpdate(string item, int user)
        {
            var parameters = JsonConvert.DeserializeObject<Parameters>(item, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return SaveParameter(parameters);
        }

        public void Delete(dynamic id, int user)
        {
        }
    }
}
