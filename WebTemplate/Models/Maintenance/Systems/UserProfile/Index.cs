namespace WebTemplate.Models.Maintenance.Systems.UserProfile
{
    public class Index
    {
        public DataAccessLayer.System.User User { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}