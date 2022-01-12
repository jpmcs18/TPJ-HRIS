using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace ProcessLayer.Processes.HR
{
    public sealed class PolicyAndProcedureProcess
    {
        public static readonly Lazy<PolicyAndProcedureProcess> Instance = new Lazy<PolicyAndProcedureProcess>(() => new PolicyAndProcedureProcess());
        private PolicyAndProcedureProcess() { }

        private bool PolicyAndProcedureOnly = false;
        internal PolicyAndProcedure Converter(DataRow dr)
        {
            PolicyAndProcedure pap = new PolicyAndProcedure
            {
                ID = dr["ID"].ToLong(),
                Description = dr["Description"].ToString(),
                Subject = dr["Subject"].ToString(),
                File = dr["File"].ToString(),
                SaveOnly = dr["Save Only"].ToNullableBoolean(),
                MemoNo = dr["Memo No"].ToString(),
                MemoDate = dr["Memo Date"].ToDateTime(),
            };

            try
            {
                pap.PersonnelPolicyAndProcedureId = dr["Personnel Policy And Procedure ID"].ToNullableLong();
                pap.Acknowledge = dr["Acknowledge"].ToNullableBoolean();
            }
            catch { }
            try {
                pap.IsNew = dr["Is New"].ToNullableBoolean() ?? false;
            }
            catch { }

            if (!string.IsNullOrEmpty(pap.File))
                pap.FilePath = Path.Combine(ConfigurationManager.AppSettings["MemoFolder"], pap.File);

            if (!PolicyAndProcedureOnly)
            {
                pap.Content = PersonnelPolicyAndProcedureProcess.Instance.Value.GetListOfPersonnel(pap.ID);
            }

            return pap;
        }

        public PolicyAndProcedure Get(long id, bool policyAndProcedureOnly = false)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetPolicyAndProcedure", Parameters))
                {
                    PolicyAndProcedureOnly = policyAndProcedureOnly;
                    PolicyAndProcedure inf = ds.Get(Converter);
                    PolicyAndProcedureOnly = false;
                    return inf;
                }
            }

        }

        public List<PolicyAndProcedure> Filter(int year, int page, int gridCount, out int PageCount)
        {
            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@Year", year },
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                };

                var outParameters = new List<OutParameters>
                {
                    { "@PageCount", SqlDbType.Int }
                };

                using (var ds = db.ExecuteReader("hr.FilterPolicyAndProcedure", ref outParameters, Parameters))
                {
                    PolicyAndProcedureOnly = true;
                    var infraction = ds.GetList(Converter);
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    PolicyAndProcedureOnly = false;
                    return infraction;
                }

            }
        }


        public List<PolicyAndProcedure> GetListByPersonnel(long personnelId)
        {
            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@PersonnelID", personnelId },
                };

                using (var ds = db.ExecuteReader("hr.GetPolicyAndProcedureByPersonnel", Parameters))
                {
                    PolicyAndProcedureOnly = true;
                    var infraction = ds.GetList(Converter);
                    PolicyAndProcedureOnly = false;
                    return infraction;
                }

            }
        }

        public List<int> FilterYears(int page, int gridCount, out int PageCount)
        {
            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                };

                var outParameters = new List<OutParameters>
                {
                    { "@PageCount", SqlDbType.Int }
                };

                using (var ds = db.ExecuteReader("hr.FilterPolicyAndProcedureYears", ref outParameters, Parameters))
                {
                    var years = ds.GetList((dr) => dr["Year"].ToInt());
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    return years;
                }

            }
        }
        public PolicyAndProcedure Create(PolicyAndProcedure policyAndProcedure, long? personnelId, long? groupId, List<int> vesselIds, int userId)
        {

            using (var db = new DBTools())
            {
                bool newPolicyAndProcedure = policyAndProcedure.ID == 0;
                List<OutParameters> outParameters = new List<OutParameters>
                {
                    { "@ID", SqlDbType.Int, policyAndProcedure.ID }
                };

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@MemoNo", policyAndProcedure.MemoNo },
                    { "@MemoDate", policyAndProcedure.MemoDate },
                    { "@Description", policyAndProcedure.Description },
                    { "@Subject", policyAndProcedure.Subject },
                    { "@File", policyAndProcedure.File },
                    { "@saveOnly", policyAndProcedure.SaveOnly },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdatePolicyAndProcedure", ref outParameters, parameters);
                policyAndProcedure.ID = outParameters.Get("@ID").ToLong();

                if (newPolicyAndProcedure)
                {
                    List<long> personnelIds = new List<long>();
                    if(personnelId != null)
                    {
                        personnelIds.Add(personnelId ?? 0);
                    }
                    if(groupId != null)
                    {
                        personnelIds = PersonnelGroupMemberProcess.GetByGroup(groupId ?? 0, true)?.Select(x => x.PersonnelID ?? 0)?.ToList();
                    }
                    vesselIds?.ForEach((id) => {
                        var content = new PersonnelPolicyAndProcedure()
                        {
                            PolicyAndProcedureID = policyAndProcedure.ID,
                            VesselID = id
                        };
                        PersonnelPolicyAndProcedureProcess.Instance.Value.Create(db, content, userId);
                    });

                    personnelIds?.ForEach((id) => {
                        var content = new PersonnelPolicyAndProcedure() {

                            PolicyAndProcedureID = policyAndProcedure.ID,
                            PersonnelID = id
                        };
                        PersonnelPolicyAndProcedureProcess.Instance.Value.Create(db, content, userId);
                    });
                }

                return policyAndProcedure;
            }
        }


        public void Update(PolicyAndProcedure policyAndProcedure, int userId)
        {

            using (var db = new DBTools())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", policyAndProcedure.ID },
                    { "@MemoNo", policyAndProcedure.MemoNo },
                    { "@MemoDate", policyAndProcedure.MemoDate },
                    { "@Description", policyAndProcedure.Description },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdatePolicyAndProcedure", parameters);
            }
        }

        public void UpdateFile(long id, string file, int userId)
        {

            using (var db = new DBTools())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@File", file },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdatePolicyAndProcedure", parameters);
            }
        }
        public void Delete(long id, int userId)
        {

            using (var db = new DBTools())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@Delete", true },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdatePolicyAndProcedure", parameters);
            }
        }

    }
}
