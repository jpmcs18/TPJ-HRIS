using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using ProcessLayer.Helpers.ObjectParameter.VesselCrewMovement;
using ProcessLayer.Helpers.ObjectParameter.VesselMovement;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes
{
    public static class VesselMovementProcess
    {
        internal static bool IsVesselMovementOnly = false;

        internal static VesselMovement Converter(DataRow dr)
        {
            var v = new VesselMovement {
                ID = dr["ID"].ToLong(),
                VesselID = dr["Vessel ID"].ToInt(),
                VoyageStartDate = dr["Voyage Start Date"].ToDateTime(),
                VoyageEndDate = dr["Voyage End Date"].ToNullableDateTime(),
                OriginLocationID = dr["Origin Location ID"].ToInt(),
                DestinationLocationID = dr["Destination Location ID"].ToNullableInt(),
                ETA = dr["ETA"].ToDateTime(),
                ETD = dr["ETD"].ToNullableDateTime(),
                VoyageDetails = dr["Voyage Details"].ToString(),
                MovementStatusID = dr["Movement Status ID"].ToNullableInt() ?? 0, //1: Cancel, 2: Pending, 3: Checked, 4: Approved
                CreatedDate = dr["Created Date"].ToDateTime(),
                ModifiedDate = dr["Modified Date"].ToNullableDateTime(),

                CreatedBy = dr["Created By"].ToInt(),
                //Checker = dr["Checker"].ToString(),
                CheckedDate = dr["Checked Date"].ToNullableDateTime(),
                CheckedBy = dr["Checked By"].ToNullableInt(),
                //Approver = dr["Approver"].ToString(),
                ApprovedDate = dr["Approved Date"].ToNullableDateTime(),
                ApprovedBy = dr["Approved By"].ToNullableInt(),
            };
            if(!IsVesselMovementOnly)
            { 
                v._Vessel = VesselProcess.Instance.Get(v.VesselID);
                v.OriginLocation = LocationProcess.Instance.Get(v.OriginLocationID);
                v.DestinationLocation = LocationProcess.Instance.Get(v.DestinationLocationID);
                v.VesselMovementCrewList = GetMovementCrews(v.ID);
            }
            v.Creator = LookupProcess.GetUser(v.CreatedBy);
            v.Checker = LookupProcess.GetUser(v.CheckedBy);
            v.Approver = LookupProcess.GetUser(v.ApprovedBy);

            return v;
        }
        internal static VesselMovementCrews CrewConverter(DataRow dr)
        {
            var v = new VesselMovementCrews
            {
                ID = dr["ID"].ToLong(),
                VesselMovementID = dr["Vessel Movement ID"].ToLong(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                DepartmentID = dr["Department ID"].ToNullableInt(),
                PositionID = dr["Position ID"].ToInt(),
                DailyRate = dr["Daily Rate"].ToNullableDecimal(),
                Remarks = dr["Remarks"].ToString()
            };

            if (!IsVesselMovementOnly)
            {
                v.Position = PositionProcess.Instance.Get(v.PositionID);
                v.Department = DepartmentProcess.Instance.Get(v.DepartmentID ?? 0);
                v.Personnel = PersonnelProcess.Get(v.PersonnelID, true);
            }

            if(v.DailyRate == null)
            {
                v.DailyRate = PositionSalaryProcess.GetByPositionId(v.PositionID)?.Salary;
            }

            return v;
        }
        public static List<VesselMovementCrews> GetMovementCrews(long vesselMovementId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@MovementID", vesselMovementId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("vessel.GetVesselMovementCrew", parameters))
                {
                    return ds.GetList(CrewConverter);
                }
            }
        }
        public static List<CrewDetails> GetCrewDetailList(int vesselid, DateTime? startingdate, DateTime? endingdate)
        {
            var crewlist = new List<CrewDetails>();
            var crews = GetCrewList(vesselid, startingdate, endingdate);
            if (crews != null && crews.Any())
            {
                crews.ForEach(crew =>
                {   
                    var crewdetails = new CrewDetails() { Crew = crew };
                    if (crew.OffboardDate < endingdate)
                    {
                        var trans = GetTransferredVessel(crew.ID);
                        if (trans != null)
                        {
                            crewdetails.Reference = trans.TransactionNo;

                            if (trans.VesselID != null)
                            {
                                crewdetails.FromCrew = crew;
                                crewdetails.ToCrew = trans;
                            }
                            else if (trans.SNVesselID != null)
                            {
                                crewdetails.FromCrew = trans;
                                crewdetails.ToCrew = GetTransferredVessel(trans.ID);
                                if (crewdetails.ToCrew != null)
                                {
                                    if (crewdetails.ToCrew.VesselID == null && crewdetails.ToCrew.SNVesselID == null)
                                    {
                                        crewdetails.ToCrew = null;
                                        crewdetails.Disembarked = trans.OnboardDate;
                                    }
                                }
                                else
                                {
                                    crewdetails.FromCrew = crew;
                                    crewdetails.ToCrew = trans;
                                }
                            }
                            else
                            {
                                crewdetails.Disembarked = trans.OnboardDate;
                            }
                        }
                    }
                    crewlist.Add(crewdetails);
                });
            }
            return crewlist;
        }
        public static List<CrewMovement> GetCrewList(int vesselid, DateTime? startingdate, DateTime? endingdate)
        {
            var crews = new List<CrewMovement>();
            var Parameters = new Dictionary<string, object>
            {
                { CrewParameters.VesselID, vesselid },
                { CrewMovementParameters.StartingDate, startingdate },
                { CrewMovementParameters.EndingDate, endingdate }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.GetVesselCrews, Parameters))
                {
                    CrewMovementProcess.WithPreviousCrewMovement = false;
                    crews = ds.GetList(CrewMovementProcess.Converter);
                    CrewMovementProcess.WithPreviousCrewMovement = true;
                }
            }
            return crews;
        }

        public static List<CrewMovement> GetCrewList(int vesselid, long personnelId, DateTime? startingdate, DateTime? endingdate)
        {
            var crews = new List<CrewMovement>();
            var Parameters = new Dictionary<string, object>
            {
                { CrewParameters.VesselID, vesselid },
                { CrewParameters.PersonnelID, personnelId },
                { CrewMovementParameters.StartingDate, startingdate },
                { CrewMovementParameters.EndingDate, endingdate }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.GetVesselCrews, Parameters))
                {
                    CrewMovementProcess.WithPreviousCrewMovement = false;
                    crews = ds.GetList(CrewMovementProcess.Converter);
                    CrewMovementProcess.WithPreviousCrewMovement = true;
                }
            }
            return crews;
        }

        public static CrewMovement GetTransferredVessel(long previouscrewmovementid)
        {
            var crew = new CrewMovement();
            var Parameters = new Dictionary<string, object>
            {
                { CrewMovementParameters.PreviousCrewMovementID, previouscrewmovementid },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewMovementProcedures.GetTransferredVessel, Parameters))
                {
                    CrewMovementProcess.WithPreviousCrewMovement = false;
                    crew = ds.Get(CrewMovementProcess.Converter);
                    CrewMovementProcess.WithPreviousCrewMovement = true;
                }
            }
            return crew;
        }
        public static VesselMovement Get(long id, bool isVesselMovementOnly = false)
        {
            var vms = new VesselMovement();
            var parameters = new Dictionary<string, object> {
                { VesselMovementParameters.ID, id }
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselMovementProcedures.Get, parameters))
                {
                    IsVesselMovementOnly = isVesselMovementOnly;
                    vms = ds.Get(Converter);
                    IsVesselMovementOnly = false;
                }
            }
            return vms;
        }
        public static List<VesselMovement> GetList(int vesselid, DateTime? startingdate = null, DateTime? endingdate = null, bool isVesselMovementOnly = false)
        {
            var vms = new List<VesselMovement>();

            var parameters = new Dictionary<string, object> {
                { VesselMovementParameters.VesselID, vesselid }
                , {VesselMovementParameters.StartingDate, startingdate}
                , {VesselMovementParameters.EndingDate, endingdate}
            };
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(VesselMovementProcedures.Get, parameters))
                {
                    IsVesselMovementOnly = isVesselMovementOnly;
                    vms = ds.GetList(Converter);
                    IsVesselMovementOnly = false;
                }
            }

            return vms;
        }
        public static List<VesselMovement> GetLastTwoMovement(long vesselID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("vessel.GetLastTwoVesselMovement", new Dictionary<string, object> {
                    { "@VesselID", vesselID }
                }))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public static VesselMovement CreateOrUpdate(VesselMovement vessel, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {VesselMovementParameters.VesselID, vessel.VesselID}
                , {VesselMovementParameters.VoyageStartDate, vessel.VoyageStartDate}
                , {VesselMovementParameters.VoyageEndDate, vessel.VoyageEndDate}
                , {VesselMovementParameters.OriginLocationID, vessel.OriginLocationID}
                , {VesselMovementParameters.DestinationLocationID, vessel.DestinationLocationID}
                , {VesselMovementParameters.ETD, vessel.ETD}
                , {VesselMovementParameters.ETA, vessel.ETA}
                , {VesselMovementParameters.VoyageDetails, vessel.VoyageDetails}
                , {CredentialParameters.LogBy, userid}
            };
            var outparameters = new List<OutParameters> {
                {VesselMovementParameters.ID, SqlDbType.BigInt, vessel.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselMovementProcedures.CreateOrUpdate, ref outparameters, parameters);
                vessel.ID = outparameters.Get(VesselMovementParameters.ID).ToLong();

                vessel._Vessel = VesselProcess.Instance.Get(vessel.VesselID);
            }

            CreateOrUpdateCrew(vessel.VesselMovementCrewList, userid);

            return vessel;
        }

        public static List<VesselMovementCrews> CreateOrUpdateCrew(List<VesselMovementCrews> crews, int userid)
        {
            for (int i = 0; i < crews.Count; i++)
            {
                var crew = crews[i];

                var parameters = new Dictionary<string, object> {
                    {VesselMovementParameters.PersonnelID, crew.PersonnelID}
                    , {VesselMovementParameters.VesselMovementID, crew.VesselMovementID}
                    , {VesselMovementParameters.DepartmentID, crew.DepartmentID}
                    , {VesselMovementParameters.PositionID, crew.PositionID}
                    , {VesselMovementParameters.DailyRate, crew.DailyRate}
                    , {VesselMovementParameters.Remarks, crew.Remarks}
                    , {CredentialParameters.LogBy, userid}
                };

                var outparameters = new List<OutParameters> {
                    {VesselMovementParameters.ID, SqlDbType.BigInt, crew.ID }
                };
                using (var db = new DBTools())
                {
                    db.ExecuteNonQuery(VesselMovementProcedures.CreateOrUpdateCrew, ref outparameters, parameters);
                    crew.ID = outparameters.Get(VesselMovementParameters.ID).ToLong();

                    crew.Personnel = PersonnelProcess.Get(crew.PersonnelID, true);
                    crew.Department = DepartmentProcess.Instance.Get(crew.DepartmentID ?? 0);
                    crew.Position = PositionProcess.Instance.Get(crew.PositionID);
                }
            }

            return crews;
        }

        public static void DeleteCrew(long vesselmovementcrewid, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {VesselMovementParameters.ID, vesselmovementcrewid }
                , {"@Delete", true }
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselMovementProcedures.CreateOrUpdateCrew, parameters);
            }
        }

        public static void Delete(long vesselmovementid, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {VesselMovementParameters.ID, vesselmovementid }
                , {CredentialParameters.LogBy, userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselMovementProcedures.Delete, parameters);
            }
        }

        public static VesselMovement Checked(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {"@ID", id}
                , {"@LogBy", userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("vessel.CheckedVesselMovement", parameters);
                return Get(id);
            }
        }

        public static VesselMovement Approved(long id, int userid)
        {
            var parameters = new Dictionary<string, object> {
                {"@ID", id}
                , {"@LogBy", userid}
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("vessel.ApprovedVesselMovement", parameters);
                return Get(id);
            }
        }
    }
}
