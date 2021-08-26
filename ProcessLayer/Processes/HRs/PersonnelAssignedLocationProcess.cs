using ProcessLayer.Entities;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Collections.Generic;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using System;

namespace ProcessLayer.Processes
{
    public static class PersonnelAssignedLocationProcess
    {
        internal static bool IsPersonnelAssignedLocationOnly = false;
        internal static PersonnelAssignedLocation Converter(DataRow dr)
        {
            var obj = new PersonnelAssignedLocation
            {
                ID = dr[PersonnelAssignedLocationFields.ID].ToLong(),
                PersonnelID = dr[PersonnelAssignedLocationFields.PersonnelID].ToNullableLong(),
                LocationID = dr[PersonnelAssignedLocationFields.LocationID].ToNullableInt(),
                StartDate = dr[PersonnelAssignedLocationFields.StartDate].ToNullableDateTime(),
                EndDate = dr[PersonnelAssignedLocationFields.EndDate].ToNullableDateTime()
            };

            obj._Location = LocationProcess.Instance.Value.Get(obj.LocationID);

            return obj;
        }

        public static PersonnelAssignedLocation Get(long id)
        {
            var obj = new PersonnelAssignedLocation();
            var parameters = new Dictionary<string, object> {
                { PersonnelAssignedLocationParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelAssignedLocationProcedures.Get, parameters))
                {
                    obj = ds.Get(Converter);
                }
            }
            return obj;
        }
        public static PersonnelAssignedLocation GetCurrent(long personnelID, DateTime date)
        {
            var obj = new PersonnelAssignedLocation();
            var parameters = new Dictionary<string, object> {
                { PersonnelAssignedLocationParameters.PersonnelID, personnelID },
                { PersonnelAssignedLocationParameters.Date, date }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelAssignedLocationProcedures.GetCurrent, parameters))
                {
                    obj = ds.Get(Converter);
                }
            }
            return obj;
        }
        public static List<PersonnelAssignedLocation> GetList(long personnelid, bool isPersonnelAssignedLocationOnly = false)
        {
            var obj = new List<PersonnelAssignedLocation>();
            var parameters = new Dictionary<string, object> {
                { PersonnelAssignedLocationParameters.PersonnelID, personnelid}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelAssignedLocationProcedures.Get, parameters))
                {
                    IsPersonnelAssignedLocationOnly = isPersonnelAssignedLocationOnly;
                    obj = ds.GetList(Converter);
                    IsPersonnelAssignedLocationOnly = false;
                }
            }
            return obj;
        }
        public static PersonnelAssignedLocation CreateOrUpdate(PersonnelAssignedLocation obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelAssignedLocationParameters.PersonnelID, obj.PersonnelID}
                , {PersonnelAssignedLocationParameters.LocationID, obj.LocationID}
                , {PersonnelAssignedLocationParameters.StartDate,obj.StartDate}
                , {PersonnelAssignedLocationParameters.EndDate, obj.EndDate}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelAssignedLocationParameters.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelAssignedLocationProcedures.CreateOrUpdate, ref outParameters, parameters);
                obj.ID = outParameters.Get(PersonnelAssignedLocationParameters.ID).ToLong();
                obj._Location = LocationProcess.Instance.Value.Get(obj.LocationID);
            }
            return obj;
        }
    }
}
