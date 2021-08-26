using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public sealed class PersonnelLeaveCreditProcess
    {
        public static readonly Lazy<PersonnelLeaveCreditProcess> Instance = new Lazy<PersonnelLeaveCreditProcess>(() => new PersonnelLeaveCreditProcess());
        private PersonnelLeaveCreditProcess() { }
        internal PersonnelLeaveCredit Converter(DataRow dr)
        {
            var l = new PersonnelLeaveCredit
            {
                ID = dr[PersonnelLeaveCreditsFields.ID].ToLong(),
                PersonnelID = dr[PersonnelLeaveCreditsFields.PersonnelID].ToNullableLong(),
                LeaveCredits = dr[PersonnelLeaveCreditsFields.LeaveCredits].ToNullableFloat(),
                LeaveTypeID = dr[PersonnelLeaveCreditsFields.LeaveTypeID].ToNullableByte(),
                YearValid = dr[PersonnelLeaveCreditsFields.YearValid].ToNullableShort()
            };

            l._LeaveType = LeaveTypeProcess.Instance.Value.Get(l.LeaveTypeID);

            return l;
        }

        public List<PersonnelLeaveCredit> GetByPersonnelID(long? PersonnelID = null)
        {
            var eb = new List<PersonnelLeaveCredit>();

            if (PersonnelID.HasValue)
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelLeaveCreditsParameters.PersonnelID, PersonnelID.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                    {
                        eb = ds.GetList(Converter);
                    }
                }
            }

            return eb;
        }
        public PersonnelLeaveCredit GetRemainingCredits(long personnelID, byte leaveTypeID, short year)
        {

            using (var db = new DBTools())
            {
                return GetRemainingCredits(db, personnelID, leaveTypeID, year);
            }
        }

        public PersonnelLeaveCredit GetRemainingCredits(DBTools db, long personnelID, byte leaveTypeID, short year)
        {
            var eb = new PersonnelLeaveCredit();
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.PersonnelID, personnelID },
                { PersonnelLeaveCreditsParameters.LeaveTypeID, leaveTypeID},
                { PersonnelLeaveCreditsParameters.YearValid, year}
            };

            using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.GetRemainingCredits, Parameters))
            {
                eb = ds.Get(Converter);
            }

            return eb;
        }

        public PersonnelLeaveCredit Get(long Id)
        {
            var eb = new PersonnelLeaveCredit();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                {
                    eb = ds.Get(Converter);
                }
            }

            return eb;
        }

        public List<LeaveType> GetLeaveWithCredits(long personnelID, int year)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeavesWithCredits", new Dictionary<string, object> { { "@PersonnelID", personnelID }, { "@Year", year } }))
                {
                    return ds.GetList(LeaveTypeProcess.Instance.Value.Converter);
                }
            }
        }

        public PersonnelLeaveCredit CreateOrUpdate(PersonnelLeaveCredit obj, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {PersonnelLeaveCreditsParameters.PersonnelID, obj.PersonnelID}
                , {PersonnelLeaveCreditsParameters.LeaveCredits, obj.LeaveCredits}
                , {PersonnelLeaveCreditsParameters.LeaveTypeID,obj.LeaveTypeID}
                , {PersonnelLeaveCreditsParameters.YearValid,obj.YearValid}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelLeaveCreditsParameters.ID, SqlDbType.BigInt, obj.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.CreateOrUpdate, ref outParameters, parameters);
                obj = Get(outParameters.Get(PersonnelLeaveCreditsParameters.ID).ToLong());
            }
            return obj;
        }

        public void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.Delete, Parameters);
            }
        }
        public void UpdateCredits(DBTools db, long personnelId, byte leaveTypeId, int year, double credits, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { "@LeaveTypeId", leaveTypeId },
                { "@UsedCredits", credits },
                { "@YearValid", year },
                { "@LogBy", userid }
            };
            db.ExecuteNonQuery("hr.UpdateLeaveCredits", parameters);

        }
    }
}
