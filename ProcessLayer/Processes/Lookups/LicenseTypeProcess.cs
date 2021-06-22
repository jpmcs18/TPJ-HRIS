using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProcessLayer.Processes
{
    public class LicenseTypeProcess
    {
        static string tableName = "License Type";

        private static LicenseType Converter(DataRow dr)
        {
            return new LicenseType()
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                Perpetual = dr["Perpetual"].ToNullableBoolean() ?? false
            };
        }

        public static LicenseType GetByID(int id)
        {
            LicenseType LicenseType = new LicenseType();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.LicenseType }
            };

            using (DBTools db = new DBTools())
            {
                var dt = db.ExecuteReader(LookupProcedures.GetLookup, Parameters).Tables[0];
                if (dt.Rows.Count > 0)
                    LicenseType = Converter(dt.Rows[0]);
                else
                    throw new Exception();
            }

            return LicenseType;
        }

        public static List<LicenseType> GetList()
        {
            List<LicenseType> LicenseTypeList = new List<LicenseType>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, tableName }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    LicenseTypeList = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }

            return LicenseTypeList;
        }

        public static LicenseType CreateOrUpdate(LicenseType LicenseType, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { "@description", LicenseType.Description },
                { "@perpetual", LicenseType.Perpetual },
                { "@userid", userid }
            };
            var outParameters = new List<OutParameters>
            {
                { "@id", SqlDbType.Int, LicenseType.ID }
            };

            using (DBTools db = new DBTools())
            {
                object ret = db.ExecuteReader("lookup.CreateOrUpdateLicenseType", ref outParameters, Parameters);
                if (ret is null)
                    throw new Exception();
                else
                    LicenseType.ID = outParameters.Get("@id").ToInt();
            }

            return LicenseType;
        }

        public static void Delete(int id, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.LicenseType },
                { LookupParameters.UserId, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LookupProcedures.DeleteLookup, Parameters);
            }
        }

        public static List<LicenseType> GetPage(string Filter, bool? IsPerpetual, int PageNumber, int GridCount, out int Count)
        {
            List<LicenseType> LicenseType = new List<LicenseType>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { FilterParameters.Filter, Filter },
                { FilterParameters.IsPerpetual, IsPerpetual },
                { FilterParameters.PageNumber, PageNumber },
                { FilterParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.[SearchLicenseType]", Parameters))
                {
                    LicenseType = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                }
            }

            return LicenseType;
        }
    }
}
