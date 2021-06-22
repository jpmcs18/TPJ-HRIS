namespace WebTemplate.Models.Maintenance.Systems.User
{
    public class VerifyMobile
    {
        public int UserID { get; set; }
        public string MobileNumber { get; set; }
        public string MobileNumberPrefix { get; set; }
        public string Code { get; set; }
        public bool Resend { get; set; }
        public bool Verified { get; set; }
    }
}