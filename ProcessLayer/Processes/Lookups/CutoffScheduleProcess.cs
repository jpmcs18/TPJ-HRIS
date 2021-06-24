using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes
{
    public class CutoffScheduleProcess
    {
        private static CutoffSchedule Converter(DataRow dr)
        {
            return new CutoffSchedule()
            {
                ID = dr["ID"].ToInt(),
                Month = dr["Month"].ToByte(),
                Day = dr["Start Day"].ToByte()
            };
        }

        public static CutoffSchedule GetByID(int id)
        {
            CutoffSchedule CutoffSchedule = new CutoffSchedule();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.CutOffSchedule }
            };

            using (DBTools db = new DBTools())
            {
                var dt = db.ExecuteReader(LookupProcedures.GetLookup, Parameters).Tables[0];
                if (dt.Rows.Count > 0)
                    CutoffSchedule = Converter(dt.Rows[0]);
                else
                    throw new Exception();
            }

            return CutoffSchedule;
        }

        public static List<CutoffSchedule> GetList()
        {
            List<CutoffSchedule> CutoffScheduleList = new List<CutoffSchedule>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, Table.CutOffSchedule }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    CutoffScheduleList = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }

            return CutoffScheduleList;
        }

        public static CutoffSchedule CreateOrUpdate(CutoffSchedule CutoffSchedule, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { "@month", CutoffSchedule.Month },
                { "@startday", CutoffSchedule.Day },
                { "@userid", userid }
            };
            var outParameters = new List<OutParameters>
            {
                { "@id", SqlDbType.Int, CutoffSchedule.ID }
            };

            using (DBTools db = new DBTools())
            {
                object ret = db.ExecuteReader("lookup.CreateOrUpdateCutoffSchedule", ref outParameters, Parameters);
                if (ret is null)
                    throw new Exception();
                else
                    CutoffSchedule.ID = outParameters.Get("@id").ToInt();
            }

            return CutoffSchedule;
        }

        public static void Delete(int id, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.CutOffSchedule },
                { LookupParameters.UserId, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LookupProcedures.DeleteLookup, Parameters);
            }
        }

        public static List<CutoffSchedule> GetPage(byte? Filter, int PageNumber, int GridCount, out int Count)
        {
            List<CutoffSchedule> Lookup = new List<CutoffSchedule>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { "@FilterMonth", Filter },
                { FilterParameters.PageNumber, PageNumber },
                { FilterParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.[SearchCutoffSchedule]", Parameters))
                {
                    Lookup = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                }
            }

            return Lookup;
        }
    }
}
