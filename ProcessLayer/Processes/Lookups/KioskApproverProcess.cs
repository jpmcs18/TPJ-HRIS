using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public sealed class KioskApproverProcess 
    {
        public static readonly Lazy<KioskApproverProcess> Instance = new Lazy<KioskApproverProcess>(() => new KioskApproverProcess());
        private KioskApproverProcess() { }
        internal bool IsKioskApproverOnly = false;
        internal KioskApprovers Converter(DataRow dr)
        {
            var k = new KioskApprovers
            {
                ID = dr["ID"].ToInt(),
                ApproverID = dr["Approver ID"].ToNullableLong(),
                DepartmentID = dr["Department ID"].ToNullableInt(),
                Sequence = dr["Sequence"].ToNullableByte()
            };

            if (!IsKioskApproverOnly)
            {
                k._Personnel = PersonnelProcess.Get((int)(k.ApproverID ?? 0), true);
                if (k._Personnel != null)
                {
                    k._Personnel._Departments = PersonnelDepartmentProcess.GetList(k._Personnel.ID);
                    k._Personnel._Positions = PersonnelPositionProcess.GetList(k._Personnel.ID);
                }
            }

            return k;
        }

        public List<KioskApprovers> Get(string name, int? departmentid)
        {
            var k = new List<KioskApprovers>();
            var parameters = new Dictionary<string, object> {
                { "@Name", name },
                { "@DepartmentID", departmentid },
                //{ FilterParameters.PageNumber, page },
                //{ FilterParameters.GridCount, gridCount }
            };

            //var outParameters = new List<OutParameters>
            //{
            //    { FilterParameters.PageCount, SqlDbType.Int }
            //};

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.FilterKioskApprovers", parameters))
                {
                    k = ds.GetList(Converter);
                    //PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }
            return k;
        }

        public KioskApprovers Get(int id)
        {
            var k = new KioskApprovers();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetKioskApprover", new Dictionary<string, object> { { "@ID", id } }))
                {
                    k = ds.Get(Converter);
                }
            }
            return k;
        }

        public List<KioskApprovers> Get()
        {
            var k = new List<KioskApprovers>();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("lookup.GetKioskApprover"))
                {
                    k = ds.GetList(Converter);
                }
            }
            return k;
        }

        public KioskApprovers CreateOrUpdate(KioskApprovers k, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@DepartmentID", k.DepartmentID },
                { "@DepartmentID", k.ApproverID },
                { "@DepartmentID", k.Sequence },
                { "@LogBy", userid }
            };
            var outpar = new List<OutParameters> {
                { "@ID", SqlDbType.Int, k.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.CreateOrUpdateKioskApprover", ref outpar, parameters);
                k.ID = outpar.Get("@ID").ToInt();
            }
            return k;
        }

        public List<KioskApprovers> CreateOrUpdate(List<KioskApprovers> KioskApprovers, int userid)
        {
            foreach (var k in KioskApprovers)
            {
                var parameters = new Dictionary<string, object> {
                    { "@DepartmentID", k.DepartmentID },
                    { "@ApproverID", k.ApproverID },
                    { "@Deleted", k.Deleted },
                    { "@Sequence", k.Sequence },
                    { "@LogBy", userid }
                };
                var outpar = new List<OutParameters> {
                    { "@ID", SqlDbType.Int, k.ID }
                };
                using (var db = new DBTools())
                {
                    db.ExecuteNonQuery("lookup.CreateOrUpdateKioskApprover", ref outpar, parameters);
                    k.ID = outpar.Get("@ID").ToInt();
                }
                k._Personnel = PersonnelProcess.Get(k.ApproverID ?? 0, true);
                k._Personnel._Departments = PersonnelDepartmentProcess.GetList(k._Personnel.ID);
                k._Personnel._Positions = PersonnelPositionProcess.GetList(k._Personnel.ID);
            }
            return KioskApprovers;
        }

        public void Delete(int id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id },
                { "@LogBy", userid }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("lookup.DeleteKioskApprover", parameters);
            }
        }
    }
}
