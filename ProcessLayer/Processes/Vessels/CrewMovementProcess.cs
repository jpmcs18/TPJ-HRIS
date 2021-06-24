using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using ProcessLayer.Helpers.ObjectParameter.VesselCrewMovement;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes
{
    public static class CrewMovementProcess
    {
        internal static bool IsCrewMovementOnly = false;
        internal static bool WithPreviousCrewMovement = true;
        internal static CrewMovement Converter(DataRow dr)
        {
            var c = new CrewMovement
            {
                ID = dr[CrewMovementFields.ID].ToLong(),
                TransactionNo = dr[CrewMovementFields.TransactionNo].ToString(),
                PersonnelID = dr[CrewMovementFields.PersonnelID].ToLong(),
                PreviousCrewMovementID = dr[CrewMovementFields.PreviousCrewMovementID].ToNullableInt(),
                DepartmentID = dr[CrewMovementFields.DepartmentID].ToNullableInt(),
                PositionID = dr[CrewMovementFields.PositionID].ToNullableInt(),
                VesselID = dr[CrewMovementFields.VesselID].ToNullableInt(),
                SNPositionID = dr[CrewMovementFields.SNPositionID].ToNullableInt(),
                SNVesselID = dr[CrewMovementFields.SNVesselID].ToNullableInt(),
                OffboardDate = dr[CrewMovementFields.OffboardDate].ToNullableDateTime(),
                OnboardDate = dr[CrewMovementFields.OnboardDate].ToNullableDateTime(),
                Status = dr[CrewMovementFields.Status].ToInt(),
                Remarks = dr[CrewMovementFields.Remarks].ToString(),
                CreatedBy = dr[CrewMovementFields.CreatedBy].ToNullableInt(),
                CheckedBy = dr[CrewMovementFields.CheckedBy].ToNullableInt(),
                ModifiedBy = dr[CrewMovementFields.ModifiedBy].ToNullableInt(),
                NotedBy = dr[CrewMovementFields.NotedBy].ToNullableInt(),
                PostedBy = dr[CrewMovementFields.PostedBy].ToNullableInt(),
                CancelledBy = dr[CrewMovementFields.CancelledBy].ToNullableInt(),
                CreatedDate = dr[CrewMovementFields.CreatedDate].ToNullableDateTime(),
                CheckedDate = dr[CrewMovementFields.CheckedDate].ToNullableDateTime(),
                ModifiedDate = dr[CrewMovementFields.ModifiedDate].ToNullableDateTime(),
                NotedDate = dr[CrewMovementFields.NotedDate].ToNullableDateTime(),
                PostedDate = dr[CrewMovementFields.PostedDate].ToNullableDateTime(),
                CancelledDate = dr[CrewMovementFields.CancelledDate].ToNullableDateTime()
            };

            if (WithPreviousCrewMovement)
            {
                c._Creator = LookupProcess.GetUser(c.CreatedBy);
                c._Modify = LookupProcess.GetUser(c.ModifiedBy);
                c._Check = LookupProcess.GetUser(c.CheckedBy);
                c._Note = LookupProcess.GetUser(c.NotedBy);
                c._Post = LookupProcess.GetUser(c.PostedBy);
                c._Cancelled = LookupProcess.GetUser(c.CancelledBy);
                c._PreviousCrewMovement = GetPreviousMovement(c.PersonnelID, c.PreviousCrewMovementID??0);
                if(c._PreviousCrewMovement != null) c._PreviousCrewMovement.ID = c.PreviousCrewMovementID??0;
            }

            if (!IsCrewMovementOnly)
            {
                c._Personnel = PersonnelProcess.Get(c.PersonnelID, true);
                c._Department = DepartmentProcess.Instance.Get(c.DepartmentID ?? 0);
                c._Position = PositionProcess.Instance.Get(c.PositionID);
                c._Vessel = VesselProcess.Instance.Get(c.VesselID);
                c._SNPosition = PositionProcess.Instance.Get(c.SNPositionID);
                c._SNVessel = VesselProcess.Instance.Get(c.SNVesselID);
            }
            return c;
        }

        public static CrewMovement Get(long id, bool isCrewMovementOnly = false)
        {
            var cms = new CrewMovement();
            var parameters = new Dictionary<string, object> {
                { CrewMovementParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.Get, parameters))
                {
                    IsCrewMovementOnly = isCrewMovementOnly;
                    cms = ds.Get(Converter);
                    IsCrewMovementOnly = false;
                }
            }
            return cms;
        }

        public static List<CrewMovement> GetList(long personnelid, int? vesselid, DateTime? startingdate = null, DateTime? endingdate = null, bool isCrewMovementOnly = false)
        {
            var vms = new List<CrewMovement>();
            var parameters = new Dictionary<string, object> {
                { CrewMovementParameters.PersonnelID, personnelid }
                , {CrewMovementParameters.VesselID, vesselid }
                , {CrewMovementParameters.StartingDate, startingdate }
                , {CrewMovementParameters.EndingDate, endingdate }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.Get, parameters))
                {
                    WithPreviousCrewMovement = true;
                    IsCrewMovementOnly = isCrewMovementOnly;
                    vms = ds.GetList(Converter);
                    IsCrewMovementOnly = false;
                }
            }

            var lastMovement = GetLastMovement(personnelid);
            foreach(var c in vms)
            {
                if (c.ID == lastMovement.PreviousCrewMovementID)
                    c.IsLast = true;
                else c.IsLast = false;
            }
            return vms;
        }

        public static CrewMovement GetLastMovement(long personnelId)
        {
            var cms = new CrewMovement();
            var parameters = new Dictionary<string, object> {
                { CrewMovementParameters.PersonnelID, personnelId }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.GetLast, parameters))
                {
                    cms = ds.Get(Converter);
                }
            }

            if(cms != null)
            {
                cms._PreviousCrewMovement = GetPreviousMovement(cms.PersonnelID, cms.PreviousCrewMovementID??0);
            }

            return cms;
        }

        public static CrewMovement GetPreviousMovement(long personnelId, long previousCrewMovementId)
        {
            var cms = new CrewMovement();
            var parameters = new Dictionary<string, object> {
                { CrewMovementParameters.PreviousCrewMovementID, previousCrewMovementId },
                { CrewMovementParameters.PersonnelID, personnelId }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.GetPrevious, parameters))
                {
                    WithPreviousCrewMovement = false;
                    cms = ds.Get(Converter);
                    WithPreviousCrewMovement = true;
                }
            }
            return cms;
        }

        public static List<CrewMovement> GetCrewActualMovement(long personnelid, short? vesselid = null, DateTime? startingdate = null, DateTime? endingdate = null, bool isCrewMovementOnly = false)
        {
            var crews = new List<CrewMovement>();
            var Parameters = new Dictionary<string, object>
            {
                { CrewMovementParameters.PersonnelID, personnelid},
                { CrewMovementParameters.VesselID, vesselid},
                { CrewMovementParameters.StartingDate, startingdate},
                { CrewMovementParameters.EndingDate, endingdate},
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewProcedures.GetCrewActualMovement, Parameters))
                {
                    IsCrewMovementOnly = isCrewMovementOnly;
                    WithPreviousCrewMovement = false;
                    crews = ds.GetList(Converter);
                    WithPreviousCrewMovement = true;
                    IsCrewMovementOnly = false;
                }
            }

            return crews;
        }

        public static CrewMovement CreateOrUpdate(CrewMovement crew, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {CrewMovementParameters.TransactionNo, crew.TransactionNo}
                , {CrewMovementParameters.PersonnelID, crew.PersonnelID}
                , {CrewMovementParameters.PreviousCrewMovementID, crew.PreviousCrewMovementID}
                , {CrewMovementParameters.DepartmentID, crew.DepartmentID}
                , {CrewMovementParameters.PositionID, crew.PositionID}
                , {CrewMovementParameters.VesselID, crew.VesselID}
                , {CrewMovementParameters.SNPositionID, crew.SNPositionID}
                , {CrewMovementParameters.SNVesselID, crew.SNVesselID}
                , {CrewMovementParameters.OnboardDate, crew.OnboardDate}
                , {CrewMovementParameters.OffboardDate, crew.OffboardDate}
                , {CrewMovementParameters.Remarks, crew.Remarks}
                , {CredentialParameters.LogBy, userid}
            };
            var outparameters = new List<OutParameters> {
                {CrewMovementParameters.ID, SqlDbType.BigInt, crew.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewMovementProcedures.CreateOrUpdate, ref outparameters, parameters);
                crew.ID = outparameters.Get(CrewMovementParameters.ID).ToLong();
                crew = Get(crew.ID);
            }
            return crew;
        }

        public static CrewMovement UpdatePostedMovement(CrewMovement crew, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {CrewMovementParameters.ID, crew.ID}
                , {CrewMovementParameters.DepartmentID, crew.DepartmentID}
                , {CrewMovementParameters.PositionID, crew.PositionID}
                , {CrewMovementParameters.SNPositionID, crew.SNPositionID}
                , {CrewMovementParameters.Remarks, crew.Remarks}
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewMovementProcedures.UpdatePosted,parameters);
                crew = Get(crew.ID);
            }
            return crew;
        }

        public static CrewMovement UpdateStatus(CrewMovement crew, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {CrewMovementParameters.ID, crew.ID}
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewMovementProcedures.UpdateStatus, parameters);
                crew = Get(crew.ID);
            }
            return crew;
        }

        public static CrewMovement Cancel(CrewMovement crew, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {CrewMovementParameters.ID, crew.ID}
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewMovementProcedures.Cancel, parameters);
                crew = Get(crew.ID);
            }
            return crew;
        }

        public static void Delete(long crewmovementid, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {CrewMovementParameters.ID, crewmovementid }
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewMovementProcedures.Delete, parameters);
            }
        }
    }
}
