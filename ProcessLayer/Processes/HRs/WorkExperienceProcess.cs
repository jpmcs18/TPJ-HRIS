using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class WorkExperienceProcess
    {
        internal static WorkExperience Converter(DataRow dr)
        {
            return new WorkExperience
            {
                ID = dr[WorkExperienceFields.ID].ToInt(),
                PersonnelID = dr[WorkExperienceFields.PersonnelID].ToNullableLong(),
                Company = dr[WorkExperienceFields.Company].ToString(),
                Position = dr[WorkExperienceFields.Position].ToString(),
                EmploymentType = dr[WorkExperienceFields.EmploymentType].ToString(),
                FromMonth = dr[WorkExperienceFields.FromMonth].ToNullableInt(),
                FromYear = dr[WorkExperienceFields.FromYear].ToNullableInt(),
                ToMonth = dr[WorkExperienceFields.ToMonth].ToNullableInt(),
                ToYear = dr[WorkExperienceFields.ToYear].ToNullableInt()
            };
        }

        public static List<WorkExperience> GetByPersonnelID(long? PersonnelID = null)
        {
            var we = new List<WorkExperience>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { WorkExperienceParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(WorkExperienceProcedures.GetWorkExperience, Parameters))
                    {
                        we = ds.GetList(Converter);
                    }
                }
            }

            return we;
        }

        public static WorkExperience Get(long Id)
        {
            var we = new WorkExperience();

            var Parameters = new Dictionary<string, object>
                {
                    { WorkExperienceParameters.ID, Id }
                };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(WorkExperienceProcedures.GetWorkExperience, Parameters))
                {
                    we = ds.Get(Converter);
                }
            }

            return we;
        }

        public static WorkExperience Create(WorkExperience workExperience, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { WorkExperienceParameters.PersonnelID, workExperience.PersonnelID },
                { WorkExperienceParameters.Company, workExperience.Company },
                { WorkExperienceParameters.Position, workExperience.Position },
                { WorkExperienceParameters.EmploymentType, workExperience.EmploymentType },
                { WorkExperienceParameters.FromYear, workExperience.FromYear },
                { WorkExperienceParameters.FromMonth, workExperience.FromMonth },
                { WorkExperienceParameters.ToYear, workExperience.ToYear },
                { WorkExperienceParameters.ToMonth, workExperience.ToMonth },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { WorkExperienceParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(WorkExperienceProcedures.CreateWorkExperience, ref OutParameters, Parameters);
                workExperience.ID = OutParameters.Get(WorkExperienceParameters.ID).ToLong();
            }

            return workExperience;

        }

        public static WorkExperience Update(WorkExperience workExperience, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { WorkExperienceParameters.ID, workExperience.ID },
                { WorkExperienceParameters.PersonnelID, workExperience.PersonnelID },
                { WorkExperienceParameters.Company, workExperience.Company },
                { WorkExperienceParameters.Position, workExperience.Position },
                { WorkExperienceParameters.EmploymentType, workExperience.EmploymentType },
                { WorkExperienceParameters.FromYear, workExperience.FromYear },
                { WorkExperienceParameters.FromMonth, workExperience.FromMonth },
                { WorkExperienceParameters.ToYear, workExperience.ToYear },
                { WorkExperienceParameters.ToMonth, workExperience.ToMonth },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(WorkExperienceProcedures.UpdateWorkExperience, Parameters);
            }
            return workExperience;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(WorkExperienceProcedures.DeleteWorkExperience, Parameters);
            }
        }

    }
}
