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
    public class PersonnelTypeProcess : ILookupProcess<PersonnelType>, ILookupSourceProcess<PersonnelType>
    {
        private static PersonnelTypeProcess _instance;

        public static PersonnelTypeProcess Instance
        {
            get { if (_instance == null) _instance = new PersonnelTypeProcess(); return _instance; }
        }

        protected PersonnelType Converter(DataRow dr)
        {
            return new PersonnelType()
            {
                ID = dr["ID"].ToByte(),
                Description = dr["Description"].ToString()
            };
        }
        public PersonnelType CreateOrUpdate(string item, int user)
        {
            throw new NotImplementedException();
        }

        public void Delete(dynamic id, int user)
        {
            throw new NotImplementedException();
        }

        public List<PersonnelType> Filter(string key, int page, int gridCount, out int pageCount)
        {
            throw new NotImplementedException();
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
