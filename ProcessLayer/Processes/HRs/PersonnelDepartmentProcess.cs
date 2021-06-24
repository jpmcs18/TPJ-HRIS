using ProcessLayer.Entities;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Collections.Generic;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Processes.Lookups;

namespace ProcessLayer.Processes
{
    public static class PersonnelDepartmentProcess
    {
        internal static bool IsPersonnelDepartmentOnly = false;
        internal static PersonnelDepartment Converter(DataRow dr)
        {
            var obj = new PersonnelDepartment
            {
                ID = dr[PersonnelDepartmentFields.ID].ToLong(),
                PersonnelID = dr[PersonnelDepartmentFields.PersonnelID].ToNullableLong(),
                DepartmentID = dr[PersonnelDepartmentFields.DepartmentID].ToNullableInt(),
                StartDate = dr[PersonnelDepartmentFields.StartDate].ToNullableDateTime(),
                EndDate = dr[PersonnelDepartmentFields.EndDate].ToNullableDateTime()
            };

            obj._Department = DepartmentProcess.Instance.Get(obj.DepartmentID ?? 0);

            return obj;
        }
        public static PersonnelDepartment Get(long id)
        {
            var obj = new PersonnelDepartment();
            var parameters = new Dictionary<string, object> {
                { PersonnelDepartmentParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelDepartmentProcedures.Get, parameters))
                {
                    obj = ds.Get(Converter);
                }
            }
            return obj;
        }
        public static List<PersonnelDepartment> GetList(long personnelid, bool isPersonnelDepartmentOnly = false)
        {
            var obj = new List<PersonnelDepartment>();
            var parameters = new Dictionary<string, object> {
                { PersonnelDepartmentParameters.PersonnelID, personnelid}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelDepartmentProcedures.Get, parameters))
                {
                    IsPersonnelDepartmentOnly = isPersonnelDepartmentOnly;
                    obj = ds.GetList(Converter);
                    IsPersonnelDepartmentOnly = false;
                }
            }
            return obj;
        }
        public static PersonnelDepartment CreateOrUpdate(PersonnelDepartment obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelDepartmentParameters.PersonnelID, obj.PersonnelID}
                , {PersonnelDepartmentParameters.DepartmentID, obj.DepartmentID}
                , {PersonnelDepartmentParameters.StartDate,obj.StartDate}
                , {PersonnelDepartmentParameters.EndDate, obj.EndDate}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelDepartmentParameters.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelDepartmentProcedures.CreateOrUpdate, ref outParameters, parameters);
                obj.ID = outParameters.Get(PersonnelDepartmentParameters.ID).ToLong();
                obj._Department = DepartmentProcess.Instance.Get(obj.DepartmentID ?? 0);
            }
            return obj;
        }
    }
}
