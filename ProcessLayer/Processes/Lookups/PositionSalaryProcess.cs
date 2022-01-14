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
    public class PositionSalaryProcess
    {
        private static PositionSalary Converter(DataRow dr, bool WithLookup = false)
        {
            return new PositionSalary()
            {
                ID = dr["ID"].ToInt(),
                PositionID = dr["Position ID"].ToInt(),
                Salary = dr["Salary"].ToDouble(),
                Position = WithLookup
                    ? new Position()
                        {
                            ID = dr["Position ID"].ToInt(),
                            Description = dr["Position Description"].ToString()
                        }
                    : null
            };
        }

        public static PositionSalary GetByID(int id)
        {
            PositionSalary PositionSalary = new PositionSalary();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.PositionSalary }
            };

            using (DBTools db = new DBTools())
            {
                var dt = db.ExecuteReader(LookupProcedures.GetLookup, Parameters).Tables[0];
                if (dt.Rows.Count > 0)
                    PositionSalary = Converter(dt.Rows[0]);
                //else
                //    throw new Exception();
            }

            return PositionSalary;
        }

        public static List<PositionSalary> GetList()
        {
            List<PositionSalary> PositionSalaryList = new List<PositionSalary>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, Table.PositionSalary }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    PositionSalaryList = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }

            return PositionSalaryList;
        }

        public static PositionSalary CreateOrUpdate(PositionSalary PositionSalary, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { "@positionid", PositionSalary.PositionID },
                { "@salary", PositionSalary.Salary },
                { "@userid", userid }
            };
            var outParameters = new List<OutParameters>
            {
                { "@id", SqlDbType.Int, PositionSalary.ID }
            };

            using (DBTools db = new DBTools())
            {
                object ret = db.ExecuteReader("lookup.CreateOrUpdatePositionSalary", ref outParameters, Parameters);
                if (ret is null)
                    throw new Exception();
                else
                    PositionSalary.ID = outParameters.Get("@id").ToInt();
            }

            return PositionSalary;
        }

        public static void Delete(int id, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, Table.PositionSalary },
                { LookupParameters.Id, id },
                { LookupParameters.UserId, userid },
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LookupProcedures.DeleteLookup, Parameters);
            }
        }

        public static List<PositionSalary> GetPage(string Filter, int Page, int GridCount, out int Count)
        {
            List<PositionSalary> PositionSalaryList = new List<PositionSalary>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { FilterParameters.Filter, Filter },
                { FilterParameters.PageNumber, Page },
                { FilterParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.[SearchPositionSalary]", Parameters))
                {
                    PositionSalaryList = ds.Tables[0].AsEnumerable().Select(r => Converter(r, true)).ToList();
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                }
            }

            return PositionSalaryList;
        }

        //public static List<PositionSalary> GetPositionSalaryWithPosition(int? id = null)
        //{
        //    List<PositionSalary> PositionSalaryList = new List<PositionSalary>();
        //    Dictionary<string, object> Parameters = new Dictionary<string, object>()
        //    {
        //        { "@id", id }
        //    };

        //    using (DBTools db = new DBTools())
        //    {
        //        using (var ds = db.ExecuteReader("lookup.GetPositionSalaryWithPosition", Parameters))
        //        {
        //            PositionSalaryList = ds.Tables[0].AsEnumerable().Select(r => Converter(r, true)).ToList();
        //        }
        //    }

        //    return PositionSalaryList;
        //}
    }
}
