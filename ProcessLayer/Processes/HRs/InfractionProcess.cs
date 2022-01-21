using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public sealed class InfractionProcess
    {
        public static readonly InfractionProcess Instance = new InfractionProcess();
        private InfractionProcess() { }

        private bool InfractionOnly = false;
        internal Infraction Converter(DataRow dr)
        {
            Infraction infraction = new Infraction
            {
                ID = dr["ID"].ToLong(),
                Description = dr["Description"].ToString(),
                MemoNo = dr["Memo No"].ToString(),
                MemoDate = dr["Memo Date"].ToDateTime(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                HearingSchedule = dr["Hearing Schedule"].ToNullableDateTime(),
                HearingStatusID = dr["Hearing Status ID"].ToNullableShort(),
                SanctionID = dr["Sanction ID"].ToNullableShort(),
                SanctionDate = dr["Sanction Date"].ToNullableDateTime(),
                SanctionDays = dr["Sanction Days"].ToNullableInt(),
                StatusID = dr["Status ID"].ToShort(),
                Subject = dr["Subject"].ToString()
            };

            infraction.Status = InfractionStatusProcess.Instance.Get(infraction.StatusID);
            infraction.HearingStatus = HearingStatusProcess.Instance.Get(infraction.HearingStatusID ?? 0);
            infraction.Sanction = SanctionProcess.Instance.Get(infraction.SanctionID ?? 0);
            infraction.Personnel = PersonnelProcess.Get(infraction.PersonnelID, true);

            if (!InfractionOnly)
            {
                infraction.Content = InfractionContentProcess.Instance.GetList(infraction.ID);
            }

            try { infraction.IsNew = dr["Is New"].ToNullableBoolean() ?? false; } catch { }

            return infraction;
        }

        public Infraction Get(long id, bool infractionOnly = false)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetInfraction", Parameters))
                {
                    InfractionOnly = infractionOnly;
                    Infraction inf = ds.Get(Converter);
                    InfractionOnly = false;
                    return inf;
                }
            }

        }


        public List<Infraction> GetList(long personnelId, bool infractionOnly = false)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@PersonnelID", personnelId }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetInfraction", Parameters))
                {
                    InfractionOnly = infractionOnly;
                    List<Infraction> inf = ds.GetList(Converter);
                    InfractionOnly = false;
                    return inf;
                }
            }

        }

        public List<Infraction> Filter(string personnel, DateTime? date, short? statusId, int page, int gridCount, out int PageCount)
        {
            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@Personnel", personnel },
                    { "@MemoDate", date },
                    { "@StatusID", statusId },
                    { "@PageNumber", page },
                    { "@GridCount", gridCount }
                };
                    
                var outParameters = new List<OutParameters>
                {
                    { "@PageCount", SqlDbType.Int }
                };

                using (var ds = db.ExecuteReader("hr.FilterInfraction", ref outParameters, Parameters))
                {
                    InfractionOnly = true;
                    var infraction = ds.GetList(Converter);
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    InfractionOnly = false;
                    return infraction;
                }

            }
        }
        public void SetHearingStatus(long id, short? hearingStatusID, int userId, DateTime? schedule = null)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@HearingStatusID", hearingStatusID },
                    { "@Date", schedule },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.SetHearingStatus", parameters);
            }
        }

        public void Close(long id, short sanctionId, DateTime? date, int? days, int userId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@SanctionID", sanctionId },
                    { "@Days", days },
                    { "@Date", date },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CloseInfraction", parameters);
            }
        }

        public void ScheduleHearing(long id, DateTime schedule, int userId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@Date", schedule },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.ScheduleHearing", parameters);
            }
        }
        public Infraction CreateOrUpdate(Infraction infraction, int userId) {

            using (var db = new DBTools())
            {
                bool newInfraction = infraction.ID == 0;
                List<OutParameters> outParameters = new List<OutParameters>
                {
                    { "@ID", SqlDbType.Int, infraction.ID }
                };

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@MemoNo", infraction.MemoNo },
                    { "@MemoDate", infraction.MemoDate },
                    { "@Description", infraction.Description },
                    { "@PersonnelID", infraction.PersonnelID },
                    { "@Subject", infraction.Subject },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdateInfraction", ref outParameters, parameters);
                infraction.ID = outParameters.Get("@ID").ToLong();

                if(newInfraction)
                {
                    infraction.Content?.ForEach((content) => { 
                        content.InfractionID = infraction.ID; 
                        InfractionContentProcess.Instance.Create(db, content, userId); 
                    });
                    infraction.Content = InfractionContentProcess.Instance.GetList(infraction.ID);
                }

                return infraction;
            }
        }

        public void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { "@ID", Id },
                { "@Delete", true },
                { "@LogBy", userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("hr.CreateOrUpdateInfraction", Parameters);
            }
        }

    }
}
