using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Security
{
    public class SMSVerification
    {
        public int? UserID { get; set; }
        public string Code { get; set; }
        public string MobileNumber { get; set; }
        public int? CreatedBy { get; set; }

        private string GenerateCode()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void CreateAndSend(bool verified, string numberprefix = null)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var user = (from u in entity.Users
                            where u.UserStatusID != 4
                            && u.ID == this.UserID
                            select u).FirstOrDefault();

                if (user != null && (user.isVerifiedMobileNumber == true || !verified))
                {
                    this.MobileNumber = user.MobileNumber;
                    string code = GenerateCode();
                    entity.VerificationCodes.Where(v => v.UserID == user.ID && v.Status == 1).ToList().ForEach(v => v.Status = 3);
                    entity.VerificationCodes.Add(new DataLayer.VerificationCode
                    {
                        UserID = user.ID,
                        Code = code,
                        Status = 1,
                        CreatedBy = this.CreatedBy,
                        CreatedDate = DateTime.Now
                    });
                    entity.SaveChanges();
                    try
                    {
                        entity.sp_SendSMSMessage(String.Format("{0}{1}",numberprefix, user.MobileNumber), code);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public bool Verify(double expirationminutes)
        {
            bool verified = false;
            using (var entity = new DataLayer.WebDBEntities())
            {
                var user = entity.Users.Where(u => u.ID == this.UserID).FirstOrDefault();
                if (user != null)
                {
                    this.MobileNumber = user.MobileNumber;

                    var vcode = entity.VerificationCodes.Where(v => v.UserID == user.ID && v.Code == this.Code && v.Status == 1).FirstOrDefault();
                    if (vcode != null)
                    {
                        bool expired = (vcode.CreatedDate.HasValue && (DateTime.Now - vcode.CreatedDate).Value.TotalMinutes >= expirationminutes);
                        vcode.Status = (byte)(expired ? 3 : 2);
                        vcode.ModifiedBy = user.ID;
                        vcode.ModifiedDate = DateTime.Now;
                        user.isVerifiedMobileNumber = true;
                        entity.SaveChanges();
                        verified = !expired;
                    }
                }
            }
            return verified;
        }
    }
}
