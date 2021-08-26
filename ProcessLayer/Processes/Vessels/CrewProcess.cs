using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using System;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using ProcessLayer.Processes.Lookups;

namespace ProcessLayer.Processes
{
    public static class CrewProcess
    {
        internal static bool IsCrewOnly = false;

        internal static Crew Converter(DataRow dr)
        {
            var c = new Crew
            {
                ID = dr[CrewFields.ID].ToLong(),
                VesselID = dr[CrewFields.VesselID].ToNullableShort(),
                PersonnelID = dr[CrewFields.PersonnelID].ToNullableLong(),
                PositionID = dr[CrewFields.PositionID].ToNullableShort(),
                DepartmentID = dr[CrewFields.DepartmentID].ToNullableInt(),
                StartDate = dr[CrewFields.StartDate].ToNullableDateTime(),
                EndDate = dr[CrewFields.EndDate].ToNullableDateTime(),
                Remarks = dr[CrewFields.Remarks].ToString()
            };

            c._Personnel = PersonnelProcess.Get(c.PersonnelID ?? 0, true);

            if (!IsCrewOnly)
            {
                c._Vessel = VesselProcess.Instance.Value.Get(c.VesselID);
                c._Personnel = PersonnelProcess.Get(c.PersonnelID??0, true);
                c._Position = PositionProcess.Instance.Value.Get(c.PositionID);
                c._Department = DepartmentProcess.Instance.Value.Get(c.DepartmentID ?? 0);
            }
            return c;
        }
        
        public static Crew Get(long Id, bool isCrewOnly = false)
        {
            var eb = new Crew();

            var Parameters = new Dictionary<string, object>
            {
                { CrewParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewProcedures.Get, Parameters))
                {
                    IsCrewOnly = isCrewOnly;
                    eb = ds.Get(Converter);
                    IsCrewOnly = false;
                }
            }

            return eb;
        }

        public static List<Crew> GetList(string name, short? vesselID, DateTime? startdate, DateTime? enddate, short? positionid, int? departmentid, string remarks, int page, int gridCount, out int PageCount)
        {
            var emp = new List<Crew>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Name, name},
                { CrewParameters.VesselID, vesselID},
                { CrewParameters.StartDate, startdate},
                { CrewParameters.EndDate, enddate},
                { CrewParameters.DepartmentID, departmentid},
                { CrewParameters.PositionID, positionid},
                { CrewParameters.Remarks, remarks},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(CrewProcedures.Filter, ref outParameters, Parameters))
                {
                    emp = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return emp;
        }

        public static List<Crew> GetList(string filter)
        {
            var emp = new List<Crew>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.SearchPersonnel, Parameters))
                {
                    emp = ds.GetList(Converter);
                }
            }

            return emp;
        }

        public static Crew CreateOrUpdate(Crew crew, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { CrewParameters.VesselID, crew.VesselID },
                { CrewParameters.PersonnelID, crew.PersonnelID },
                { CrewParameters.PositionID, crew.PositionID },
                { CrewParameters.DepartmentID, crew.DepartmentID },
                { CrewParameters.StartDate, crew.StartDate },
                { CrewParameters.EndDate, crew.EndDate },
                { CrewParameters.Remarks, crew.Remarks },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { CrewParameters.ID, SqlDbType.BigInt, crew.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                crew.ID = OutParameters.Get(CrewParameters.ID).ToLong();
            }

            return crew;
        }
        
        public static void Delete(long id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { CrewFields.ID, id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(CrewProcedures.Delete, Parameters);
            }
        }
    }
}
