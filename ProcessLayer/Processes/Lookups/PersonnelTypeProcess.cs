using DBUtilities;
using ProcessLayer.Entities.Lookups;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Lookups
{
    public sealed class PersonnelTypeProcess : ILookupProcess<PersonnelType>, ILookupSourceProcess<PersonnelType>
    {
        public static readonly Lazy<PersonnelTypeProcess> Instance = new Lazy<PersonnelTypeProcess>(() => new PersonnelTypeProcess());
        private PersonnelTypeProcess() { }
        internal PersonnelType Converter(DataRow dr)
        {
            return new PersonnelType()
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString()
            };
        }
        public PersonnelType CreateOrUpdate(string item, int user)
        {
            return null;
        }

        public void Delete(dynamic id, int user)
        {
        }

        public List<PersonnelType> Filter(string key, int page, int gridCount, out int pageCount)
        {
            pageCount = 0;
            return null;
        }

        public PersonnelType Get(dynamic id)
        {
            if (id == null) return new PersonnelType();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetPersonnelTypes", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<PersonnelType> GetList(bool hasDefault = false)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetPersonnelTypes"))
                {
                    var res = ds.GetList(Converter);
                    if (hasDefault)
                        res.Insert(0, new PersonnelType() { Description = "N/A" });
                    return res;
                }
            }
        }
    }
}
