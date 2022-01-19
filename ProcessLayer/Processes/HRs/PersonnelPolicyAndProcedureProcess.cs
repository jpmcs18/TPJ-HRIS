using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public sealed class PersonnelPolicyAndProcedureProcess
    {
        public static readonly PersonnelPolicyAndProcedureProcess Instance = new PersonnelPolicyAndProcedureProcess();
        private PersonnelPolicyAndProcedureProcess() { }

        internal PersonnelPolicyAndProcedure Converter(DataRow dr)
        {
            PersonnelPolicyAndProcedure pap = new PersonnelPolicyAndProcedure
            {
                ID = dr["ID"].ToLong(),
                PersonnelID = dr["Personnel ID"].ToNullableLong(),
                VesselID = dr["Vessel ID"].ToNullableInt(),
                PolicyAndProcedureID = dr["Policy And Procedure ID"].ToLong(),
                Acknowledge = dr["Acknowledge"].ToBoolean()
            };

            pap.Personnel = PersonnelProcess.Get(pap.PersonnelID ?? 0, true);
            pap.Vessel = VesselProcess.Instance.Get(pap.VesselID ?? 0);

            return pap;
        }

        public PersonnelPolicyAndProcedure Get(long id)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetPersonnelPolicyAndProcedure", Parameters))
                {
                    return ds.Get(Converter);
                }
            }

        }

        public List<PersonnelPolicyAndProcedure> GetList(long personnelId)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@PersonnelID", personnelId }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetPersonnelPolicyAndProcedure", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }

        }

        public List<PersonnelPolicyAndProcedure> GetList(string ids)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@IDs", ids }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetPersonnelPolicyAndProcedure", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }

        }
        public List<PersonnelPolicyAndProcedure> GetListOfPersonnel(long policyAndProcedureID)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@PolicyAndProcedureID", policyAndProcedureID }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetPersonnelPolicyAndProcedure", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }

        }

        public List<PersonnelPolicyAndProcedure> Filter(DateTime? date, int page, int gridCount, out int PageCount)
        {
            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@MemoDate", date },
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                };

                var outParameters = new List<OutParameters>
                {
                    { "@PageCount", SqlDbType.Int }
                };

                using (var ds = db.ExecuteReader("hr.FilterPersonnelPolicyAndProcedure", ref outParameters, Parameters))
                {
                    var pap = ds.GetList(Converter);
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    return pap;
                }

            }
        }
        public void Acknowledge(long id, int userId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.AcknowledgePersonnelPolicyAndProcedure", parameters);
            }
        }

        public PersonnelPolicyAndProcedure Create(DBTools db, PersonnelPolicyAndProcedure pap, int userId)
        {
            List<OutParameters> outParameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.Int }
            };

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@PolicyAndProcedureID", pap.PolicyAndProcedureID },
                { "@PersonnelID", pap.PersonnelID },
                { "@VesselID", pap.VesselID },
                { "@LogBy",  userId },
            };

            db.ExecuteNonQuery("hr.CreatePersonnelPolicyAndProcedure", ref outParameters, parameters);
            pap.ID = outParameters.Get("@ID").ToLong();

            return pap;
        }
    }
}
