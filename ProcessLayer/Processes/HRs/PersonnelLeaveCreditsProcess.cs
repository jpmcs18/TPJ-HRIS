using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        public List<PersonnelLeaveCredit> GetByPersonnelID(long PersonnelID)
        {
            GenerateDefaultCredits(PersonnelID, DateTime.Now, default);

            var Parameters = new Dictionary<string, object>
                {
                    { PersonnelLeaveCreditsParameters.PersonnelID, PersonnelID }
                };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
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
            GenerateDefaultCredits(personnelID, DateTime.Now, default);

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetLeavesWithCredits", new Dictionary<string, object> { { "@PersonnelID", personnelID }, { "@Year", year } }))
                {
                    return ds.GetList(LeaveTypeProcess.Instance.Value.Converter);
                }
            }
        }

        public PersonnelLeaveCredit CreateOrUpdate(PersonnelLeaveCredit leaveCredit, int userid)
        {
            using (var db = new DBTools())
            {
                return CreateOrUpdate(db, leaveCredit, userid);
            }
        }

        private PersonnelLeaveCredit CreateOrUpdate(DBTools db, PersonnelLeaveCredit leaveCredit, int userid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                {PersonnelLeaveCreditsParameters.PersonnelID, leaveCredit.PersonnelID}
                , {PersonnelLeaveCreditsParameters.LeaveCredits, leaveCredit.LeaveCredits}
                , {PersonnelLeaveCreditsParameters.LeaveTypeID,leaveCredit.LeaveTypeID}
                , {PersonnelLeaveCreditsParameters.YearValid,leaveCredit.YearValid}
                , {CredentialParameters.LogBy, userid}
            };
            var outParameters = new List<OutParameters>
            {
                { PersonnelLeaveCreditsParameters.ID, SqlDbType.BigInt, leaveCredit.ID }
            };
            db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.CreateOrUpdate, ref outParameters, parameters);
            leaveCredit.ID = outParameters.Get(PersonnelLeaveCreditsParameters.ID).ToLong();
            return leaveCredit;
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
        public PersonnelLeaveCredit Get(long personnelID, byte leaveTypeId, int year)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.PersonnelID, personnelID },
                { PersonnelLeaveCreditsParameters.LeaveTypeID, leaveTypeId },
                { PersonnelLeaveCreditsParameters.YearValid, year }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public void GenerateDefaultCredits(long personnelID, DateTime date, int userId)
        {
            Personnel personnel = PersonnelProcess.Get(personnelID, true);
            if (personnel.DateHired == null)
            {
                throw new Exception("Date hired is null");
            }

            int year = date.Year - personnel.DateHired.Value.Year;
            year += personnel.DateHired.Value.Month > date.Date.Month ? (personnel.DateHired.Value.Day > date.Date.Day ? -1 : 0) : 0;

            List<LeaveDefaultCredits> defaultCredits = LeaveDefaultCreditsProcess.Instance.Value.GetList(year);
            List<PersonnelLeaveCredit> leaveCredits = new List<PersonnelLeaveCredit>();

            if (defaultCredits?.Any() ?? false)
            {
                foreach (LeaveDefaultCredits defaultCredit in defaultCredits)
                {
                    PersonnelLeaveCredit leave = Get(personnel.ID, defaultCredit.LeaveTypeID, date.Year);
                    if ((leave?.ID ?? 0) == 0)
                    {
                        leaveCredits.Add(new PersonnelLeaveCredit { LeaveCredits = defaultCredit.Credits, LeaveTypeID = defaultCredit.LeaveTypeID, PersonnelID = personnel.ID, YearValid = (short)date.Year });
                    }
                }
            }

            SaveDefaultCredits(leaveCredits, userId);
        }

        private void SaveDefaultCredits(List<PersonnelLeaveCredit> leaveCredits, int userId)
        {
            if(leaveCredits?.Any() ?? false)
            {
                using (DBTools db = new DBTools())
                {
                    db.StartTransaction();

                    try
                    {
                        foreach (var leaveCredit in leaveCredits)
                        {
                            CreateOrUpdate(db, leaveCredit, userId);
                        }

                        db.CommitTransaction();
                    }
                    catch(Exception) {
                        db.RollBackTransaction();
                        throw;
                    }
                }
            }
        }
    }
}
