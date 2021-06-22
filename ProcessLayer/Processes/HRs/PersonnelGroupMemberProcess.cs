using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System;
using System.Linq;

namespace ProcessLayer.Processes
{
    public static class PersonnelGroupMemberProcess
    {
        private static bool IsMemberOnly { get; set; } = false;
        internal static PersonnelGroupMember Converter(DataRow dr)
        {
            var p = new PersonnelGroupMember
            {
                ID = dr[PersonnelGroupMemberFields.ID].ToLong(),
                PersonnelGroupID = dr[PersonnelGroupMemberFields.PersonnelGroupID].ToNullableInt(),
                PersonnelID = dr[PersonnelGroupMemberFields.PersonnelID].ToNullableLong(),
                Deleted = dr[PersonnelGroupMemberFields.Deleted].ToBoolean()
            };

            p._Personnel = PersonnelProcess.Get(p.PersonnelID ?? 0, true) ?? new Personnel();
            p._Personnel._Departments = PersonnelDepartmentProcess.GetList(p._Personnel?.ID ?? 0).OrderByDescending(x => x.StartDate)?.Take(1)?.ToList();
            p._Personnel._Positions = PersonnelPositionProcess.GetList(p._Personnel?.ID ?? 0).OrderByDescending(x => x.StartDate)?.Take(1)?.ToList();

            if (!IsMemberOnly)
            {
                p._PersonnelGroup = PersonnelGroupProcess.Get(p.PersonnelGroupID ?? 0, true);
            }
            return p;

        }

        public static PersonnelGroupMember Get(long Id, bool isMemberOnly = false)
        {
            var eb = new PersonnelGroupMember();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupMemberParameters.ID, Id}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupMemberProcedures.Get, Parameters))
                {
                    IsMemberOnly = isMemberOnly;
                    eb = ds.Get(Converter);
                    IsMemberOnly = false;
                }
            }

            return eb;
        }

        public static List<PersonnelGroupMember> GetByGroup(int GroupId, bool isMemberOnly = false)
        {
            var eb = new List<PersonnelGroupMember>();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupMemberParameters.PersonnelGroupID, GroupId}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupMemberProcedures.Get, Parameters))
                {
                    IsMemberOnly = isMemberOnly;
                    eb = ds.GetList(Converter);
                    IsMemberOnly = false;
                }
            }

            return eb;
        }

        public static List<PersonnelGroupMember> GetList()
        {
            var eb = new List<PersonnelGroupMember>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupMemberProcedures.Get))
                {
                    eb = ds.GetList(Converter);
                }
            }

            return eb;
        }


        public static PersonnelGroupMember CreateOrUpdate(PersonnelGroupMember personnelGroupMember, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupMemberParameters.PersonnelGroupID, personnelGroupMember.PersonnelGroupID },
                { PersonnelGroupMemberParameters.PersonnelID, personnelGroupMember.PersonnelID },
                { PersonnelGroupMemberParameters.Deleted, personnelGroupMember.Deleted },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelGroupMemberParameters.ID, SqlDbType.BigInt, personnelGroupMember.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelGroupMemberProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                if(personnelGroupMember.ID != 0)
                    personnelGroupMember.ID = OutParameters.Get(PersonnelGroupMemberParameters.ID).ToLong();
            }

            return personnelGroupMember;
        }

        public static List<PersonnelGroupMember> CreateOrUpdate(List<PersonnelGroupMember> personnelGroupMembers, int userid)
        {
            foreach (var personnelGroupMember in personnelGroupMembers)
            {
                using (var db = new DBTools())
                {
                    db.StartTransaction();
                    try
                    {
                        var Parameters = new Dictionary<string, object>
                        {
                            { PersonnelGroupMemberParameters.PersonnelGroupID, personnelGroupMember.PersonnelGroupID },
                            { PersonnelGroupMemberParameters.PersonnelID, personnelGroupMember.PersonnelID },
                            { PersonnelGroupMemberParameters.Deleted, personnelGroupMember.Deleted },
                            { CredentialParameters.LogBy, userid }
                        };
                        var OutParameters = new List<OutParameters>
                        {
                            { PersonnelGroupMemberParameters.ID, SqlDbType.BigInt, personnelGroupMember.ID}
                        };

                        db.ExecuteNonQuery(PersonnelGroupMemberProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                        if (personnelGroupMember.ID != 0)
                            personnelGroupMember.ID = OutParameters.Get(PersonnelGroupMemberParameters.ID).ToLong();
                        db.CommitTransaction();
                    }
                    catch(Exception ex)
                    {
                        db.RollBackTransaction();
                        throw ex;
                    }
                }
                personnelGroupMember._Personnel = PersonnelProcess.Get(personnelGroupMember.PersonnelID??0, true);
            }
            return personnelGroupMembers;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupMemberFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelGroupMemberProcedures.Delete, Parameters);
            }
        }
    }
}
