using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public class PersonnelLeaveCreditProcess
    {
        internal static PersonnelLeaveCredit Converter(DataRow dr)
        {
            var l = new PersonnelLeaveCredit
            {
                ID = dr[PersonnelLeaveCreditsFields.Instance.ID].ToLong(),
                PersonnelID = dr[PersonnelLeaveCreditsFields.Instance.PersonnelID].ToNullableLong(),
                LeaveCredits = dr[PersonnelLeaveCreditsFields.Instance.LeaveCredits].ToNullableFloat(),
                LeaveTypeID = dr[PersonnelLeaveCreditsFields.Instance.LeaveTypeID].ToNullableByte(),
                YearValid = dr[PersonnelLeaveCreditsFields.Instance.YearValid].ToNullableShort()
            };

            l._LeaveType = LeaveTypeProcess.GetLeaveType(l.LeaveTypeID);

            return l;
        }

        public static List<PersonnelLeaveCredit> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelLeaveCredit>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelLeaveCreditsParameters.Instance.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Instance.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }
        public static PersonnelLeaveCredit GetRemainingCredits(long personnelID, byte leaveTypeID, short year)
        {

            using (var db = new DBTools())
            {
                return GetRemainingCredits(db, personnelID, leaveTypeID, year);
            }
        }

        public static PersonnelLeaveCredit GetRemainingCredits(DBTools db, long personnelID, byte leaveTypeID, short year)
        {
            var eb = new PersonnelLeaveCredit();
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.Instance.PersonnelID, personnelID },
                { PersonnelLeaveCreditsParameters.Instance.LeaveTypeID, leaveTypeID},
                { PersonnelLeaveCreditsParameters.Instance.YearValid, year}
            };

            using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Instance.GetRemainingCredits, Parameters))
            {
                eb = ds.Get(Converter);
            }

            return eb;
        }

        public static PersonnelLeaveCredit Get(long Id)
        {
            var eb = new PersonnelLeaveCredit();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.Instance.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Instance.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public static List<LeaveType> GetLeaveWithCredits(long personnelID, int year)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeavesWithCredits", new Dictionary<string, object> { { "@PersonnelID", personnelID }, { "@Year", year } }))
                {
                    return ds.GetList(LeaveTypeProcess.Converter);
                }
            }
        }

        public static PersonnelLeaveCredit CreateOrUpdate(PersonnelLeaveCredit obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelLeaveCreditsParameters.Instance.PersonnelID, obj.PersonnelID}
                , {PersonnelLeaveCreditsParameters.Instance.LeaveCredits, obj.LeaveCredits}
                , {PersonnelLeaveCreditsParameters.Instance.LeaveTypeID,obj.LeaveTypeID}
                , {PersonnelLeaveCreditsParameters.Instance.YearValid,obj.YearValid}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelLeaveCreditsParameters.Instance.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.Instance.CreateOrUpdate, ref outParameters, parameters);
                obj = Get(outParameters.Get(PersonnelLeaveCreditsParameters.Instance.ID).ToLong());
            }
            return obj;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsFields.Instance.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.Instance.Delete, Parameters);
            }
        }
        public static void UpdateCredits(DBTools db, byte leaveTypeId, int year, double credits, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@LeaveTypeId", leaveTypeId },
                { "@UsedCredits", credits },
                { "@YearValid", year },
                { "@LogBy", userid }
            };
            db.ExecuteNonQuery("hr.UpdateLeaveCredits", parameters);

        }
    }
}
