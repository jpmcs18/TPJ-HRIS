using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public sealed class HighRiskRequestProcess
    {
        public static readonly HighRiskRequestProcess Instance = new HighRiskRequestProcess();
        private HighRiskRequestProcess() { }
        internal bool IsHighRiskRequestOnly = false;

        internal HighRiskRequest Converter(DataRow dr) {

            var h = new HighRiskRequest
            {
                ID = dr["ID"].ToLong(),
                RequestDate = dr["Request Date"].ToDateTime(),
                PersonnelID = dr["Personnel ID"].ToNullableLong(),
                Reasons = dr["Reasons"].ToString(),
                Approved = dr["Approved"].ToNullableBoolean(),
                ApprovedBy = dr["Approved By"].ToNullableInt(),
                ApprovedOn = dr["Approved On"].ToNullableDateTime(),
                Cancelled = dr["Cancelled"].ToNullableBoolean(),
                CancelledBy = dr["Cancelled By"].ToNullableInt(),
                CancelledOn = dr["Cancelled On"].ToNullableDateTime(),
                CancellationRemarks = dr["Cancellation Remarks"].ToString(),
                CreatedBy = dr["Created By"].ToNullableInt(),
                CreatedOn = dr["Created On"].ToNullableDateTime(),
                ModifiedBy = dr["Modified By"].ToNullableInt(),
                ModifiedOn = dr["Modified On"].ToNullableDateTime()
            };

            if (!IsHighRiskRequestOnly)
            {
                h._Personnel = PersonnelProcess.Get(h.PersonnelID ?? 0, true);
                h._Approver = LookupProcess.GetUser(h.ApprovedBy);
                h._Cancel = LookupProcess.GetUser(h.CancelledBy);
                h._Creator = LookupProcess.GetUser(h.CreatedBy);
                h._Modifier = LookupProcess.GetUser(h.ModifiedBy);
            }
            return h;
        }

        public List<HighRiskRequest> GetList(long personnelId, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdate, DateTime? enddate, int page, int gridCount, out int PageCount)
        {
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { "@IsExpired", isExpired },
                { "@IsPending", isPending },
                { "@IsApproved", isApproved },
                { "@IsCancelled", isCancelled },
                { "@StartDate", startdate },
                { "@EndDate", enddate },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[kiosk].[FilterHighRiskRequest]", ref outParameters, parameters))
                {
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }

        public List<HighRiskRequest> GetApprovingList(string personnel, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdate, DateTime? enddate, int page, int gridCount, out int PageCount)
        {
            var parameters = new Dictionary<string, object> {
                { "@Personnel", personnel },
                { "@IsExpired", isExpired },
                { "@IsPending", isPending },
                { "@IsApproved", isApproved },
                { "@IsCancelled", isCancelled },
                { "@StartDate", startdate },
                { "@EndDate", enddate },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[kiosk].[FilterApprovingHighRiskRequest]", ref outParameters, parameters))
                {
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return ds.GetList(Converter);
                }
            }
        }
        public List<HighRiskRequest> GetList(long personnelid)
        {
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[kiosk].[GetHighRiskRequest]", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public List<HighRiskRequest> GetApproved(long personnelid, DateTime startdate, DateTime enddate)
        {
            var parameters = new Dictionary<string, object>{
                { "@PersonnelID", personnelid },
                { "@StartDate", startdate },
                { "@EndDate", enddate }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[kiosk].[GetApprovedHighRiskRequest]", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public HighRiskRequest Get(long id)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("[kiosk].[GetHighRiskRequest]", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public HighRiskRequest CreateOrUpdate(HighRiskRequest hrr, int userid)
        {
            var parameters = new Dictionary<string, object> {
                  { "@PersonnelID", hrr.PersonnelID }
                , { "@RequestDate", hrr.RequestDate }
                , { "@Reasons", hrr.Reasons }
                , { CredentialParameters.LogBy, userid }
            };

            var outparameter = new List<OutParameters> { { "@ID", SqlDbType.BigInt, hrr.ID } };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("kiosk.CreateOrUpdateHighRishRequest", ref outparameter, parameters);
                hrr = Get(outparameter.Get("@ID").ToLong());
            }

            return hrr;
        }

        public void Approve(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("kiosk.ApproveHighRiskRequest", parameters);
            }
        }

        public void Cancel(HighRiskRequest hrr, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", hrr.ID },
                { "@CancellationRemarks", hrr.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("kiosk.CancelHighRiskRequest", parameters);
            }
        }

        public void Delete(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("[kiosk].[DeleteHighRiskRequest]", parameters);
            }
        }

    }
}
