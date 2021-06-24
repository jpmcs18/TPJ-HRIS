using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Kiosk.OT_Request;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.Kiosk
{
    public class OTRequestProcess
    {
        private static OTRequestProcess _instance;
        public static OTRequestProcess Instance { get { if (_instance == null) { _instance = new OTRequestProcess(); } return _instance; } }
        internal bool IsOTRequestOnly = false;
        internal OTRequest Converter(DataRow dr)
        { 
            var o = new OTRequest
            {
                ID = dr[OTRequestFields.Instance.ID].ToLong(),
                PersonnelID = dr[OTRequestFields.Instance.PersonnelID].ToNullableLong(),
                IsOffice = dr[OTRequestFields.Instance.IsOffice].ToNullableBoolean() ?? false,
                RequestDate = dr[OTRequestFields.Instance.RequestDate].ToNullableDateTime() ?? default,
                Reasons = dr[OTRequestFields.Instance.Reasons].ToString(),
                Approved = dr[OTRequestFields.Instance.Approved].ToNullableBoolean(),
                ApprovedBy = dr[OTRequestFields.Instance.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[OTRequestFields.Instance.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[OTRequestFields.Instance.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[OTRequestFields.Instance.CancelledBy].ToNullableInt(),
                CancelledOn = dr[OTRequestFields.Instance.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[OTRequestFields.Instance.CancellationRemarks].ToString(),
                CreatedBy = dr[OTRequestFields.Instance.CreatedBy].ToNullableInt(),
                CreatedOn = dr[OTRequestFields.Instance.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[OTRequestFields.Instance.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[OTRequestFields.Instance.ModifiedOn].ToNullableDateTime()
            };

            if (!IsOTRequestOnly)
            {
                o._Personnel = PersonnelProcess.Get(o.PersonnelID??0, true);
                o._Approver = LookupProcess.GetUser(o.ApprovedBy);
                o._Cancel = LookupProcess.GetUser(o.CancelledBy);
                o._Creator = LookupProcess.GetUser(o.CreatedBy);
                o._Modifier = LookupProcess.GetUser(o.ModifiedBy);
            }
            return o;
        }

        public List<OTRequest> GetList(string personnel, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            var ots = new List<OTRequest>();
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.Personnel, personnel },
                { OTRequestParameters.Instance.IsExpired, isExpired },
                { OTRequestParameters.Instance.IsPending, isPending },
                { OTRequestParameters.Instance.IsApproved, isApproved },
                { OTRequestParameters.Instance.IsCancelled, isCancelled },
                { OTRequestParameters.Instance.StartDate, startdatetime },
                { OTRequestParameters.Instance.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Instance.Filter, ref outParameters, parameters))
                {
                    ots = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return ots;
        }

        public List<OTRequest> GetApprovingList(string personnel, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount, int? approver = null)
        {
            var ots = new List<OTRequest>();
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.Personnel, personnel },
                { OTRequestParameters.Instance.IsPending, isPending },
                { OTRequestParameters.Instance.IsApproved, isApproved },
                { OTRequestParameters.Instance.IsCancelled, isCancelled },
                { OTRequestParameters.Instance.StartDate, startdatetime },
                { OTRequestParameters.Instance.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { OTRequestParameters.Instance.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Instance.FiltrApproving, ref outParameters, parameters))
                {
                    ots = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return ots;
        }
        public List<OTRequest> GetList(long personnelid)
        {
            var ots = new List<OTRequest>();
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Instance.Get, parameters))
                {
                    ots = ds.GetList(Converter);
                }
            }

            return ots;
        }

        public List<OTRequest> GetApprovedOT(long personnelid, DateTime startdatetime, DateTime enddatetime)
        {
            var ots = new List<OTRequest>();
            var parameters = new Dictionary<string, object>{
                { OTRequestParameters.Instance.PersonnelID, personnelid },
                { OTRequestParameters.Instance.StartDate, startdatetime },
                { OTRequestParameters.Instance.EndDate, enddatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Instance.GetApprovedOT, parameters))
                {
                    ots = ds.GetList(Converter);
                }
            }
            return ots;
        }
        public OTRequest Get(long id)
        {

            var ot = new OTRequest();
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Instance.Get, parameters))
                {
                    ot = ds.Get(Converter);
                }
            }

            return ot;
        }

        public OTRequest CreateOrUpdate(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.PersonnelID, ot.PersonnelID }
                , { OTRequestParameters.Instance.IsOffice, ot.IsOffice }
                , { OTRequestParameters.Instance.RequestDate, ot.RequestDate }
                , { OTRequestParameters.Instance.Reasons, ot.Reasons }
                , { CredentialParameters.LogBy, userid }
            };

            var outparameter = new List<OutParameters> {
                { OTRequestParameters.Instance.ID, SqlDbType.BigInt, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Instance.CreateOrUpdate, ref outparameter, parameters);
                ot = Get(outparameter.Get(OTRequestParameters.Instance.ID).ToLong());
            }

            return ot;
        }

        public void Approve(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.ID, ot.ID }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Instance.Approve, parameters);
            }
        }

        public void Cancel(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.ID, ot.ID },
                { OTRequestParameters.Instance.CancellationRemarks, ot.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Instance.Cancel, parameters);
            }
        }

        public void Delete(OTRequest ot)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.Instance.ID, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Instance.Delete, parameters);
            }
        }
    }
}
