using DBUtilities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes.Kiosks
{
    public sealed class OuterPortRequestProcess
	{
		public static readonly Lazy<OuterPortRequestProcess> Instance = new Lazy<OuterPortRequestProcess>(() => new OuterPortRequestProcess());
		private OuterPortRequestProcess() { }
		internal bool IsOuterPortRequestOnly { get; set; } = true;
		internal OuterPortRequest Converter(DataRow dr)
		{
			var o = new OuterPortRequest
			{
				ID = dr["ID"].ToLong(),
				PersonnelID = dr["Personnel ID"].ToLong(),
				StartDate = dr["Start Date"].ToNullableDateTime(),
				EndDate = dr["End Date"].ToNullableDateTime(),
				LocationID = dr["Location ID"].ToNullableByte(),
				IsHighRisk = dr["Is High Risk"].ToNullableBoolean(),
				HasQuarantine = dr["Has Quarantine"].ToNullableBoolean(),
				QuarantineDateEnd = dr["Quarantine Date End"].ToNullableDateTime(),
				Purpose = dr["Purpose"].ToString(),
				CancellationRemarks = dr["Cancellation Remarks"].ToString(),
				Cancelled = dr["Cancelled"].ToNullableBoolean(),
				CancelledBy = dr["Cancelled By"].ToNullableInt(),
				CancelledOn = dr["Cancelled On"].ToNullableDateTime(),
				CreatedBy = dr["Created By"].ToNullableInt(),
				CreatedOn = dr["Created On"].ToNullableDateTime(),
				ModifiedBy = dr["Modified By"].ToNullableInt(),
				ModifiedOn = dr["Modified On"].ToNullableDateTime()
			};

			o._Personnel = PersonnelProcess.Get(o.PersonnelID??0, true);
			o._Location = LocationProcess.Instance.Value.Get(o.LocationID);
			if(!IsOuterPortRequestOnly)
			{
				o._Cancel = LookupProcess.GetUser(o.CancelledBy);
				o._Creator = LookupProcess.GetUser(o.CreatedBy);
				o._Modifier = LookupProcess.GetUser(o.ModifiedBy);
			}
			return o;
		}

		public List<OuterPortRequest> GetList(string personnel, int? locationid, bool isCancelled, DateTime? startdate, DateTime? enddate, int page, int gridCount, out int PageCount)
		{
			var Leaves = new List<OuterPortRequest>();
			var parameters = new Dictionary<string, object> {
				{ "@Personnel", personnel },
				{ "@LocationID", locationid },
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
				using (var ds = db.ExecuteReader("[kiosk].[FilterOuterPortRequest]", ref outParameters, parameters))
				{
					Leaves = ds.GetList(Converter);
					PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
				}
			}

			return Leaves;
		}

		public List<OuterPortRequest> Get()
		{
			var o = new List<OuterPortRequest>();
			using (var db = new DBTools())
			{
				using (var ds = db.ExecuteReader("kiosk.GetOuterPortRequest"))
				{
					o = ds.GetList(Converter);
				}
			}
			return o;
		}

		public OuterPortRequest Get(long id)
		{
			var o = new OuterPortRequest();
			using (var db = new DBTools())
			{
				using (var ds = db.ExecuteReader("kiosk.GetOuterPortRequest", new Dictionary<string, object> { {"@ID", id } }))
				{
					o = ds.Get(Converter);
				}
			}
			return o;
		}

		public List<OuterPortRequest> GetApprovedOuterPort(long personnelid, byte? locationid, DateTime startdate, DateTime enddate)
		{
			var OP = new List<OuterPortRequest>();
			var parameters = new Dictionary<string, object>{
				{ "@PersonnelID", personnelid },
				{ "@LocationID", locationid },
				{ "@StartDate", startdate },
				{ "@EndDate", enddate }
			};
			using (var db = new DBTools())
			{
				using (var ds = db.ExecuteReader("[kiosk].[GetApprovedOuterPortRequest]", parameters))
				{
					OP = ds.GetList(Converter);
				}
			}
			return OP;
		}
		public List<OuterPortRequest> GetByPersonnelID(long personnelID)
		{
			var o = new List<OuterPortRequest>();
			using (var db = new DBTools())
			{
				using (var ds = db.ExecuteReader("kiosk.GetOuterPortRequest", new Dictionary<string, object> { { "@PersonnelID", personnelID } }))
				{
					o = ds.GetList(Converter);
				}
			}
			return o;
		}

		public OuterPortRequest CreateOrUpdate(OuterPortRequest outerPortRequest, int userid)
		{
			var parameters = new Dictionary<string, object> {
				{ "@PersonnelID", outerPortRequest.PersonnelID },
				{ "@StartDate", outerPortRequest.StartDate },
				{ "@EndDate", outerPortRequest.EndDate },
				{ "@Purpose", outerPortRequest.Purpose },
				{ "@LocationID", outerPortRequest.LocationID },
				{ "@IsHighRisk", outerPortRequest.IsHighRisk },
				{ "@HasQuarantine", outerPortRequest.HasQuarantine },
				{ "@QuarantineDateEnd", outerPortRequest.QuarantineDateEnd },
				{ "@LogBy",  userid}
			};
			
			var outparameters = new List<OutParameters> {
				{ "@ID", SqlDbType.BigInt, outerPortRequest.ID}
			};

			using (var db = new DBTools())
			{
				db.ExecuteNonQuery("kiosk.CreateOrUpdateOuterPortRequest", ref outparameters,  parameters);
				outerPortRequest.ID = outparameters.Get("@ID").ToLong();
			}
			return outerPortRequest;
		}
		public void Cancel(long id, int userid, string cancellationremarks)
		{
			var parameters = new Dictionary<string, object> {
				{ "@ID", id },
				{ "@CancellationRemarks",  cancellationremarks},
				{ "@LogBy",  userid}
			};

			using (var db = new DBTools())
			{
				db.ExecuteNonQuery("kiosk.CancelOuterPortRequest", parameters);
			}
		}

		public void Delete(long ID)
		{
			var parameters = new Dictionary<string, object> {
				{ "@ID", ID }
			};

			using (var db = new DBTools())
			{
				db.ExecuteNonQuery("[kiosk].[DeleteOuterPortRequest]", parameters);
			}
		}
	}
}
