using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.EmailLogs;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes
{
    public class EmailCredentialProcess
    {
        internal static EmailCredential Converter(DataRow dr)
        {
            var m = new EmailCredential
            {
                ID = dr[EmailCredentialFields.ID].ToInt(),
                Owner = dr[EmailCredentialFields.Owner].ToString(),
                Email = dr[EmailCredentialFields.Email].ToString(),
                Password = dr[EmailCredentialFields.Password].ToByteArray()
            };

            return m;
        }

        public static EmailCredential Get(int id)
        {
            var email = new EmailCredential();

            var Parameters = new Dictionary<string, object>
            {
                { EmailCredentialParameters.ID, id}
            };

            using (var db = new DBTools())
            {
                using (var res = db.ExecuteReader(EmailCredentialProcedures.Get, Parameters))
                {
                    email = res.Get(Converter);
                }
            }
            return email;
        }

        public static EmailCredential Get(string owner)
        {
            var email = new EmailCredential();

            var Parameters = new Dictionary<string, object>
            {
                { EmailCredentialParameters.Owner, owner}
            };

            using (var db = new DBTools())
            {
                using (var res = db.ExecuteReader(EmailCredentialProcedures.Get, Parameters))
                {
                    email = res.Get(Converter);
                }
            }
            return email;
        }

        public static IEnumerable<EmailCredential> Get()
        {
            var email = new List<EmailCredential>();
            
            using (var db = new DBTools())
            {
                using (var res = db.ExecuteReader(EmailCredentialProcedures.Get))
                {
                    email = res.GetList(Converter);
                }
            }
            return email;
        }

        public static EmailCredential CreateOrUpdate(EmailCredential email)
        {
            var Parameters = new Dictionary<string, object>
            {
                { EmailCredentialParameters.Owner, email.Owner },
                { EmailCredentialParameters.Email, email.Email },
                { EmailCredentialParameters.Password, email.Password }
            };

            var OutParameters = new List<OutParameters>
            {
                { EmailCredentialParameters.ID, SqlDbType.Int, email.ID}
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(EmailCredentialProcedures.CreateOrUpdate, ref OutParameters, Parameters);
                email.ID = OutParameters.Get(EmailCredentialParameters.ID).ToInt();
            }

            return email;
        }
    }
}
