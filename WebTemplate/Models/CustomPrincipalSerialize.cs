namespace WebTemplate.Models
{
    public class CustomPrincipalSerialize
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }

        public bool ForcePasswordChange { get; set; }
    }
}