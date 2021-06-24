using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelVaccineProcess
    {
        internal static PersonnelVaccine Converter(DataRow dr)
        {
            return new PersonnelVaccine
            {
                ID = dr[PersonnelVaccineFields.ID].ToInt(),
                PersonnelID = dr[PersonnelVaccineFields.PersonnelID].ToNullableLong(),
                ExpirationDate = dr[PersonnelVaccineFields.ExpirationDate].ToNullableDateTime(),
                VaccineTypeID = dr[PersonnelVaccineFields.VaccineTypeID].ToNullableInt(),
                _VaccineType = LookupProcess.GetVaccineType(dr[PersonnelVaccineFields.VaccineTypeID].ToNullableInt())
            };
        }

        public static List<PersonnelVaccine> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelVaccine>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelVaccineParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelVaccineProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }

        public static PersonnelVaccine Get(long Id)
        {
            var eb = new PersonnelVaccine();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelVaccineParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelVaccineProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static PersonnelVaccine Create(PersonnelVaccine personnelVaccine, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelVaccineParameters.PersonnelID, personnelVaccine.PersonnelID },
                { PersonnelVaccineParameters.ExpirationDate, personnelVaccine.ExpirationDate },
                { PersonnelVaccineParameters.VaccineTypeID, personnelVaccine.VaccineTypeID },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelVaccineParameters.ID, SqlDbType.BigInt}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelVaccineProcedures.Create, ref OutParameters, Parameters);
                personnelVaccine.ID = OutParameters.Get(PersonnelVaccineParameters.ID).ToLong();
                personnelVaccine._VaccineType = LookupProcess.GetVaccineType(personnelVaccine.VaccineTypeID);
            }

            return personnelVaccine;
        }

        public static PersonnelVaccine Update(PersonnelVaccine personnelVaccine, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelVaccineParameters.ID, personnelVaccine.ID },
                { PersonnelVaccineParameters.PersonnelID, personnelVaccine.PersonnelID },
                { PersonnelVaccineParameters.ExpirationDate, personnelVaccine.ExpirationDate },
                { PersonnelVaccineParameters.VaccineTypeID, personnelVaccine.VaccineTypeID },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelVaccineProcedures.Update, Parameters);
                personnelVaccine._VaccineType = LookupProcess.GetVaccineType(personnelVaccine.VaccineTypeID);

            }
            return personnelVaccine;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelVaccineFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelVaccineProcedures.Delete, Parameters);
            }
        }
    }
}
