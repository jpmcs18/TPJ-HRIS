using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Kiosk.Time_Edit_Request;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public class TimeEditRequestProcess
    {
        private static TimeEditRequestProcess _instance;
        public static TimeEditRequestProcess Instance { get { if (_instance == null) { _instance = new TimeEditRequestProcess(); } return _instance;  } }
        internal bool IsTimeEditRequestOnly = false;
        internal TimeEditRequest Converter(DataRow dr)
        {
            var o = new TimeEditRequest
            {
                ID = dr[TimeEditRequestFields.Instance.ID].ToLong(),
                PersonnelID = dr[TimeEditRequestFields.Instance.PersonnelID].ToNullableLong(),
                RequestDate = dr["Request Date"].ToDateTime(),
                LoginDateTime = dr[TimeEditRequestFields.Instance.LoginDateTime].ToNullableDateTime(),
                LogoutDateTime = dr[TimeEditRequestFields.Instance.LogoutDateTime].ToNullableDateTime(),
                Reasons = dr[TimeEditRequestFields.Instance.Reasons].ToString(),
                Approved = dr[TimeEditRequestFields.Instance.Approved].ToNullableBoolean(),
                ApprovedBy = dr[TimeEditRequestFields.Instance.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[TimeEditRequestFields.Instance.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[TimeEditRequestFields.Instance.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[TimeEditRequestFields.Instance.CancelledBy].ToNullableInt(),
                CancelledOn = dr[TimeEditRequestFields.Instance.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[TimeEditRequestFields.Instance.CancellationRemarks].ToString(),
                CreatedBy = dr[TimeEditRequestFields.Instance.CreatedBy].ToNullableInt(),
                CreatedOn = dr[TimeEditRequestFields.Instance.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[TimeEditRequestFields.Instance.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[TimeEditRequestFields.Instance.ModifiedOn].ToNullableDateTime()
            };

            if(!IsTimeEditRequestOnly)
            {
                o._Personnel = PersonnelProcess.Get(o.PersonnelID??0, true);
                o._Approver = LookupProcess.GetUser(o.ApprovedBy);
                o._Cancel = LookupProcess.GetUser(o.CancelledBy);
                o._Creator = LookupProcess.GetUser(o.CreatedBy);
                o._Modifier = LookupProcess.GetUser(o.ModifiedBy);
            }
            return o;
        }

        public List<TimeEditRequest> GetList(long personnelId, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? logindatetime, DateTime? logoutdatetime, int page, int gridCount, out int PageCount)
        {
            var TimeEdits = new List<TimeEditRequest>();
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { TimeEditRequestParameters.Instance.IsExpired, isExpired },
                { TimeEditRequestParameters.Instance.IsPending, isPending },
                { TimeEditRequestParameters.Instance.IsApproved, isApproved },
                { TimeEditRequestParameters.Instance.IsCancelled, isCancelled },
                { TimeEditRequestParameters.Instance.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.Instance.LogoutDateTime, logoutdatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Instance.Filter, ref outParameters, parameters))
                {
                    TimeEdits = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return TimeEdits;
        }

        public List<TimeEditRequest> GetApprovingList(string personnel, bool isPending, bool isApproved, bool isCancelled, DateTime? logindatetime, DateTime? logoutdatetime, int page, int gridCount, out int PageCount, int approver)
        {
            var TimeEdits = new List<TimeEditRequest>();
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.Personnel, personnel },
                { TimeEditRequestParameters.Instance.IsPending, isPending },
                { TimeEditRequestParameters.Instance.IsApproved, isApproved },
                { TimeEditRequestParameters.Instance.IsCancelled, isCancelled },
                { TimeEditRequestParameters.Instance.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.Instance.LogoutDateTime, logoutdatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { TimeEditRequestParameters.Instance.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Instance.FilterApproving, ref outParameters, parameters))
                {
                    TimeEdits = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return TimeEdits;
        }
        public List<TimeEditRequest> GetList(long personnelid)
        {
            var TimeEdits = new List<TimeEditRequest>();
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Instance.Get, parameters))
                {
                    TimeEdits = ds.GetList(Converter);
                }
            }

            return TimeEdits;
        }

        public List<TimeEditRequest> GetApprovedTimeEdit(long personnelid, DateTime logindatetime, DateTime logoutdatetime)
        {
            var TimeEdits = new List<TimeEditRequest>();
            var parameters = new Dictionary<string, object>{
                { TimeEditRequestParameters.Instance.PersonnelID, personnelid },
                { TimeEditRequestParameters.Instance.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.Instance.LogoutDateTime, logoutdatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Instance.GetApprovedTimeEdit, parameters))
                {
                    TimeEdits = ds.GetList(Converter);
                }
            }
            return TimeEdits;
        }
        public TimeEditRequest Get(long id)
        {

            var TimeEdit = new TimeEditRequest();
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Instance.Get, parameters))
                {
                    TimeEdit = ds.Get(Converter);
                }
            }

            return TimeEdit;
        }

        public TimeEditRequest CreateOrUpdate(TimeEditRequest timeedit, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.PersonnelID, timeedit.PersonnelID }
                , { "@RequestDate", timeedit.RequestDate }
                , { TimeEditRequestParameters.Instance.LoginDateTime, timeedit.LoginDateTime }
                , { TimeEditRequestParameters.Instance.LogoutDateTime, timeedit.LogoutDateTime }
                , { TimeEditRequestParameters.Instance.Reasons, timeedit.Reasons }
                , { CredentialParameters.LogBy, userid }
            };

            var outparameter = new List<OutParameters> {
                { TimeEditRequestParameters.Instance.ID, SqlDbType.BigInt, timeedit.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Instance.CreateOrUpdate, ref outparameter, parameters);
                timeedit = Get(outparameter.Get(TimeEditRequestParameters.Instance.ID).ToLong());
            }

            return timeedit;
        }

        public void Approve(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.ID, id }
                , { CredentialParameters.LogBy, userid }
            };
            
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Instance.Approve, parameters);
            }
        }

        public void Cancel(TimeEditRequest timeedit, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.ID, timeedit.ID },
                { TimeEditRequestParameters.Instance.CancellationRemarks, timeedit.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Instance.Cancel, parameters);
            }
        }

        public void Delete(TimeEditRequest ot)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.Instance.ID, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Instance.Delete, parameters);
            }
        }
    }
}
