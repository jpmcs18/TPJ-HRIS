using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Security
{
    public class CredentialRoles
    {
        public Credentials Credentials { get; set; }
        public List<int> Roles { get; set; }

        public CredentialRoles(Credentials credentials)
        {
            this.Credentials = credentials;
            using (var entity = new DataLayer.WebDBEntities())
            {
                this.Roles = (from u in entity.Users
                            join ur in entity.UserRoles on u.ID equals ur.UserID
                            join r in entity.Roles on ur.RoleID equals r.ID
                            where (ur.Deleted == null || ur.Deleted == false)
                            && (r.Deleted == null || r.Deleted == false)
                            && u.ID == this.Credentials.UserID
                            select r.ID).ToList();
            }
        }
    }
}
