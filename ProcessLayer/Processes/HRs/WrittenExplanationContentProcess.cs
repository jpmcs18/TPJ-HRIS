using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;

namespace ProcessLayer.Processes.HR
{
    public sealed class WrittenExplanationContentProcess
    {
        public static readonly WrittenExplanationContentProcess Instance = new WrittenExplanationContentProcess();
        private WrittenExplanationContentProcess() { }
        internal WrittenExplanationContent Converter(DataRow dr)
        {
            WrittenExplanationContent WrittenExplanationContent = new WrittenExplanationContent
            {
                ID = dr["ID"].ToLong(),
                WrittenExplanationID = dr["Written Explanation ID"].ToLong(),
                Subject = dr["Subject"].ToString(),
                Message = dr["Message"].ToString(),
                File = dr["File"].ToString(),
                SaveOnly = dr["Save Only"].ToNullableBoolean(),
                FromPersonnel = dr["From Personnel"].ToNullableBoolean() ?? false,
                Date = dr["Created On"].ToDateTime()
            };

            if (!string.IsNullOrEmpty(WrittenExplanationContent.File))
                WrittenExplanationContent.FilePath = Path.Combine(ConfigurationManager.AppSettings["MemoFolder"], WrittenExplanationContent.File);

            return WrittenExplanationContent;
        }

        public WrittenExplanationContent Get(long id)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetWrittenExplanationContent", Parameters))
                {
                    return ds.Get(Converter);
                }
            }

        }

        public List<WrittenExplanationContent> GetList(string ids)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@IDs", ids }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetWrittenExplanationContent", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }

        }

        public List<WrittenExplanationContent> GetList(long WrittenExplanationId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@WrittenExplanationID", WrittenExplanationId }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetWrittenExplanationContent", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public WrittenExplanationContent Create(WrittenExplanationContent content, int userId)
        {
            using (var db = new DBTools())
            {
                return Create(db, content, userId);
            }
        }

        public WrittenExplanationContent Create(DBTools db, WrittenExplanationContent content, int userId)
        {

            List<OutParameters> outParameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.Int }
            };

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@WrittenExplanationID", content.WrittenExplanationID },
                { "@Subject", content.Subject },
                { "@Message", content.Message },
                { "@File", content.File },
                { "@SaveOnly", content.SaveOnly },
                { "@FromPersonnel", content.FromPersonnel },
                { "@LogBy", userId },
            };

            db.ExecuteNonQuery("hr.CreateWrittenExplanationContent", ref outParameters, parameters);
            content.ID = outParameters.Get("@ID").ToLong();

            return content;
        }

        public void UpdateFile(long id, string file, int userId)
        {

            using (var db = new DBTools())
            {

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ID", id },
                    { "@File", file },
                    { "@LogBy", userId },
                };

                db.ExecuteNonQuery("hr.CreateOrUpdateWrittenExplanationContent", parameters);
            }
        }

        public void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { "@ID", Id },
                { "@Delete", true },
                { "@LogBy", userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("hr.CreateOrUpdateWrittenExplanationContent", Parameters);
            }
        }
    }
}
