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
            PersonnelLeaveCredit l = new PersonnelLeaveCredit
            {
                ID = dr[PersonnelLeaveCreditsFields.ID].ToLong(),
                PersonnelID = dr[PersonnelLeaveCreditsFields.PersonnelID].ToNullableLong(),
                LeaveCredits = dr[PersonnelLeaveCreditsFields.LeaveCredits].ToNullableFloat(),
                LeaveTypeID = dr[PersonnelLeaveCreditsFields.LeaveTypeID].ToNullableInt(),
                YearValid = dr[PersonnelLeaveCreditsFields.YearValid].ToNullableShort(),
                ValidFrom = dr[PersonnelLeaveCreditsFields.ValidFrom].ToNullableDateTime(),
                ValidTo = dr[PersonnelLeaveCreditsFields.ValidTo].ToNullableDateTime()
            };

            l._LeaveType = LeaveTypeProcess.Instance.Value.Get(l.LeaveTypeID);
            return l;
        }

        public List<PersonnelLeaveCredit> GetByPersonnelID(long PersonnelID)
        {
            GenerateDefaultCredits(PersonnelID, default);

            Dictionary<string, object> Parameters = new Dictionary<string, object>
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

        public PersonnelLeaveCredit GetRemainingCredits(long personnelID, int leaveTypeID, DateTime? date)
        {
            using (DBTools db = new DBTools())
            {
                return GetRemainingCredits(db, personnelID, leaveTypeID, date);
            }
        }

        public PersonnelLeaveCredit GetRemainingCredits(DBTools db, long personnelID, int leaveTypeID, DateTime? date)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.PersonnelID, personnelID },
                { PersonnelLeaveCreditsParameters.LeaveTypeID, leaveTypeID},
                { PersonnelLeaveCreditsParameters.Date, date}
            };

            using (DataSet ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.GetRemainingCredits, Parameters))
            {
                return ds.Get(Converter);
            }
        }

        public PersonnelLeaveCredit Get(long Id)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.ID, Id }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<LeaveType> GetLeaveWithCredits(long personnelID, DateTime date)
        {
            GenerateDefaultCredits(personnelID, default);

            using (DBTools db = new DBTools())
            {
                Dictionary<string, object> parameters = new Dictionary<string, object> 
                {
                    { PersonnelEmploymentTypeParameters.PersonnelID, personnelID }, 
                    { PersonnelLeaveCreditsParameters.Date, date }
                };

                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.GetLeavesWithCredits, parameters))
                {
                    return ds.GetList(LeaveTypeProcess.Instance.Value.Converter);
                }
            }
        }

        public PersonnelLeaveCredit CreateOrUpdate(PersonnelLeaveCredit leaveCredit, int userid)
        {
            using (DBTools db = new DBTools())
            {
                return CreateOrUpdate(db, leaveCredit, userid);
            }
        }

        private PersonnelLeaveCredit CreateOrUpdate(DBTools db, PersonnelLeaveCredit leaveCredit, int userid)
        {
            leaveCredit._LeaveType = LeaveTypeProcess.Instance.Value.Get(leaveCredit.LeaveTypeID);

            if (!(leaveCredit._LeaveType.IsMidYear ?? false))
            {
                leaveCredit.ValidFrom = new DateTime(leaveCredit.YearValid ?? 0, 1, 1);
                leaveCredit.ValidTo = new DateTime(leaveCredit.YearValid ?? 0, 12, 31);
            }

            Dictionary<string, object> parameters = new Dictionary<string, object> 
            {
                {PersonnelLeaveCreditsParameters.PersonnelID, leaveCredit.PersonnelID}
                , {PersonnelLeaveCreditsParameters.LeaveCredits, leaveCredit.LeaveCredits}
                , {PersonnelLeaveCreditsParameters.LeaveTypeID,leaveCredit.LeaveTypeID}
                , {PersonnelLeaveCreditsParameters.YearValid,leaveCredit.YearValid}
                , {PersonnelLeaveCreditsParameters.ValidFrom,leaveCredit.ValidFrom}
                , {PersonnelLeaveCreditsParameters.ValidTo,leaveCredit.ValidTo}
                , {CredentialParameters.LogBy, userid}
            };

            List<OutParameters> outParameters = new List<OutParameters>
            {
                { PersonnelLeaveCreditsParameters.ID, SqlDbType.BigInt, leaveCredit.ID }
            };
            
            db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.CreateOrUpdate, ref outParameters, parameters);
            leaveCredit.ID = outParameters.Get(PersonnelLeaveCreditsParameters.ID).ToLong();
            return leaveCredit;
        }

        public void Delete(long Id, int userid)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.Delete, Parameters);
            }
        }
        public void UpdateCredits(DBTools db, long id, float credits, int userid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { PersonnelLeaveCreditsParameters.ID, id },
                { PersonnelLeaveCreditsParameters.UsedCredits, credits },
                { CredentialParameters.LogBy, userid }
            };
            db.ExecuteNonQuery(PersonnelLeaveCreditsProcedures.UpdateLeaveCredits, parameters);
        }
        public PersonnelLeaveCredit Get(long personnelID, int leaveTypeId, DateTime date)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelLeaveCreditsParameters.PersonnelID, personnelID },
                { PersonnelLeaveCreditsParameters.LeaveTypeID, leaveTypeId },
                { PersonnelLeaveCreditsParameters.Date, date },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelLeaveCreditsProcedures.Get, Parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public void GenerateDefaultCredits(long personnelID, int userId)
        {
            Personnel personnel = PersonnelProcess.Get(personnelID, true);
            if (personnel.DateHired == null)
            {
                return;
            }

            float yearsInService = personnel.Years + (personnel.Months / (float)12);

            List<LeaveDefaultCredits> defaultCredits = LeaveDefaultCreditsProcess.Instance.Value.GetList(yearsInService);
            List<PersonnelLeaveCredit> leaveCredits = new List<PersonnelLeaveCredit>();

            if (!(defaultCredits?.Any() ?? false))
            {
                return;
            }
            
            foreach (LeaveDefaultCredits defaultCredit in defaultCredits)
            {
                PersonnelLeaveCredit leave = Get(personnel.ID, defaultCredit.LeaveTypeID, DateTime.Now);
                if ((leave?.ID ?? 0) > 0)
                {
                    continue;
                }
                leaveCredits.Add(GeneratePersonnelLeaveCredit(personnel, defaultCredit));

            }
            
            SaveDefaultCredits(leaveCredits, userId);
        }

        private PersonnelLeaveCredit GeneratePersonnelLeaveCredit(Personnel personnel, LeaveDefaultCredits defaultCredit)
        {
            PersonnelLeaveCredit personnelLeaveCredit = new PersonnelLeaveCredit
            {
                LeaveCredits = defaultCredit.Credits,
                LeaveTypeID = defaultCredit.LeaveTypeID,
                PersonnelID = personnel.ID
            };

            if (defaultCredit.LeaveType.IsMidYear ?? false)
            {
                personnelLeaveCredit.ValidFrom = new DateTime(DateTime.Now.Year, defaultCredit.LeaveType.DateStart?.Month ?? 0, defaultCredit.LeaveType.DateStart?.Day ?? 0);
                personnelLeaveCredit.ValidTo = personnelLeaveCredit.ValidFrom?.AddYears(1).AddDays(-1);
                return personnelLeaveCredit;
            }

            personnelLeaveCredit.YearValid = (short)DateTime.Now.Year;
            return personnelLeaveCredit;
        }

        private void SaveDefaultCredits(List<PersonnelLeaveCredit> leaveCredits, int userId)
        {
            if (!(leaveCredits?.Any() ?? false))
            {
                return;
            }

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
                catch (Exception)
                {
                    db.RollBackTransaction();
                    throw;
                }
            }

        }
    }
}
