using System;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer.System
{
    public class PageAccess : IEntity
    {
        public int PageID { get; set; }

        public bool EnableView { get; set; }

        public bool EnableInsert { get; set; }

        public bool EnableUpdate { get; set; }

        public bool EnableDelete { get; set; }

        public bool AllAccess { get; set; }

        public bool Approvable { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public void Save()
        {
        }
    }
}
