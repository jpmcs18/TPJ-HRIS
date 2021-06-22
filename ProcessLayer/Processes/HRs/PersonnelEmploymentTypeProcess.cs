using ProcessLayer.Entities;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Collections.Generic;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;

namespace ProcessLayer.Processes
{
    public static class PersonnelEmploymentTypeProcess
    {
        internal static bool IsPersonnelEmploymentTypeOnly = false;
        internal static PersonnelEmploymentType Converter(DataRow dr)
        {
            var obj = new PersonnelEmploymentType
            {
                ID = dr[PersonnelEmploymentTypeFields.ID].ToLong(),
                PersonnelID = dr[PersonnelEmploymentTypeFields.PersonnelID].ToNullableLong(),
                EmploymentTypeID = dr[PersonnelEmploymentTypeFields.EmploymentTypeID].ToNullableInt(),
                StartDate = dr[PersonnelEmploymentTypeFields.StartDate].ToNullableDateTime(),
                EndDate = dr[PersonnelEmploymentTypeFields.EndDate].ToNullableDateTime()
            };

            obj._EmploymentType = LookupProcess.GetEmploymentType(obj.EmploymentTypeID);

            return obj;
        }

        public static PersonnelEmploymentType Get(long id)
        {
            var obj = new PersonnelEmploymentType();
            var parameters = new Dictionary<string, object> {
                { PersonnelEmploymentTypeParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelEmploymentTypeProcedures.Get, parameters))
                {
                    obj = ds.Get(Converter);
                }
            }
            return obj;
        }
        public static List<PersonnelEmploymentType> GetList(long personnelid,bool isPersonnelEmploymentTypeOnly = false)
        {
            var obj = new List<PersonnelEmploymentType>();
            var parameters = new Dictionary<string, object> {
                { PersonnelEmploymentTypeParameters.PersonnelID, personnelid}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelEmploymentTypeProcedures.Get, parameters))
                {
                    IsPersonnelEmploymentTypeOnly = isPersonnelEmploymentTypeOnly;
                    obj = ds.GetList(Converter);
                    IsPersonnelEmploymentTypeOnly = false;
                }
            }
            return obj;
        }
        public static PersonnelEmploymentType CreateOrUpdate(PersonnelEmploymentType obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelEmploymentTypeParameters.PersonnelID, obj.PersonnelID}
                , {PersonnelEmploymentTypeParameters.EmploymentTypeID, obj.EmploymentTypeID}
                , {PersonnelEmploymentTypeParameters.StartDate,obj.StartDate}
                , {PersonnelEmploymentTypeParameters.EndDate, obj.EndDate}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelEmploymentTypeParameters.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelEmploymentTypeProcedures.CreateOrUpdate, ref outParameters, parameters);
                obj.ID = outParameters.Get(PersonnelEmploymentTypeParameters.ID).ToLong();
                obj._EmploymentType = LookupProcess.GetEmploymentType(obj.EmploymentTypeID);
            }
            return obj;
        }
    }
}
