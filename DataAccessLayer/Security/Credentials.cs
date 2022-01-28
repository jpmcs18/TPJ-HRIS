using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Security
{
    public class Credentials
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public long? PersonnelID { get; set; }
        public bool? ForcePasswordChange { get; set; }

        public Credentials()
        {
        }

        public Credentials(int userid)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var c = (from u in entity.Users
                         where u.ID == userid
                         select new {
                             u.ID,
                             u.FirstName,
                             u.MiddleName,
                             u.LastName,
                             u.ForcePasswordChange,
                             u.EmailAddress,
                             u.Personnel_ID
                         }).FirstOrDefault();
                if (c != null)
                {
                    this.UserID = c.ID;
                    this.FirstName = c.FirstName;
                    this.MiddleName = c.MiddleName;
                    this.LastName = c.LastName;
                    this.ForcePasswordChange = c.ForcePasswordChange;
                    this.EmailAddress = c.EmailAddress;
                    this.PersonnelID = c.Personnel_ID;
                }
            }
        }
    }
}