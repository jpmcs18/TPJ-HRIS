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
    public class ApproverPositionProcess 
    {
        private static ApproverPosition Converter(DataRow dr, bool WithLookup = false)
        {
            return new ApproverPosition()
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                Abbreviation = dr["Abbreviation"].ToString(),
                AllowApprove = dr["Allow Approve"].ToBoolean(),
                CreatedBy = dr["Created By"].ToNullableInt(),
                CreatedOn = dr["Created On"].ToNullableDateTime(),
                ModifiedBy = dr["Modified By"].ToNullableInt(),
                ModifiedOn = dr["Modified On"].ToNullableDateTime(),
                _Creator = LookupProcess.GetUser(dr["Created By"].ToNullableInt()),
                _Modifier = LookupProcess.GetUser(dr["Modified By"].ToNullableInt()),
            };
        }

        public static ApproverPosition GetByID(int id)
        {
            ApproverPosition ApproverPosition = new ApproverPosition();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, id },
                { LookupParameters.Table, Table.Position }
            };

            using (DBTools db = new DBTools())
            {
                var dt = db.ExecuteReader(LookupProcedures.GetLookup, Parameters).Tables[0];
                if (dt.Rows.Count > 0)
                    ApproverPosition = Converter(dt.Rows[0]);
                //else
                //    throw new Exception();
            }

            return ApproverPosition;
        }

        public static List<ApproverPosition> GetList()
        {
            List<ApproverPosition> ApproverPositionList = new List<ApproverPosition>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, Table.Position }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    ApproverPositionList = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }

            return ApproverPositionList;
        }

        public static Position CreateOrUpdate(Position position, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { "@description", position.Description },
                { "@abbreviation", position.Abbreviation },
                { "@allowapprove", position.AllowApprove },
                { "@userid", userid }
            };
            var outParameters = new List<OutParameters>
            {
                { "@id", SqlDbType.Int, position.ID }
            };

            using (DBTools db = new DBTools())
            {
                object ret = db.ExecuteReader("lookup.CreateOrUpdateApproverPosition", ref outParameters, Parameters);
                if (ret is null)
                    throw new Exception();
                else
                    position.ID = outParameters.Get("@id").ToByte();
            }

            return position;
        }

        public static List<ApproverPosition> GetPage(string Filter, bool? IsApprover, int Page, int GridCount, out int Count)
        {
            List<ApproverPosition> ApproverPositionList = new List<ApproverPosition>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { FilterParameters.Filter, Filter },
                { FilterParameters.PageNumber, Page },
                { FilterParameters.IsApprover, IsApprover },
                { FilterParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.[SearchApproverPosition]", Parameters))
                {
                    ApproverPositionList = ds.Tables[0].AsEnumerable().Select(r => Converter(r, true)).ToList();
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                }
            }

            return ApproverPositionList;
        }

        public static void Delete(int id, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, Table.Position },
                { LookupParameters.Id, id },
                { LookupParameters.UserId, userid },
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LookupProcedures.DeleteLookup, Parameters);
            }
        }
    }
}
