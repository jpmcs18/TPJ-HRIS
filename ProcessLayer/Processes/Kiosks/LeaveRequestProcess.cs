using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request;
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
    public class LeaveRequestProcess
    {
        private static LeaveRequestProcess _instance;
        public static LeaveRequestProcess Instance { get { if (_instance == null) { _instance = new LeaveRequestProcess(); } return _instance; } }
        
        internal bool IsLeaveRequestOnly = false;
        internal LeaveRequest Converter(DataRow dr)
        {
            var o = new LeaveRequest
            {
                ID = dr[LeaveRequestFields.Instance.ID].ToLong(),
                PersonnelID = dr[LeaveRequestFields.Instance.PersonnelID].ToNullableLong(),
                LeaveTypeID = dr[LeaveRequestFields.Instance.LeaveTypeID].ToNullableByte(),
                _LeaveType = LeaveTypeProcess.GetLeaveType(dr[LeaveRequestFields.Instance.LeaveTypeID].ToNullableByte()),
                StartDateTime = dr[LeaveRequestFields.Instance.StartDateTime].ToNullableDateTime(),
                EndDateTime = dr[LeaveRequestFields.Instance.EndDateTime].ToNullableDateTime(),
                Reasons = dr[LeaveRequestFields.Instance.Reasons].ToString(),
                File = dr[LeaveRequestFields.Instance.File].ToString(),
                Approved = dr[LeaveRequestFields.Instance.Approved].ToNullableBoolean(),
                ApprovedBy = dr[LeaveRequestFields.Instance.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[LeaveRequestFields.Instance.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[LeaveRequestFields.Instance.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[LeaveRequestFields.Instance.CancelledBy].ToNullableInt(),
                CancelledOn = dr[LeaveRequestFields.Instance.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[LeaveRequestFields.Instance.CancellationRemarks].ToString(),
                ApprovedLeaveCredits = dr[LeaveRequestFields.Instance.ApprovedLeaveCredits].ToNullableFloat(),
                CreatedBy = dr[LeaveRequestFields.Instance.CreatedBy].ToNullableInt(),
                CreatedOn = dr[LeaveRequestFields.Instance.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[LeaveRequestFields.Instance.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[LeaveRequestFields.Instance.ModifiedOn].ToNullableDateTime()
            };

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

            return o;
        }

        public List<LeaveRequest> GetList(string personnel, byte? leavetypeid, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Instance.Personnel, personnel },
                { LeaveRequestParameters.Instance.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.Instance.IsExpired, isExpired },
                { LeaveRequestParameters.Instance.IsPending, isPending },
                { LeaveRequestParameters.Instance.IsApproved, isApproved },
                { LeaveRequestParameters.Instance.IsCancelled, isCancelled },
                { LeaveRequestParameters.Instance.StartDateTime, startdatetime },
                { LeaveRequestParameters.Instance.EndDateTime, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.Filter, ref outParameters, parameters))
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
                { LeaveRequestParameters.Instance.Personnel, personnel },
                { LeaveRequestParameters.Instance.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.Instance.IsExpired, isExpired },
                { LeaveRequestParameters.Instance.IsPending, isPending },
                { LeaveRequestParameters.Instance.IsApproved, isApproved },
                { LeaveRequestParameters.Instance.IsCancelled, isCancelled },
                { LeaveRequestParameters.Instance.StartDateTime, startdatetime },
                { LeaveRequestParameters.Instance.EndDateTime, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { LeaveRequestParameters.Instance.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.FilterApproving, ref outParameters, parameters))
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
                { LeaveRequestParameters.Instance.Personnel, personnel },
                { LeaveRequestParameters.Instance.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.Instance.IsExpired, isExpired },
                { LeaveRequestParameters.Instance.IsPending, isPending },
                { LeaveRequestParameters.Instance.IsApproved, isApproved },
                { LeaveRequestParameters.Instance.IsCancelled, isCancelled },
                { LeaveRequestParameters.Instance.StartDateTime, startdatetime },
                { LeaveRequestParameters.Instance.EndDateTime, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.FilterRequestThatNeedDocument, ref outParameters, parameters))
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
                { LeaveRequestParameters.Instance.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.Get, parameters))
                {
                    Leaves = ds.GetList(Converter);
                }
            }

            return Leaves;
        }

        public List<LeaveRequest> GetApprovedLeave(long personnelid, byte? leavetypeid, DateTime startdatetime, DateTime enddatetime)
        {
            var Leaves = new List<LeaveRequest>();
            var parameters = new Dictionary<string, object>{
                { LeaveRequestParameters.Instance.PersonnelID, personnelid },
                { LeaveRequestParameters.Instance.LeaveTypeID, leavetypeid },
                { LeaveRequestParameters.Instance.StartDateTime, startdatetime },
                { LeaveRequestParameters.Instance.EndDateTime, enddatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.GetApprovedLeave, parameters))
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
                { LeaveRequestParameters.Instance.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LeaveRequestProcedures.Instance.Get, parameters))
                {
                    Leave = ds.Get(Converter);
                }
            }

            return Leave;
        }
        public bool ValidateLeaveRequest(LeaveRequest leave)
        {
            StringBuilder sb = new StringBuilder();
            leave._LeaveType = LeaveTypeProcess.GetLeaveType(leave.LeaveTypeID);
            if ((leave.LeaveTypeID ?? 0) == 0)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("- Leave type cannot be null.");
            }
            if ((leave.PersonnelID ?? 0) == 0)
            {

                sb.AppendLine("<br/>");
                sb.AppendLine("- Requestor cannot be null.");
            }
            if (leave._LeaveType?.BulkUse ?? false)
            {
                if (leave.StartDateTime == null)
                {

                    sb.AppendLine("<br/>");
                    sb.AppendLine("- Start date cannot be null.");
                }
            }
            else
            {

                if (leave.StartDateTime == null)
                {
                    sb.AppendLine("<br/>");
                    sb.AppendLine("- Start date and time cannot be null.");
                }

                if (leave.EndDateTime == null)
                {
                    sb.AppendLine("<br/>");
                    sb.AppendLine("- End date and time cannot be null.");
                }

                if (leave.StartDateTime != null && leave.EndDateTime != null)
                {
                    var leaveCreditToUse = (leave.EndDateTime - leave.StartDateTime)?.TotalHours;
                    if (leaveCreditToUse > 9)
                    {
                        sb.AppendLine("<br/>");
                        sb.AppendLine("- Leave must be 1 day per request .");
                    }

                    var leaveCredits = PersonnelLeaveCreditProcess.GetRemainingCredits(leave.PersonnelID ?? 0, leave.LeaveTypeID ?? 0, (short)(leave.StartDateTime?.Year ?? 0));
                    if (leaveCreditToUse > leaveCredits?.LeaveCredits)
                    {
                        sb.AppendLine("<br/>");
                        sb.AppendLine($"- Not enough credits. You only have {leaveCredits.LeaveCredits} credits and you're request is {leaveCreditToUse}");
                    }
                }
            }
            if (sb.Length > 0)
                throw new Exception(sb.ToString());
            return true;
        }

        public LeaveRequest CreateOrUpdate(LeaveRequest Leave, int userid)
        {
            if (ValidateLeaveRequest(Leave))
            {
                if(Leave._LeaveType?.BulkUse ?? false)
                    Leave = GetBulkUseLeaveDate(Leave);

                var parameters = new Dictionary<string, object> {
                    { LeaveRequestParameters.Instance.PersonnelID, Leave.PersonnelID }
                    , { LeaveRequestParameters.Instance.LeaveTypeID, Leave.LeaveTypeID }
                    , { LeaveRequestParameters.Instance.StartDateTime, Leave.StartDateTime }
                    , { LeaveRequestParameters.Instance.EndDateTime, Leave.EndDateTime }
                    , { LeaveRequestParameters.Instance.Reasons, Leave.Reasons }
                    , { LeaveRequestParameters.Instance.BulkUse, Leave._LeaveType.BulkUse ?? false }
                    , { CredentialParameters.LogBy, userid }
                };

                var outparameter = new List<OutParameters> {{ LeaveRequestParameters.Instance.ID, SqlDbType.BigInt, Leave.ID }};

                using (var db = new DBTools())
                {
                    db.ExecuteNonQuery(LeaveRequestProcedures.Instance.CreateOrUpdate, ref outparameter, parameters);
                    Leave = Get(outparameter.Get(LeaveRequestParameters.Instance.ID).ToLong());
                }

                return Leave;
            }
            return null;
        }

        public void Approve(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Instance.ID, id }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Instance.Approve, parameters);
            }
        }

        public void Cancel(LeaveRequest Leave, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Instance.ID, Leave.ID },
                { LeaveRequestParameters.Instance.CancellationRemarks, Leave.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Instance.Cancel, parameters);
            }
        }

        public void Delete(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { LeaveRequestParameters.Instance.ID, id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Instance.Delete, parameters);
            }
        }

        public void UploadDocument(long id, string file, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { LeaveRequestParameters.Instance.ID, id },
                { LeaveRequestParameters.Instance.File, file },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LeaveRequestProcedures.Instance.UploadDocument, parameters);
            }

        }

        public LeaveRequest GetBulkUseLeaveDate(LeaveRequest leaveRequest)
        {
            List<NonWorkingDays> nonWorkingDays = NonWorkingDaysProcess.Instance.GetNonWorkingDays(leaveRequest.StartDateTime.Value.AddMonths(-5), leaveRequest.StartDateTime.Value.AddMonths(5));
            PersonnelLeaveCredit leaveCredits = PersonnelLeaveCreditProcess.GetRemainingCredits(leaveRequest.PersonnelID ?? 0, leaveRequest.LeaveTypeID ?? 0, (short)(leaveRequest.StartDateTime?.Year ?? 0));
            DateTime assumedEndDate = leaveRequest.StartDateTime.Value;
            int totalLeaveToUse = 0;
            if (leaveCredits?.LeaveCredits > 0)
            {
                while(leaveCredits.LeaveCredits != totalLeaveToUse)
                {
                    var sched = GlobalHelper.GetSchedule(leaveRequest._Personnel._Schedules, assumedEndDate);
                    if (sched != null)
                    {
                        var nonWorking = GlobalHelper.GetNonWorking(nonWorkingDays, assumedEndDate);
                        if (nonWorking == null)
                        {
                            if (totalLeaveToUse == 0)
                            {
                                leaveRequest.StartDateTime = assumedEndDate.Date + sched.TimeIn;
                            }
                            leaveRequest.EndDateTime = assumedEndDate.Date + sched.TimeOut;
                            totalLeaveToUse += 1;
                        }
                    }
                    assumedEndDate = assumedEndDate.AddDays(1);
                }

                return leaveRequest;
            }
            else
                throw new Exception("Zero leave credit");
        }
    }
}
