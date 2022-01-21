using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public sealed class AbsenceRequestProcess
    {
        public static readonly AbsenceRequestProcess Instance = new AbsenceRequestProcess();
        private AbsenceRequestProcess() { }

        private bool IsRequestOnly { get; set; }
        internal AbsenceRequest Converter(DataRow dr)
        {
            AbsenceRequest ar = new AbsenceRequest
            {
                ID = dr["ID"].ToLong(),
                PersonnelID = dr["Personnel ID"].ToNullableLong(),
                RequestDate = dr["Request Date"].ToNullableDateTime(),
                NoofDays = dr["No of Days"].ToNullableFloat(),
                Reasons = dr["Reasons"].ToString(),
                Approved = dr["Approved"].ToNullableBoolean(),
                ApprovedBy = dr["Approved By"].ToNullableInt(),
                ApprovedOn = dr["Approved On"].ToNullableDateTime(),
                Cancelled = dr["Cancelled"].ToNullableBoolean(),
                CancelledBy = dr["Cancelled By"].ToNullableInt(),
                CancelledOn = dr["Cancelled On"].ToNullableDateTime(),
                CancellationRemarks = dr["Cancellation Remarks"].ToString(),
                IsAbsent = dr["Is Absent"].ToNullableBoolean(),
                IsHalfDay = dr["Is Halfday"].ToNullableBoolean(),
                IsMorning = dr["Is Morning"].ToNullableBoolean(),
                IsAfternoon = dr["Is Afternoon"].ToNullableBoolean(),
                IsUndertime = dr["Is Undertime"].ToNullableBoolean(),
                Time = dr["Time"].ToNullableDateTime(),
                CreatedOn = dr["Created On"].ToNullableDateTime(),
                ModifiedOn = dr["Modified On"].ToNullableDateTime(),
                NotedBy = dr["Noted By"].ToNullableInt(),
                NotedOn = dr["Noted On"].ToNullableDateTime(),
                Noted = dr["Noted"].ToNullableBoolean()
            };

            if (!IsRequestOnly)
            {
                ar._Personnel = PersonnelProcess.Get(ar.PersonnelID ?? 0, true);
                ar._Approver = LookupProcess.GetUser(ar.ApprovedBy);
                ar._Cancel = LookupProcess.GetUser(ar.CancelledBy);
                ar._Noted = LookupProcess.GetUser(ar.NotedBy);
                ar._Creator = LookupProcess.GetUser(ar.CreatedBy);
                ar._Modifier = LookupProcess.GetUser(ar.ModifiedBy);
            }

            return ar;
        }
        public List<AbsenceRequest> GetList(long personnelId, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { "@IsExpired", isExpired },
                { "@IsPending", isPending },
                { "@IsApproved", isApproved },
                { "@IsCancelled", isCancelled },
                { "@StartDate", startdatetime },
                { "@EndDate", enddatetime },
                { "@PageNumber", page },
                { "@GridCount", gridCount }
            };

            List<OutParameters> outParameters = new List<OutParameters>
            {
                { "@PageCount", SqlDbType.Int }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader("[kiosk].[FilterAbsenceRequest]", ref outParameters, parameters))
                {
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }
        public List<AbsenceRequest> GetApprovingList(string personnel, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount, int approver)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@Personnel", personnel },
                { "@IsExpired", isExpired },
                { "@IsPending", isPending },
                { "@IsApproved", isApproved },
                { "@IsCancelled", isCancelled },
                { "@StartDate", startdatetime },
                { "@EndDate", enddatetime },
                { "@PageNumber", page },
                { "@GridCount", gridCount },
                { "@Approver", approver }
            };

            List<OutParameters> outParameters = new List<OutParameters>
            {
                { "@PageCount", SqlDbType.Int }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader("kiosk.FilterApprovingAbsenceRequest", ref outParameters, parameters))
                {
                    PageCount = outParameters.Get("@PageCount").ToInt();
                    return ds.GetList(Converter);
                }
            }
        }
        public AbsenceRequest Get(long id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ID", id }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader("[kiosk].[GetAbsenceRequest]", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }
        public AbsenceRequest CreateOrUpdate(AbsenceRequest absence)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@PersonnelID", absence.PersonnelID }
                    , { "@RequestDate", absence.RequestDate }
                    , { "@NoofDays", absence.NoofDays}
                    , { "@Reasons", absence.Reasons }
                    , { "@IsAbsent", absence.IsAbsent }
                    , { "@IsHalfday", absence.IsHalfDay }
                    , { "@IsMorning", absence.IsMorning }
                    , { "@IsAfternoon", absence.IsAfternoon }
                    , { "@IsUndertime", absence.IsUndertime }
                    , { "@Time", absence.Time }
                };

            List<OutParameters> outparameter = new List<OutParameters> {
                    { "@ID", SqlDbType.BigInt, absence.ID }
                };

            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery("[kiosk].[CreateOrUpdateAbsenceRequest]", ref outparameter, parameters);
                absence = Get(outparameter.Get("@ID").ToLong());
            }

            return absence;
        }
        public void Approved(long id, int userid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@ID", id }
                    , { "@LogBy", userid }
                };

            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery("[kiosk].[ApprovedAbsenceRequest]", parameters);
            }
        }
        public void Cancel(AbsenceRequest absence, int userid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ID", absence.ID },
                { "@CancellationRemarks", absence.CancellationRemarks }
                , { "@LogBy", userid }
            };

            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery("[kiosk].[CancelAbsenceRequest]", parameters);
            }
        }
        public void Delete(long id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {
                { "@ID", id }
            };

            using (DBTools db = new DBTools())
            {
                db.ExecuteNonQuery("[kiosk].[DeleteAbsenceRequest]", parameters);
            }
        }
    }
}
