using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class EducationalBackgroundProcess
    {
        internal static EducationalBackground Converter(DataRow dr)
        {
            return new EducationalBackground
            {
                ID = dr[EducationalBackgroundFields.ID].ToInt(),
                PersonnelID = dr[EducationalBackgroundFields.PersonnelID].ToNullableLong(),
                EducationalLevelID = dr[EducationalBackgroundFields.EducationalLevelID].ToNullableInt(),
                SchoolName = dr[EducationalBackgroundFields.SchoolName].ToString(),
                Course = dr[EducationalBackgroundFields.Course].ToString(),
                FromYear = dr[EducationalBackgroundFields.FromYear].ToNullableInt(),
                ToYear = dr[EducationalBackgroundFields.ToYear].ToNullableInt(),
                _EducationalLevel = LookupProcess.GetEducationalLevel(dr[EducationalBackgroundFields.EducationalLevelID].ToNullableInt())
            };
        }

        public static List<EducationalBackground> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<EducationalBackground>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { EducationalBackgroundParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(EducationalBackgroundProcedures.GetEducationalBackground, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static EducationalBackground Get(long Id)
        {
            var eb = new EducationalBackground();

            var Parameters = new Dictionary<string, object>
            {
                { EducationalBackgroundParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(EducationalBackgroundProcedures.GetEducationalBackground, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }
        
        public static EducationalBackground Create(EducationalBackground educationalBackground, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { EducationalBackgroundParameters.PersonnelID, educationalBackground.PersonnelID },
                { EducationalBackgroundParameters.EducationalLevelID, educationalBackground.EducationalLevelID },
                { EducationalBackgroundParameters.SchoolName, educationalBackground.SchoolName },
                { EducationalBackgroundParameters.Course, educationalBackground.Course },
                { EducationalBackgroundParameters.FromYear, educationalBackground.FromYear },
                { EducationalBackgroundParameters.ToYear, educationalBackground.ToYear },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { EducationalBackgroundParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(EducationalBackgroundProcedures.CreateEducationalBackground, ref OutParameters, Parameters);
                educationalBackground.ID = OutParameters.Get(EducationalBackgroundParameters.ID).ToLong();
                educationalBackground._EducationalLevel = LookupProcess.GetEducationalLevel(educationalBackground.EducationalLevelID);
            }

            return educationalBackground;
        }

        public static EducationalBackground Update(EducationalBackground educationalBackground, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { EducationalBackgroundParameters.ID, educationalBackground.ID },
                { EducationalBackgroundParameters.PersonnelID, educationalBackground.PersonnelID },
                { EducationalBackgroundParameters.EducationalLevelID, educationalBackground.EducationalLevelID },
                { EducationalBackgroundParameters.SchoolName, educationalBackground.SchoolName },
                { EducationalBackgroundParameters.Course, educationalBackground.Course },
                { EducationalBackgroundParameters.FromYear, educationalBackground.FromYear },
                { EducationalBackgroundParameters.ToYear, educationalBackground.ToYear },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(EducationalBackgroundProcedures.UpdateEducationalBackground, Parameters);
                educationalBackground._EducationalLevel = LookupProcess.GetEducationalLevel(educationalBackground.EducationalLevelID);
            }
            return educationalBackground;
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
                db.ExecuteNonQuery(EducationalBackgroundProcedures.DeleteEducationalBackground, Parameters);
            }
        }

    }
}
