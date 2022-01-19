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
    public sealed class TimeEditRequestProcess
    {
        public static readonly TimeEditRequestProcess Instance = new TimeEditRequestProcess();
        private TimeEditRequestProcess() { }
        internal bool IsTimeEditRequestOnly = false;
        internal TimeEditRequest Converter(DataRow dr)
        {
            var o = new TimeEditRequest
            {
                ID = dr[TimeEditRequestFields.ID].ToLong(),
                PersonnelID = dr[TimeEditRequestFields.PersonnelID].ToNullableLong(),
                RequestDate = dr["Request Date"].ToDateTime(),
                LoginDateTime = dr[TimeEditRequestFields.LoginDateTime].ToNullableDateTime(),
                LogoutDateTime = dr[TimeEditRequestFields.LogoutDateTime].ToNullableDateTime(),
                Reasons = dr[TimeEditRequestFields.Reasons].ToString(),
                Approved = dr[TimeEditRequestFields.Approved].ToNullableBoolean(),
                ApprovedBy = dr[TimeEditRequestFields.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[TimeEditRequestFields.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[TimeEditRequestFields.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[TimeEditRequestFields.CancelledBy].ToNullableInt(),
                CancelledOn = dr[TimeEditRequestFields.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[TimeEditRequestFields.CancellationRemarks].ToString(),
                CreatedBy = dr[TimeEditRequestFields.CreatedBy].ToNullableInt(),
                CreatedOn = dr[TimeEditRequestFields.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[TimeEditRequestFields.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[TimeEditRequestFields.ModifiedOn].ToNullableDateTime()
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
                { TimeEditRequestParameters.IsExpired, isExpired },
                { TimeEditRequestParameters.IsPending, isPending },
                { TimeEditRequestParameters.IsApproved, isApproved },
                { TimeEditRequestParameters.IsCancelled, isCancelled },
                { TimeEditRequestParameters.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.LogoutDateTime, logoutdatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Filter, ref outParameters, parameters))
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
                { TimeEditRequestParameters.Personnel, personnel },
                { TimeEditRequestParameters.IsPending, isPending },
                { TimeEditRequestParameters.IsApproved, isApproved },
                { TimeEditRequestParameters.IsCancelled, isCancelled },
                { TimeEditRequestParameters.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.LogoutDateTime, logoutdatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { TimeEditRequestParameters.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.FilterApproving, ref outParameters, parameters))
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
                { TimeEditRequestParameters.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Get, parameters))
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
                { TimeEditRequestParameters.PersonnelID, personnelid },
                { TimeEditRequestParameters.LoginDateTime, logindatetime },
                { TimeEditRequestParameters.LogoutDateTime, logoutdatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.GetApprovedTimeEdit, parameters))
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
                { TimeEditRequestParameters.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(TimeEditRequestProcedures.Get, parameters))
                {
                    TimeEdit = ds.Get(Converter);
                }
            }

            return TimeEdit;
        }

        public TimeEditRequest CreateOrUpdate(TimeEditRequest timeedit, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.PersonnelID, timeedit.PersonnelID }
                , { "@RequestDate", timeedit.RequestDate }
                , { TimeEditRequestParameters.LoginDateTime, timeedit.LoginDateTime }
                , { TimeEditRequestParameters.LogoutDateTime, timeedit.LogoutDateTime }
                , { TimeEditRequestParameters.Reasons, timeedit.Reasons }
                , { CredentialParameters.LogBy, userid }
            };

            var outparameter = new List<OutParameters> {
                { TimeEditRequestParameters.ID, SqlDbType.BigInt, timeedit.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.CreateOrUpdate, ref outparameter, parameters);
                timeedit = Get(outparameter.Get(TimeEditRequestParameters.ID).ToLong());
            }

            return timeedit;
        }

        public void Approve(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.ID, id }
                , { CredentialParameters.LogBy, userid }
            };
            
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Approve, parameters);
            }
        }

        public void Cancel(TimeEditRequest timeedit, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.ID, timeedit.ID },
                { TimeEditRequestParameters.CancellationRemarks, timeedit.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Cancel, parameters);
            }
        }

        public void Delete(TimeEditRequest ot)
        {
            var parameters = new Dictionary<string, object> {
                { TimeEditRequestParameters.ID, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(TimeEditRequestProcedures.Delete, parameters);
            }
        }
    }
}
