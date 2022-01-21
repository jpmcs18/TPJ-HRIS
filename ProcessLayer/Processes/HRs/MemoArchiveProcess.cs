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
    public static class MemoArchiveProcess
    { 
        private static bool IsMemoOnly { get; set; } = false;

        internal static MemoArchives Converter(DataRow dr)
        {
            var m = new MemoArchives
            {
                ID = dr[MemoArchivesFields.ID].ToLong(),
                MemoTypeID = dr[MemoArchivesFields.MemoTypeID].ToNullableShort(),
                MemoNo = dr[MemoArchivesFields.MemoNo].ToString(),
                File = dr[MemoArchivesFields.File].ToString(),
                MemoDate = dr[MemoArchivesFields.MemoDate].ToNullableDateTime(),
                Description = dr[MemoArchivesFields.Description].ToString(),
                Subject = dr[MemoArchivesFields.Subject].ToString(),
                InReplyTo = dr[MemoArchivesFields.InReplyTo].ToNullableLong(),
                PersonnelReply = dr[MemoArchivesFields.PersonnelReply].ToNullableBoolean(),
                CreatedOn = dr[MemoArchivesFields.CreatedOn].ToDateTime(),
                SaveOnly = dr[MemoArchivesFields.SaveOnly].ToBoolean()
            };

            m._MemoType = LookupProcess.GetMemoType(m.MemoTypeID);

            m._Persons = PersonnelProcess.GetMemoArchivesPerson(m.ID, true);
            m._Groups = PersonnelGroupProcess.GetMemoArchivesGroup(m.ID, true);

            if (!String.IsNullOrEmpty(m.File))
                m.File = Path.Combine(ConfigurationManager.AppSettings["MemoFolder"], m.File);

            try
            {
                m.StatusId = dr["Status Id"].ToNullableInt() ?? 0;
                m._MemoStatus = LookupProcess.GetMemoStatus(m.StatusId);
                m.IsFailed = dr[MemoArchivesFields.IsFailed].ToBoolean();
            }
            catch { }

            if (!IsMemoOnly)
            {
                if (m.InReplyTo != null)
                {
                    m._Replies = GetReply(m.ID);
                }
            }
            return m;
        }

        public static List<MemoArchives> GetReply(long id)
        {
            var eb = new List<MemoArchives>();
            
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.ID, id }
            };

            using (var db = new DBTools())
            {
                IsMemoOnly = true;
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetReplies, Parameters))
                {
                    eb = ds.GetList(Converter);
                }
                IsMemoOnly = false;
            }

            return eb;
        }

        public static List<MemoArchives> GetAllReplies(long id)
        {
            var eb = new List<MemoArchives>();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.ID, id }
            };

            using (var db = new DBTools())
            {
                IsMemoOnly = true;
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetAllReplies, Parameters))
                {
                    eb = ds.GetList(Converter);
                }
                IsMemoOnly = false;
            }

            return eb;
        }

        public static MemoArchives Get(long Id, bool isMemoOnly = false)
        {
            var eb = new MemoArchives();

            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.Get, Parameters))
                {
                    IsMemoOnly = isMemoOnly;
                    eb = ds.Get(Converter);
                    IsMemoOnly = false;
                }
            }

            return eb;
        }

        public static MemoArchives GetAll(long Id, bool isMemoOnly = false)
        {
            var eb = new MemoArchives();

            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.ID, Id }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetAll, Parameters))
                {
                    IsMemoOnly = isMemoOnly;
                    eb = ds.Get(Converter);
                    IsMemoOnly = false;
                }
            }

            return eb;
        }

        public static List<MemoArchives> GetList(int? memoTypeID, string personnel, string group, string filter, int page, int gridCount, out int PageCount)
        {
            var emp = new List<MemoArchives>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.MemoTypeID, memoTypeID },
                { FilterParameters.Personnel, personnel },
                { FilterParameters.Group, group },
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
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.Filter, ref outParameters, Parameters))
                {
                    IsMemoOnly = true;
                    emp = ds.GetList(Converter);
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                    IsMemoOnly = false;
                }
            }

            return emp;
        }

        public static List<MemoArchives> GetPersonnelMemos(long? personnelId, int? memoTypeId)
        {
            var emp = new List<MemoArchives>();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.PersonnelID, personnelId },
                { MemoArchivesParameters.MemoTypeID, memoTypeId }
            };
            
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetPersonnelMemo, Parameters))
                {
                    IsMemoOnly = true;
                    emp = ds.GetList(Converter);
                    IsMemoOnly = false;
                }
            }

            return emp;
        }

        public static MemoArchives GetPersonnelMemo(long personnelId, long memoId, bool isMemoOnly = false)
        {
            var emp = new MemoArchives();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.PersonnelID, personnelId },
                { PersonnelMemoParameters.MemoID, memoId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetPersonnelMemo, Parameters))
                {
                    IsMemoOnly = isMemoOnly;
                    emp = ds.Get(Converter);
                    IsMemoOnly = false;
                }
            }

            return emp;
        }
        
        public static List<MemoArchives> GetPersonnelMemoReplies(long personnelId, long memoId)
        {
            var emp = new List<MemoArchives>();

            var Parameters = new Dictionary<string, object>
            {
                { PersonnelMemoParameters.PersonnelID, personnelId },
                { PersonnelMemoParameters.MemoID, memoId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(MemoArchivesProcedures.GetPersonnelMemoReplies, Parameters))
                {
                    IsMemoOnly = true;
                    emp = ds.GetList(Converter);
                    IsMemoOnly = false;
                }
            }

            return emp;
        }

        public static MemoArchives CreateOrUpdate(MemoArchives memoArchives, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.MemoNo, memoArchives.MemoNo },
                { MemoArchivesParameters.MemoTypeID, memoArchives.MemoTypeID },
                { MemoArchivesParameters.File, memoArchives.File },
                { MemoArchivesParameters.InReplyTo, memoArchives.InReplyTo },
                { MemoArchivesParameters.PersonnelReply, memoArchives.PersonnelReply },
                { MemoArchivesParameters.MemoDate, memoArchives.MemoDate },
                { MemoArchivesParameters.Description, memoArchives.Description },
                { MemoArchivesParameters.Subject, memoArchives.Subject },
                { MemoArchivesParameters.SaveOnly, memoArchives.SaveOnly },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { MemoArchivesParameters.ID, SqlDbType.BigInt, memoArchives.ID},
                { MemoArchivesParameters.CreatedOn, SqlDbType.DateTime}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                memoArchives.ID = OutParameters.Get(MemoArchivesParameters.ID).ToLong();
                memoArchives.CreatedOn = OutParameters.Get(MemoArchivesParameters.CreatedOn).ToDateTime();
                memoArchives._MemoType = LookupProcess.GetMemoType(memoArchives.MemoTypeID);
            }

            return memoArchives;
        }

        public static MemoArchives CreateOrUpdate(MemoArchives memoArchives, long personnelId, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.MemoNo, memoArchives.MemoNo },
                { MemoArchivesParameters.MemoTypeID, memoArchives.MemoTypeID },
                { MemoArchivesParameters.File, memoArchives.File },
                { MemoArchivesParameters.InReplyTo, memoArchives.InReplyTo },
                { MemoArchivesParameters.PersonnelReply, memoArchives.PersonnelReply },
                { MemoArchivesParameters.MemoDate, memoArchives.MemoDate },
                { MemoArchivesParameters.Description, memoArchives.Description },
                { MemoArchivesParameters.Subject, memoArchives.Subject },
                { PersonnelMemoParameters.PersonnelID, personnelId },
                { MemoArchivesParameters.SaveOnly, memoArchives.SaveOnly },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { MemoArchivesParameters.ID, SqlDbType.BigInt, memoArchives.ID},
                { MemoArchivesParameters.CreatedOn, SqlDbType.DateTime}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                memoArchives.ID = OutParameters.Get(MemoArchivesParameters.ID).ToLong();
                memoArchives.CreatedOn = OutParameters.Get(MemoArchivesParameters.CreatedOn).ToDateTime();
                memoArchives._MemoType = LookupProcess.GetMemoType(memoArchives.MemoTypeID);
            }

            return memoArchives;
        }

        public static MemoArchives CreateOrUpdate(MemoArchives memoArchives, int personnelgroupId, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.MemoNo, memoArchives.MemoNo },
                { MemoArchivesParameters.MemoTypeID, memoArchives.MemoTypeID },
                { MemoArchivesParameters.File, memoArchives.File },
                { MemoArchivesParameters.InReplyTo, memoArchives.InReplyTo },
                { MemoArchivesParameters.PersonnelReply, memoArchives.PersonnelReply },
                { MemoArchivesParameters.MemoDate, memoArchives.MemoDate },
                { MemoArchivesParameters.Description, memoArchives.Description },
                { MemoArchivesParameters.Subject, memoArchives.Subject },
                { PersonnelGroupMemberParameters.PersonnelGroupID, personnelgroupId },
                { MemoArchivesParameters.SaveOnly, memoArchives.SaveOnly },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { MemoArchivesParameters.ID, SqlDbType.BigInt, memoArchives.ID},
                { MemoArchivesParameters.CreatedOn, SqlDbType.DateTime}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                memoArchives.ID = OutParameters.Get(MemoArchivesParameters.ID).ToLong();
                memoArchives.CreatedOn = OutParameters.Get(MemoArchivesParameters.CreatedOn).ToDateTime();
                memoArchives._MemoType = LookupProcess.GetMemoType(memoArchives.MemoTypeID);
            }

            return memoArchives;
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesFields.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.Delete, Parameters);
            }
        }

        public static void Delete(long Id, long personnelId, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesFields.ID, Id },
                { PersonnelMemoParameters.PersonnelID, personnelId },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.Delete, Parameters);
            }
        }

        public static string GetMemoNumber()
        {
            var number = "";

            using (var db = new DBTools())
            {
                number = db.ExecuteScalar(MemoArchivesProcedures.GetMemoNumber)?.ToString();
            }

            return number;
        }

        public static void CloseMemo(long memoId, int userId)
        {
            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesFields.ID, memoId },
                { CredentialParameters.LogBy, userId }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(MemoArchivesProcedures.Close, Parameters);
            }
        }
    }
}
