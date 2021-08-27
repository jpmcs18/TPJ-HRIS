using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.HRs;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace ProcessLayer.Processes.Kiosk
{
    public sealed class LeaveRequestProcess
    {
        public static readonly Lazy<LeaveRequestProcess> Instance = new Lazy<LeaveRequestProcess>(() => new LeaveRequestProcess());
        private LeaveRequestProcess() { }
        
        internal bool IsLeaveRequestOnly = false;
        internal bool WithComputedLeaveCredits = false;
        internal LeaveRequest Converter(DataRow dr)
        {
            var o = new LeaveRequest
            {
                ID = dr[LeaveRequestFields.ID].ToLong(),
                PersonnelID = dr[LeaveRequestFields.PersonnelID].ToNullableLong(),
                LeaveTypeID = dr[LeaveRequestFields.LeaveTypeID].ToNullableByte(),
                _LeaveType = LeaveTypeProcess.Instance.Value.Get(dr[LeaveRequestFields.LeaveTypeID].ToNullableByte()),
                RequestedDate = dr[LeaveRequestFields.RequestedDate].ToDateTime(),
                NoofDays = dr[LeaveRequestFields.NoofDays].ToFloat(),
                Reasons = dr[LeaveRequestFields.Reasons].ToString(),
                File = dr[LeaveRequestFields.File].ToString(),
                Approved = dr[LeaveRequestFields.Approved].ToNullableBoolean(),
                ApprovedBy = dr[LeaveRequestFields.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[LeaveRequestFields.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[LeaveRequestFields.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[LeaveRequestFields.CancelledBy].ToNullableInt(),
                CancelledOn = dr[LeaveRequestFields.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[LeaveRequestFields.CancellationRemarks].ToString(),
                ApprovedLeaveCredits = dr[LeaveRequestFields.ApprovedLeaveCredits].ToNullableFloat(),
                CreatedBy = dr[LeaveRequestFields.CreatedBy].ToNullableInt(),
                CreatedOn = dr[LeaveRequestFields.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[LeaveRequestFields.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[LeaveRequestFields.ModifiedOn].ToNullableDateTime()
            };

            o._LeaveType = LeaveTypeProcess.Instance.Value.Get(o.LeaveTypeID);
            
            if (!IsLeaveRequestOnly)
            {
                o._Personnel = PersonnelProcess.Get(o.PersonnelID??0, true);
                o._Approver = LookupProcess.GetUser(o.ApprovedBy);
                o._Cancel = LookupProcess.GetUser(o.CancelledBy);
                o._Creator = LookupProcess.GetUser(o.CreatedBy);
                o._Modifier = LookupProcess.GetUser(o.ModifiedBy);
            }

            if (!string.IsNullOrEmpty(o.File))
            {
                o.FilePath = Path.Combine(ConfigurationManager.AppSettings["LeaveSaveLocation"], o.File);
            }

            if(WithComputedLeaveCredits)
            {
                //o.ComputedLeaveCredits = ComputedLeaveCreditsProcess.Instance.Value.GetList(o.ID);
            }
            return o;
        }

        public List<LeaveRequest> GetList(long personnelId, byte? leavetypeid, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { LeaveRequestParameters.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.IsExpired, isExpired },
                { LeaveRequestParameters.IsPending, isPending },
                { LeaveRequestParameters.IsApproved, isApproved },
                { LeaveRequestParameters.IsCancelled, isCancelled },
                { LeaveRequestParameters.StartDate, startdatetime },
                { LeaveRequestParameters.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Filter, ref outParameters, parameters))
                {
                    Leaves = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return Leaves;
        }

        public List<LeaveRequest> GetApprovingList(string personnel, byte? leavetypeid, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount, int approver)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Personnel, personnel },
                { LeaveRequestParameters.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.IsExpired, isExpired },
                { LeaveRequestParameters.IsPending, isPending },
                { LeaveRequestParameters.IsApproved, isApproved },
                { LeaveRequestParameters.IsCancelled, isCancelled },
                { LeaveRequestParameters.StartDate, startdatetime },
                { LeaveRequestParameters.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { LeaveRequestParameters.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.FilterApproving, ref outParameters, parameters))
                {
                    Leaves = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return Leaves;
        }

        public List<LeaveRequest> GetRequestThatNeedDocument(string personnel, byte? leavetypeid, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Personnel, personnel },
                { LeaveRequestParameters.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.IsExpired, isExpired },
                { LeaveRequestParameters.IsPending, isPending },
                { LeaveRequestParameters.IsApproved, isApproved },
                { LeaveRequestParameters.IsCancelled, isCancelled },
                { LeaveRequestParameters.StartDate, startdatetime },
                { LeaveRequestParameters.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.FilterRequestThatNeedDocument, ref outParameters, parameters))
                {
                    Leaves = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return Leaves;
        }
        public List<LeaveRequest> GetList(long personnelid)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Get, parameters))
                {
                    Leaves = ds.GetList(Converter);
                }
            }

            return Leaves;
        }

        public List<LeaveRequest> GetApprovedLeave(long personnelid, byte? leavetypeid, DateTime startdatetime, DateTime enddatetime, bool withComputedLeaveCredit = false)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object>{
                { LeaveRequestParameters.PersonnelID, personnelid },
                { LeaveRequestParameters.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.StartDate, startdatetime },
                { LeaveRequestParameters.EndDate, enddatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.GetApprovedLeave, parameters))
                {
                    Leaves = ds.GetList(Converter);
                }
            }
            return Leaves;
        }
        public LeaveRequest Get(long id)
        {

            var Leave = new LeaveRequest();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Get, parameters))
                {
                    Leave = ds.Get(Converter);
                }
            }

            return Leave;
        }

        public LeaveRequest Get(DBTools db, long id)
        {

            var Leave = new LeaveRequest();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.ID, id }
            };

            using (var ds = db.ExecuteReader(LeaveRequestProcedures.Get, parameters))
            {
                Leave = ds.Get(Converter);
            }

            return Leave;
        }
        public LeaveRequest CreateOrUpdate(LeaveRequest Leave, int userid)
        {
            if (ValidateLeaveRequest(Leave))
            {
                var parameters = new Dictionary<string, object> {
                    { LeaveRequestParameters.PersonnelID, Leave.PersonnelID }
                    , { LeaveRequestParameters.LeaveTypeID, Leave.LeaveTypeID }
                    , { LeaveRequestParameters.RequestedDate, Leave.RequestedDate }
                    , { LeaveRequestParameters.NoofDays, Leave.NoofDays}
                    , { LeaveRequestParameters.Reasons, Leave.Reasons }
                    , { CredentialParameters.LogBy, userid }
                };

                List<OutParameters> outparameter = new List<OutParameters> {{ LeaveRequestParameters.ID, SqlDbType.BigInt, Leave.ID }};

                using (var db = new DBTools())
                {
                    db.ExecuteNonQuery(LeaveRequestProcedures.CreateOrUpdate, ref outparameter, parameters);
                    Leave = Get(outparameter.Get(LeaveRequestParameters.ID).ToLong());
                }

                return Leave;
            }
            return null;
        }

        private void ApprovedRequest(DBTools db, long id, int userid)
        {

            var parameters = new Dictionary<string, object> {
                    { "@ID", id }
                    , { CredentialParameters.LogBy, userid }
                };

            db.ExecuteNonQuery("kiosk.ApprovedLeaveRequest", parameters);
        }

        private void UpdateApproveCredits(DBTools db, long id, double leaveUsed, int userid)
        {

            var parameters = new Dictionary<string, object> {
                    { "@ID", id }
                    , { "@LeaveUsed", leaveUsed }
                    , { CredentialParameters.LogBy, userid }
                };

            db.ExecuteNonQuery("kiosk.UpdateApprovedLeaveCredits", parameters);
        }

        public void Approve(long id, int userid)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    Approve(id, userid, db);

                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollBackTransaction();
                    throw;
                }
            }
        }

        private void Approve(long id, int userid, DBTools db)
        {
            LeaveRequest leave = Get(db, id);
            PersonnelLeaveCredit leaveCredits = PersonnelLeaveCreditProcess.Instance.Value.GetRemainingCredits(db, leave.PersonnelID ?? 0, leave.LeaveTypeID ?? 0, (short)(leave.RequestedDate.Year));

            float leaveCreditsToUse = leave.NoofDays;

            if (leaveCreditsToUse > (leaveCredits?.LeaveCredits ?? 0))
            {
                throw new Exception($"- Not enough credits. You only have {leaveCredits.LeaveCredits} credits and your request is {leaveCreditsToUse}");
            }

            if (!(leave.Approved ?? false))
            {
                ApprovedRequest(db, id, userid);
                leave.Approved = true;
            }
            if ((leave.Approved ?? false) && (((leave._LeaveType.HasDocumentNeeded ?? false) && !string.IsNullOrEmpty(leave.FilePath)) || !(leave._LeaveType.HasDocumentNeeded ?? false)))
            {
                UpdateApproveCredits(db, id, leaveCreditsToUse, userid);
                PersonnelLeaveCreditProcess.Instance.Value.UpdateCredits(db, leave.PersonnelID ?? 0, leave.LeaveTypeID ?? 0, leave.RequestedDate.Year, leaveCreditsToUse, userid);
            }

        }

        public void Cancel(LeaveRequest Leave, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.ID, Leave.ID },
                { LeaveRequestParameters.CancellationRemarks, Leave.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Cancel, parameters);
            }
        }

        public void Delete(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.ID, id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Delete, parameters);
            }
        }

        public void UploadDocument(long id, string file, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { LeaveRequestParameters.ID, id },
                { LeaveRequestParameters.File, file },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    db.ExecuteNonQuery(LeaveRequestProcedures.UploadDocument, parameters);
                    Approve(id, userid, db);

                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollBackTransaction();
                    throw;
                }
            }

        }

        public bool ValidateLeaveRequest(LeaveRequest leave)
        {
            StringBuilder sb = new StringBuilder();
            leave._LeaveType = LeaveTypeProcess.Instance.Value.Get(leave.LeaveTypeID);
            if ((leave?.LeaveTypeID ?? 0) == 0)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("- Leave type cannot be null.");
            }

            if ((leave?.PersonnelID ?? 0) == 0)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("- Requestor cannot be null.");
            }

            if (leave?.RequestedDate == null)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("- Requested date cannot be null.");
            }
            if ((leave?.NoofDays ?? 0) == 0)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("- No of days cannot be null.");
            }

            PersonnelLeaveCredit leaveCredits = PersonnelLeaveCreditProcess.Instance.Value.GetRemainingCredits(leave.PersonnelID ?? 0, leave.LeaveTypeID ?? 0, (short)leave.RequestedDate.Year);
            
            float leaveCreditsToUse = leave.NoofDays;

            if (leaveCreditsToUse > leaveCredits?.LeaveCredits)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine($"- Not enough credits. You only have {leaveCredits.LeaveCredits} credits and your request is {leaveCreditsToUse}");
            }

            return sb.Length > 0 ? throw new Exception(sb.ToString()) : true;
        }
    }
}
