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
    public sealed class OTRequestProcess
    {
        public static readonly Lazy<OTRequestProcess> Instance = new Lazy<OTRequestProcess>(() => new OTRequestProcess());
        private OTRequestProcess() { }
        internal bool IsOTRequestOnly = false;
        internal OTRequest Converter(DataRow dr)
        { 
            var o = new OTRequest
            {
                ID = dr[OTRequestFields.ID].ToLong(),
                PersonnelID = dr[OTRequestFields.PersonnelID].ToNullableLong(),
                IsOffice = dr[OTRequestFields.IsOffice].ToNullableBoolean() ?? false,
                OTType = dr[OTRequestFields.OTType].ToOTTYpe(),
                RequestDate = dr[OTRequestFields.RequestDate].ToDateTime(),
                StartDateTime = dr[OTRequestFields.StartDateTime].ToNullableDateTime(),
                EndDateTime = dr[OTRequestFields.EndDateTime].ToNullableDateTime(),
                Reasons = dr[OTRequestFields.Reasons].ToString(),
                Approved = dr[OTRequestFields.Approved].ToNullableBoolean(),
                ApprovedBy = dr[OTRequestFields.ApprovedBy].ToNullableInt(),
                ApprovedOn = dr[OTRequestFields.ApprovedOn].ToNullableDateTime(),
                Cancelled = dr[OTRequestFields.Cancelled].ToNullableBoolean(),
                CancelledBy = dr[OTRequestFields.CancelledBy].ToNullableInt(),
                CancelledOn = dr[OTRequestFields.CancelledOn].ToNullableDateTime(),
                CancellationRemarks = dr[OTRequestFields.CancellationRemarks].ToString(),
                CreatedBy = dr[OTRequestFields.CreatedBy].ToNullableInt(),
                CreatedOn = dr[OTRequestFields.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[OTRequestFields.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[OTRequestFields.ModifiedOn].ToNullableDateTime()
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

        public List<OTRequest> GetList(long personnelId, bool isExpired, bool isPending, bool isApproved, bool isCancelled, DateTime? startdatetime, DateTime? enddatetime, int page, int gridCount, out int PageCount)
        {
            var ots = new List<OTRequest>();
            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { OTRequestParameters.IsExpired, isExpired },
                { OTRequestParameters.IsPending, isPending },
                { OTRequestParameters.IsApproved, isApproved },
                { OTRequestParameters.IsCancelled, isCancelled },
                { OTRequestParameters.StartDate, startdatetime },
                { OTRequestParameters.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Filter, ref outParameters, parameters))
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
                { OTRequestParameters.Personnel, personnel },
                { OTRequestParameters.IsPending, isPending },
                { OTRequestParameters.IsApproved, isApproved },
                { OTRequestParameters.IsCancelled, isCancelled },
                { OTRequestParameters.StartDate, startdatetime },
                { OTRequestParameters.EndDate, enddatetime },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount },
                { OTRequestParameters.Approver, approver }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };


            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.FiltrApproving, ref outParameters, parameters))
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
                { OTRequestParameters.PersonnelID, personnelid }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Get, parameters))
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
                { OTRequestParameters.PersonnelID, personnelid },
                { OTRequestParameters.StartDate, startdatetime },
                { OTRequestParameters.EndDate, enddatetime }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.GetApprovedOT, parameters))
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
                { OTRequestParameters.ID, id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(OTRequestProcedures.Get, parameters))
                {
                    ot = ds.Get(Converter);
                }
            }

            return ot;
        }

        public OTRequest CreateOrUpdate(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.PersonnelID, ot.PersonnelID }
                , { OTRequestParameters.IsOffice, ot.IsOffice }
                , { OTRequestParameters.OTType, ot.OTType }
                , { OTRequestParameters.RequestDate, ot.RequestDate }
                , { OTRequestParameters.StartDateTime, ot.StartDateTime }
                , { OTRequestParameters.EndDateTime, ot.EndDateTime }
                , { OTRequestParameters.Reasons, ot.Reasons }
                , { CredentialParameters.LogBy, userid }
            };

            var outparameter = new List<OutParameters> {
                { OTRequestParameters.ID, SqlDbType.BigInt, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.CreateOrUpdate, ref outparameter, parameters);
                ot = Get(outparameter.Get(OTRequestParameters.ID).ToLong());
            }

            return ot;
        }

        public void Approve(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.ID, ot.ID }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Approve, parameters);
            }
        }

        public void Cancel(OTRequest ot, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.ID, ot.ID },
                { OTRequestParameters.CancellationRemarks, ot.CancellationRemarks }
                , { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Cancel, parameters);
            }
        }

        public void Delete(OTRequest ot)
        {
            var parameters = new Dictionary<string, object> {
                { OTRequestParameters.ID, ot.ID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(OTRequestProcedures.Delete, parameters);
            }
        }
    }
}
