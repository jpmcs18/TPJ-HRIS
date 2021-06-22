using ProcessLayer.Entities;
using System.Collections.Generic;
using System.Data;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using System.IO;
using System.Configuration;
using ProcessLayer.Helpers.ObjectParameter.MemoArchive;
using System;
using ProcessLayer.Helpers.ObjectParameter.Personnel;

namespace ProcessLayer.Processes
{
    public static class PersonnelGroupProcess
    {
        private static bool IsPersonnelGroupOnly { get; set; } = false;

        internal static PersonnelGroup Converter(DataRow dr)
        {
            var p = new PersonnelGroup
            {
                ID = dr[PersonnelGroupFields.ID].ToInt(),
                Description = dr[PersonnelGroupFields.Description].ToString()
            };

            if(!IsPersonnelGroupOnly)
                p._PersonnelGroupMembers = PersonnelGroupMemberProcess.GetByGroup(p.ID, true);

            return p;

        }

        public static PersonnelGroup Get(int Id, bool personnelGroupOnly = false)
        {
            var eb = new PersonnelGroup();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupParameters.ID, Id}
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupProcedures.Get, Parameters))
                {
                    IsPersonnelGroupOnly = personnelGroupOnly;
                    eb = ds.Get(Converter);
                    IsPersonnelGroupOnly = false;
                }
            }

            return eb;
        }

        public static List<PersonnelGroup> GetList()
        {
            var eb = new List<PersonnelGroup>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupProcedures.Get))
                {
                    IsPersonnelGroupOnly = true;
                    eb = ds.GetList(Converter);
                    IsPersonnelGroupOnly = false;
                }
            }

            return eb;
        }

        public static List<PersonnelGroup> GetList(string filter, int page, int gridCount, out int PageCount)
        {
            var emp = new List<PersonnelGroup>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupProcedures.Filter, ref outParameters, Parameters))
                {
                    emp = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return emp;
        }

        public static List<PersonnelGroup> GetList(string filter)
        {
            var emp = new List<PersonnelGroup>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupProcedures.Search, Parameters))
                {
                    IsPersonnelGroupOnly = true;
                    emp = ds.GetList(Converter);
                    IsPersonnelGroupOnly = false;
                }
            }

            return emp;
        }

        public static List<PersonnelGroup> GetMemoArchivesGroup(long MemoId, bool isPersonnelGroupOnly = false)
        {
            var emp = new List<PersonnelGroup>();

            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.ID, MemoId },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGroupProcedures.GetMemoGroup, Parameters))
                {
                    IsPersonnelGroupOnly = isPersonnelGroupOnly;
                    emp = ds.GetList(Converter);
                    IsPersonnelGroupOnly = false;
                }
            }

            return emp;
        }

        public static PersonnelGroup CreateOrUpdate(PersonnelGroup personnelGroup, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupParameters.Description, personnelGroup.Description },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { PersonnelGroupParameters.ID, SqlDbType.Int, personnelGroup.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelGroupProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                personnelGroup.ID = OutParameters.Get(PersonnelGroupParameters.ID).ToInt();
                
                personnelGroup._PersonnelGroupMembers = PersonnelGroupMemberProcess.GetByGroup(personnelGroup.ID, true);
            }

            return personnelGroup;
        }

        public static void Delete(int Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelGroupFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelGroupProcedures.Delete, Parameters);
            }
        }
    }
}
