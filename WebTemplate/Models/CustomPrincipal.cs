using System.Security.Principal;

namespace WebTemplate.Models
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? PersonnelID { get; set; }
        public string EmailAddress { get; set; }

        public bool? ForcePasswordChange { get; set; } = false;

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}