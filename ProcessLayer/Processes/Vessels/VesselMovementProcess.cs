using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using ProcessLayer.Helpers.ObjectParameter.VesselCrewMovement;
using ProcessLayer.Helpers.ObjectParameter.VesselMovement;
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
                ID = dr[VesselMovementFields.ID].ToShort(),
                VesselID = dr[VesselMovementFields.VesselID].ToShort(),
                Place = dr[VesselMovementFields.Place].ToString(),
                MovementTypeID = dr[VesselMovementFields.MovementTypeID].ToInt(),
                MovementDate = dr[VesselMovementFields.MovementDate].ToDateTime()
            };

            if(!IsVesselMovementOnly)
            {
                v._Vessel = VesselProcess.Instance.Get(v.VesselID);
                v._VesselMovementType = LookupProcess.GetVesselMovementType(v.MovementTypeID);
            }

            return v;
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

        public static CrewMovement GetTransferredVessel (long previouscrewmovementid)
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
                , {VesselMovementParameters.MovementTypeID, vessel.MovementTypeID}
                , {VesselMovementParameters.MovementDate, vessel.MovementDate}
                , {VesselMovementParameters.Place, vessel.Place}
                , {CredentialParameters.LogBy, userid}
            };
            var outparameters = new List<OutParameters> {
                {VesselMovementParameters.ID,SqlDbType.BigInt, vessel.ID }
            };
            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(VesselMovementProcedures.CreateOrUpdate, ref outparameters, parameters);
                vessel.ID = outparameters.Get(VesselMovementParameters.ID).ToLong();

                vessel._Vessel = VesselProcess.Instance.Get(vessel.VesselID);
                vessel._VesselMovementType = LookupProcess.GetVesselMovementType(vessel.MovementTypeID);
            }
            return vessel;
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

    }
}
