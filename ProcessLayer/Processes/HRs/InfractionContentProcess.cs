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
    public sealed class InfractionContentProcess
    {
        public static readonly InfractionContentProcess Instance = new InfractionContentProcess();
        private InfractionContentProcess() { }
        internal InfractionContent Converter(DataRow dr)
        {
            InfractionContent infractionContent = new InfractionContent
            {
                ID = dr["ID"].ToLong(),
                InfractionID = dr["Infraction ID"].ToLong(),
                Subject = dr["Subject"].ToString(),
                Message = dr["Message"].ToString(),
                File = dr["File"].ToString(),
                SaveOnly = dr["Save Only"].ToNullableBoolean(),
                FromPersonnel = dr["From Personnel"].ToNullableBoolean() ?? false,
                Date = dr["Created On"].ToDateTime()
            };

            if (!string.IsNullOrEmpty(infractionContent.File))
                infractionContent.FilePath = Path.Combine(ConfigurationManager.AppSettings["MemoFolder"], infractionContent.File);

            return infractionContent;
        }

        public InfractionContent Get(long id)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@ID", id }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetInfractionContent", Parameters))
                {
                    return ds.Get(Converter);
                }
            }

        }
        public List<InfractionContent> GetList(string ids)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@IDs", ids }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetInfractionContent", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }

        }

        public List<InfractionContent> GetList(long infractionId)
        {
            using (var db = new DBTools())
            {

                Dictionary<string, object> Parameters = new Dictionary<string, object>
                {
                    { "@InfractionID", infractionId }
                };

                using (DataSet ds = db.ExecuteReader("hr.GetInfractionContent", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public InfractionContent Create(InfractionContent content, int userId)
        {
            using (var db = new DBTools())
            {
                return Create(db, content, userId);
            }
        }
        public InfractionContent Create(DBTools db, InfractionContent content, int userId)
        {

            List<OutParameters> outParameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.Int}
            };

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@InfractionID", content.InfractionID },
                { "@Subject", content.Subject },
                { "@Message", content.Message },
                { "@File", content.File },
                { "@SaveOnly", content.SaveOnly },
                { "@FromPersonnel", content.FromPersonnel },
                { "@LogBy", userId },
            };

            db.ExecuteNonQuery("hr.CreateOrUpdateInfractionContent", ref outParameters, parameters);
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

                db.ExecuteNonQuery("hr.CreateOrUpdateInfractionContent", parameters);
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
                db.ExecuteNonQuery("hr.CreateOrUpdateInfractionContent", Parameters);
            }
        }
    }

}
