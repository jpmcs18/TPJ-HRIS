using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelDependentProcess
    {
        internal static PersonnelDependent Converter(DataRow dr)
        {
            return new PersonnelDependent
            {
                ID = dr[PersonnelDependentFields.ID].ToInt(),
                PersonnelID = dr[PersonnelDependentFields.PersonnelID].ToNullableLong(),
                FirstName = dr[PersonnelDependentFields.FirstName].ToString(),
                LastName = dr[PersonnelDependentFields.LastName].ToString(),
                MiddleName = dr[PersonnelDependentFields.MiddleName].ToString(),
                BirthDate = dr[PersonnelDependentFields.BirthDate].ToNullableDateTime(),
                RelationshipID = dr[PersonnelDependentFields.RelationshipID].ToNullableInt(),
                _Relationship = LookupProcess.GetRelationship(dr[PersonnelDependentFields.RelationshipID].ToNullableInt())
            };
        }

        public static List<PersonnelDependent> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelDependent>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelDependentParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelDependentProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelDependent Get(long Id)
        {
            var eb = new PersonnelDependent();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDependentParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelDependentProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelDependent Create(PersonnelDependent personnelDependent, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDependentParameters.PersonnelID, personnelDependent.PersonnelID },
                { PersonnelDependentParameters.FirstName, personnelDependent.FirstName },
                { PersonnelDependentParameters.LastName, personnelDependent.LastName },
                { PersonnelDependentParameters.MiddleName, personnelDependent.MiddleName },
                { PersonnelDependentParameters.BirthDate, personnelDependent.BirthDate },
                { PersonnelDependentParameters.RelationshipID, personnelDependent.RelationshipID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelDependentParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDependentProcedures.Create, ref OutParameters, Parameters);
                personnelDependent.ID = OutParameters.Get(PersonnelDependentParameters.ID).ToLong();
                personnelDependent._Relationship = LookupProcess.GetRelationship(personnelDependent.RelationshipID);
            }

            return personnelDependent;
        }

        public static PersonnelDependent Update(PersonnelDependent personnelDependent, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDependentParameters.ID, personnelDependent.ID },
                { PersonnelDependentParameters.PersonnelID, personnelDependent.PersonnelID },
                { PersonnelDependentParameters.FirstName, personnelDependent.FirstName },
                { PersonnelDependentParameters.LastName, personnelDependent.LastName },
                { PersonnelDependentParameters.MiddleName, personnelDependent.MiddleName },
                { PersonnelDependentParameters.BirthDate, personnelDependent.BirthDate },
                { PersonnelDependentParameters.RelationshipID, personnelDependent.RelationshipID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDependentProcedures.Update, Parameters);
                personnelDependent._Relationship = LookupProcess.GetRelationship(personnelDependent.RelationshipID);
            }
            return personnelDependent;

        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelDependentFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDependentProcedures.Delete, Parameters);
            }
        }
    }
}
