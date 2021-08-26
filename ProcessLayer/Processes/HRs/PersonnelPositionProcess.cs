using ProcessLayer.Entities;
using System.Data;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Collections.Generic;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;

namespace ProcessLayer.Processes
{
    public static class PersonnelPositionProcess
    {
        internal static bool IsPersonnelPositionOnly = false;
        internal static PersonnelPosition Converter(DataRow dr)
        {
            var obj = new PersonnelPosition
            {
                ID = dr[PersonnelPositionFields.ID].ToLong(),
                PersonnelID = dr[PersonnelPositionFields.PersonnelID].ToNullableLong(),
                PositionID = dr[PersonnelPositionFields.PositionID].ToNullableInt(),
                StartDate = dr[PersonnelPositionFields.StartDate].ToNullableDateTime(),
                EndDate = dr[PersonnelPositionFields.EndDate].ToNullableDateTime()
            };

            obj._Position = PositionProcess.Instance.Value.Get(obj.PositionID);
            
            return obj;
        }

        public static PersonnelPosition Get(long id)
        {
            var obj = new PersonnelPosition();
            var parameters = new Dictionary<string, object> {
                { PersonnelPositionParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelPositionProcedures.Get, parameters))
                {
                    obj = ds.Get(Converter);
                }
            }
            return obj;
        }
        public static List<PersonnelPosition> GetList(long personnelid, bool isPersonnelPositionOnly = false)
        {
            var obj = new List<PersonnelPosition>();
            var parameters = new Dictionary<string, object> {
                { PersonnelPositionParameters.PersonnelID, personnelid}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelPositionProcedures.Get, parameters))
                {
                    IsPersonnelPositionOnly = isPersonnelPositionOnly;
                    obj = ds.GetList(Converter);
                    IsPersonnelPositionOnly = false;
                }
            }
            return obj;
        }
        public static PersonnelPosition CreateOrUpdate(PersonnelPosition obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelPositionParameters.PersonnelID, obj.PersonnelID}
                , {PersonnelPositionParameters.PositionID, obj.PositionID}
                , {PersonnelPositionParameters.StartDate,obj.StartDate}
                , {PersonnelPositionParameters.EndDate, obj.EndDate}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelPositionParameters.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelPositionProcedures.CreateOrUpdate, ref outParameters, parameters);
                obj.ID = outParameters.Get(PersonnelPositionParameters.ID).ToLong();
                obj._Position = PositionProcess.Instance.Value.Get(obj.PositionID);
            }
            return obj;
        }
    }
}
