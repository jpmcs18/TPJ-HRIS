using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public sealed class WrittenExplanationProcess
    {
        public static readonly Lazy<WrittenExplanationProcess> Instance = new Lazy<WrittenExplanationProcess>(() => new WrittenExplanationProcess());
        private WrittenExplanationProcess() { }

        private bool WrittenExplanationOnly = false;
        internal WrittenExplanation Converter(DataRow dr)
        {
            WrittenExplanation WrittenExplanation = new WrittenExplanation
            {
                ID = dr["ID"].ToLong(),
                Description = dr["Description"].ToString(),
                MemoNo = dr["Memo No"].ToString(),
                MemoDate = dr["Memo Date"].ToDateTime(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                ConsultationSchedule = dr["Consultation Schedule"].ToNullableDateTime(),
                ConsultationStatusID = dr["Consultation Status ID"].ToNullableShort(),
                RecommendationID = dr["Recommendation ID"].ToNullableShort(),
                StatusID = dr["Status ID"].ToShort(),
                Subject = dr["Subject"].ToString()
            };

            WrittenExplanation.Status = WrittenExplanationStatusProcess.Instance.Value.Get(WrittenExplanation.StatusID);
            WrittenExplanation.ConsultationStatus = ConsultationStatusProcess.Instance.Value.Get(WrittenExplanation.ConsultationStatusID ?? 0);
            WrittenExplanation.Recommendation = RecommendationProcess.Instance.Value.Get(WrittenExplanation.RecommendationID ?? 0);
            WrittenExplanation.Personnel = PersonnelProcess.Get(WrittenExplanation.PersonnelID, true);

            if (!WrittenExplanationOnly)
            {
                WrittenExplanation.Content = WrittenExplanationContentProcess.Instance.Value.GetList(WrittenExplanation.ID);
            }

            try
            {
                WrittenExplanation.IsNew = dr["Is New"].ToNullableBoolean() ?? false;
            }
            catch { }

            return WrittenExplanation;
        }

        public WrittenExplanation Get(long id, bool writtenExplanationOnly = false)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetWrittenExplanation", Parameters))
                {
                    WrittenExplanationOnly = writtenExplanationOnly;
                    WrittenExplanation inf = ds.Get(Converter);
                    WrittenExplanationOnly = false;
                    return inf;
                }
            }

        }


        public List<WrittenExplanation> GetList(long personnelId, bool writtenExplanationOnly = false)
        {
            using (var db = new DBTools())
            {
                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@PersonnelID", personnelId }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetWrittenExplanation", Parameters))
                {
                    WrittenExplanationOnly = writtenExplanationOnly;
                    List<WrittenExplanation> inf = ds.GetList(Converter);
                    WrittenExplanationOnly = false;
                    return inf;
                }
            }

        }

        public List<WrittenExplanation> Filter(string personnel, DateTime? date, short? statusId, int page, int gridCount, out int PageCount)
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

                using (var ds = db.ExecuteReader("hr.FilterWrittenExplanation", ref outParameters, Parameters))
                {
                    WrittenExplanationOnly = true;
                    var WrittenExplanation = ds.GetList(Converter);
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    WrittenExplanationOnly = false;
                    return WrittenExplanation;
                }

            }
        }
        public void SetConsultationStatus(long id, short? ConsultationStatusID, int userId, DateTime? schedule = null)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@ConsultationStatusID", ConsultationStatusID },
                    { "@Date", schedule },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.SetConsultationStatus", parameters);
            }
        }

        public void Close(long id, short RecommendationId, int userId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@RecommendationID", RecommendationId },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CloseWrittenExplanation", parameters);
            }
        }

        public void ScheduleConsultation(long id, DateTime schedule, int userId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@Date", schedule },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.ScheduleConsultation", parameters);
            }
        }
        public WrittenExplanation CreateOrUpdate(WrittenExplanation writtenExplanation, int userId)
        {

            using (var db = new DBTools())
            {
                bool newWrittenExplanation = writtenExplanation.ID == 0;
                List<OutParameters> outParameters = new List<OutParameters>
                {
                    { "@ID", SqlDbType.Int, writtenExplanation.ID }
                };

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@MemoNo", writtenExplanation.MemoNo },
                    { "@MemoDate", writtenExplanation.MemoDate },
                    { "@Description", writtenExplanation.Description },
                    { "@PersonnelID", writtenExplanation.PersonnelID },
                    { "@Subject", writtenExplanation.Subject },
                    { "@LogBy",  userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdateWrittenExplanation", ref outParameters, parameters);
                writtenExplanation.ID = outParameters.Get("@ID").ToLong();

                if (newWrittenExplanation)
                {
                    writtenExplanation.Content?.ForEach((content) => {
                        content.WrittenExplanationID = writtenExplanation.ID;
                        WrittenExplanationContentProcess.Instance.Value.Create(db, content, userId);
                    });
                    writtenExplanation.Content = WrittenExplanationContentProcess.Instance.Value.GetList(writtenExplanation.ID);
                }

                return writtenExplanation;
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
                db.ExecuteNonQuery("hr.CreateOrUpdateWrittenExplanation", Parameters);
            }
        }

    }
}
