using System;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer.System
{
    public class UserRole : IEntity
    {
        public int RoleID { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public void Save()
        {
        }
    }
}
