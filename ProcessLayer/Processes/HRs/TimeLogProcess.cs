using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.TimeLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes
{
    public static class TimeLogProcess
    {
        internal static bool IsTimeLogOnly = false; 
        internal static TimeLog Converter(DataRow dr)
        {
            var t = new TimeLog {
                ID = dr[TimeLogFields.ID].ToNullableLong() ?? 0,
                PersonnelID = dr[TimeLogFields.PersonnelID].ToNullableLong() ?? 0,
                LoginDate = dr[TimeLogFields.LoginDate].ToNullableDateTime(),
                LogoutDate = dr[TimeLogFields.LogoutDate].ToNullableDateTime()
            };

            if(!IsTimeLogOnly)
            {
                t._Personnel = PersonnelProcess.Get(t.PersonnelID, true);
            }

            return t;
        }

        public static List<TimeLog> Get(long personnelid, DateTime? startdate, DateTime? enddate)
        {
            var t = new List<TimeLog>();
            var parameters = new Dictionary<string, object>() {
                { TimeLogParameters.PersonnelID, personnelid},
                { TimeLogParameters.StartDate, startdate},
                { TimeLogParameters.EndDate, enddate}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeLogProcedures.Get, parameters))
                {
                    t = ds.GetList(Converter);
                }
            }
            return t;
        }

        public static IEnumerable<TimeLog> GetLast()
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeLogProcedures.GetLast))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public static IEnumerable<Personnel> GetPersonnels(string key, DateTime? startdate, DateTime? enddate)
        {
            var p = new List<Personnel>();
            var parameters = new Dictionary<string, object>() {
                { TimeLogParameters.Key, key},
                { TimeLogParameters.StartDate, startdate},
                { TimeLogParameters.EndDate, enddate}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeLogProcedures.GetPersonnels, parameters))
                {
                    PersonnelProcess.PersonnelOnly = true;
                    p = ds.GetList(PersonnelProcess.Converter);
                    PersonnelProcess.PersonnelOnly = false;

                }
            }
            return p;
        }
        public static TimeLog CreateOrUpdate(TimeLog timelog, int userid)
        {
            using (var db = new DBTools())
            {
                return CreateOrUpdate(db, timelog, userid);
            }
        }
        public static TimeLog CreateOrUpdate(DBTools db, TimeLog timelog, int userid)
        {
            var parameters = new Dictionary<string, object>() {
                { TimeLogParameters.PersonnelID, timelog.PersonnelID },
                { TimeLogParameters.LoginDate, timelog.LoginDate },
                {TimeLogParameters.LogoutDate, timelog.LogoutDate },
                { CredentialParameters.LogBy, userid }
            };

            var outParameters = new List<OutParameters>() {
                { TimeLogParameters.ID, SqlDbType.BigInt, timelog.ID }
            };

            db.ExecuteNonQuery(TimeLogProcedures.CreateOrUpdate, ref outParameters, parameters);
            timelog.ID = outParameters.Get(TimeLogParameters.ID).ToLong();

            timelog._Personnel = PersonnelProcess.Get(timelog.PersonnelID, true);
            
            return timelog;
        }
        public static bool ImportTimelog(List<TimeLog> timeLogs, int userid)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try {
                    foreach(TimeLog timelog in timeLogs)
                    {
                        CreateOrUpdate(db, timelog, userid);
                    }
                    db.CommitTransaction();
                    return true;
                }
                catch(Exception) {
                    db.RollBackTransaction();
                    throw;
                }
            }
        }
        public static void Delete(long id, int userid)
        {
            var parameters = new Dictionary<string, object>() {
                { TimeLogParameters.ID, id },
                { CredentialParameters.LogBy, userid }
            };
            
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeLogProcedures.Delete, parameters);
            }
        }

    }
}
