//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataLayer
{
    using System;
    using System.Collections.Generic;
    
    public partial class PageAccess
    {
        public int RoleID { get; set; }
        public int PageID { get; set; }
        public Nullable<bool> EnableView { get; set; }
        public Nullable<bool> EnableInsert { get; set; }
        public Nullable<bool> EnableUpdate { get; set; }
        public Nullable<bool> EnableDelete { get; set; }
        public Nullable<bool> AllAccess { get; set; }
        public Nullable<bool> Approvable { get; set; }
        public Nullable<int> RoleApprover { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdateDate { get; set; }
    
        public virtual Page Page { get; set; }
        public virtual Role Role { get; set; }
    }
}