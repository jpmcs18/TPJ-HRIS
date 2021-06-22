using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.EmailLogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Processes
{
    public class EmailLogsProcess
    {
        internal static EmailLogs Converter(DataRow dr)
        {
            var m = new EmailLogs
            {
                ID = dr[EmailLogsFields.ID].ToLong(),
                Category = dr[EmailLogsFields.Category].ToString(),
                ItemID = dr[EmailLogsFields.ItemID].ToNullableLong(),
                Email = dr[EmailLogsFields.Email].ToString(),
                File = dr[EmailLogsFields.File].ToString(),
                Subject = dr[EmailLogsFields.Subject].ToString(),
                Body = dr[EmailLogsFields.Body].ToString(),
                Status = dr[EmailLogsFields.Status].ToBoolean(),
                Remarks = dr[EmailLogsFields.Remarks].ToString(),
                Resended = dr[EmailLogsFields.Resended].ToBoolean()
            };
            return m;
        }
        public static EmailLogs Insert(EmailLogs email, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { EmailLogsParameters.Category, email.Category },
                { EmailLogsParameters.ItemID, email.ItemID },
                { EmailLogsParameters.Email, email.Email },
                { EmailLogsParameters.File, email.File },
                { EmailLogsParameters.Subject, email.Subject },
                { EmailLogsParameters.Body, email.Body },
                { EmailLogsParameters.Status, email.Status },
                { EmailLogsParameters.Remarks, email.Remarks },
                { CredentialParameters.LogBy, userid }
            };

            var OutParameters = new List<OutParameters>
            {
                { EmailLogsParameters.ID, SqlDbType.BigInt, email.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(EmailLogsProcedures.Insert, ref OutParameters, Parameters);
                email.ID = OutParameters.Get(EmailLogsParameters.ID).ToLong();
            }

            return email;
        }
    }
}
