using DataAccessLayer.HR;
using System;
using System.Linq;
using ProcessLayer.Processes;

namespace DataAccessLayer.Security
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }

        public bool RememberMe { get; set; }
        public bool IsValidUsername { get; set; }
        public bool? IsAuthenticated { get; set; }
        public int UserID { get; set; }
        public int UserStatusID { get; set; }

        public bool ForcePasswordChange { get; set; }
        public string Position { get; set; }
        public string VerificationCode { get; set; }

        public ProcessLayer.Entities.Personnel Personnel { get; set; }

        public void IsUserExists()
        {
            this.IsValidUsername = false;
            using (var entity = new DataLayer.WebDBEntities())
            {
                var u = entity.Users.Where(us => us.UserName == this.Username).Select(us => new { us.FirstName, us.LastName }).FirstOrDefault();
                if (u != null)
                {
                    this.IsValidUsername = true;
                    this.FirstName = u.FirstName;
                    this.LastName = u.LastName;
                }
            }
        }

        public void _Login()
        {
            this.IsAuthenticated = false;
            this.UserStatusID = 0;
            this.UserID = 0;

            using (var entity = new DataLayer.WebDBEntities())
            {
                var u = entity.Users.Where(us => us.UserName == this.Username).FirstOrDefault();

                if (u != null)
                {
                    //null status set to active
                    this.UserStatusID = u.UserStatusID ?? 1;

                     if (this.UserStatusID == 1)
                    {
                        string saltedpassword = SecurityLib.SecurityHash.Encrypt(this.Password, u.PasswordSalt);
                        if (u.Password == saltedpassword)
                        {
                            this.IsAuthenticated = true;
                            this.UserID = u.ID;
                            this.FirstName = u.FirstName;
                            this.LastName = u.LastName;
                            this.Personnel = PersonnelProcess.Get(u.Personnel_ID ?? 0);
                            u.SignOnAttempts = 0;

                            this.ForcePasswordChange = u.ForcePasswordChange ?? false;

                        }
                        else
                        {
                            string signonattemps = entity.Parameters.Where(sp => sp.ParameterName == "Max Sign On Attemps").Select(up => up.ParameterValue).FirstOrDefault();

                            u.SignOnAttempts = (u.SignOnAttempts ?? 0) + 1;

                            if (int.TryParse(signonattemps, out int maxsignonattemps) && u.SignOnAttempts >= maxsignonattemps)
                            {
                                //lock
                                u.UserStatusID = 3;
                            }
                        }

                        entity.SaveChanges();
                    }
                }
            }
        }

        public void LoginUpdate(int userid, string sessionid, string internalip, string externalip, string macaddress)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var user = (from u in entity.Users
                            select u).FirstOrDefault();
                if (user != null)
                {
                    user.LoggedOn = true;
                    user.LastLoginDate = DateTime.Now;
                    user.LastLogoutDate = null;
                    user.LastSessionID = sessionid;
                    user.LastInternalIPAddressUsed = internalip;
                    user.LastExternalIPAddressUsed = externalip;
                    user.LastMacAddressUsed = macaddress;
                    entity.SaveChanges();
                }
            }
        }

        public void LogoutUpdate(int userid)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var user = (from u in entity.Users
                            select u).FirstOrDefault();
                if (user != null)
                {
                    user.LoggedOn = false;
                    user.LastLogoutDate = DateTime.Now;
                    entity.SaveChanges();
                }
            }
        }
    }
}
