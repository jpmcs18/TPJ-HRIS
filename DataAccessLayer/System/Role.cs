using System;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Security;
using DataLayer;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace DataAccessLayer.System
{
    public class Role : IEntity
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        [DisplayName("Role Description")]
        [Required(ErrorMessage = "Role Description is required")]
        public string RoleDescription { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? CreatedBy { get; set; }

        public bool? Deleted { get; set; } = false;

        public int? UpdatedBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public int? RoleAdmin { get; set; }

        [Required(ErrorMessage = "Page Access is required")]
        public List<PageAccess> Pages { get; set; }

        public Role()
        {
        }

        public Role(Credentials credential)
        {
            this.Credential = credential;
        }

        public Role(int id, Credentials credential) : this(id)
        {
            this.Credential = credential;
        }

        public Role(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var role = (from r in entity.Roles
                            where (r.Deleted == null || r.Deleted == false)
                            && r.ID == id
                            select r).FirstOrDefault();

                if (role != null)
                {
                    this.ID = role.ID;
                    this.RoleName = role.RoleName;
                    this.RoleDescription = role.RoleDescription;

                    this.Pages = (from p in entity.PageAccesses
                                  where (p.Deleted == null || p.Deleted == false)
                                  && p.RoleID == id
                                  select new PageAccess
                                  {
                                      PageID = p.PageID,
                                      EnableView = p.EnableView ?? false,
                                      EnableInsert = p.EnableInsert ?? false,
                                      EnableUpdate = p.EnableUpdate ?? false,
                                      EnableDelete = p.EnableDelete ?? false,
                                      AllAccess = p.AllAccess ?? false,
                                      Approvable = p.Approvable ?? false
                                  }).ToList();
                }
            }
        }
        
        public List<Role> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var rl = from r in entity.Roles
                         where (r.Deleted == null || r.Deleted == false)
                         select new Role
                         {
                             ID = r.ID,
                             RoleName = r.RoleName,
                             RoleDescription = r.RoleDescription,
                             RoleAdmin = r.RoleAdmin
                         };

                if (!String.IsNullOrEmpty(filter))
                {
                    rl = from r in rl
                         where r.RoleName.Contains(filter) || r.RoleDescription.Contains(filter)
                         select r;
                }

                count = rl.Count();
                return rl.OrderBy(m => m.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return entity.Roles.Any(r => (r.Deleted == null || r.Deleted == false) && r.ID == this.ID);
            }
        }

        public void AddPage(PageAccess pa)
        {
            if (this.Pages == null)
            {
                this.Pages = new List<PageAccess>();
            }
            this.Pages.Add(pa);
        }

        public void RemovePage(PageAccess pa)
        {
            this.Pages.Remove(this.Pages.Where(r => r.PageID == pa.PageID).FirstOrDefault());
        }

        public void Save(Credentials credential)
        {
            this.Credential = credential;
            Save();
        }

        public void Save()
        {
            using (var entity = new WebDBEntities())
            {
                using (var transaction = entity.Database.BeginTransaction())
                {
                    try
                    {
                        var role = (from r in entity.Roles
                                    where (r.Deleted == null || r.Deleted == false)
                                    && r.ID == this.ID
                                    select r).FirstOrDefault();

                        if (role != null)
                        {
                            role.RoleName = this.RoleName;
                            role.RoleDescription = this.RoleDescription;

                            if (entity.Entry(role).State == EntityState.Modified)
                            {
                                role.UpdatedBy = Credential?.UserID;
                                role.LastUpdateDate = DateTime.Now;
                                entity.SaveChanges();
                            }
                        }
                        else
                        {
                            role = new DataLayer.Role
                            {
                                RoleName = this.RoleName,
                                RoleDescription = this.RoleDescription,
                                CreatedBy = Credential?.UserID,
                                DateCreated = DateTime.Now
                            };
                            entity.Roles.Add(role);
                            entity.SaveChanges();

                            this.ID = role.ID;
                        }

                        var pal = (from p in entity.PageAccesses
                                   where p.RoleID == role.ID
                                   select p).ToList();

                        foreach (PageAccess page in this.Pages)
                        {
                            var pa = pal.Where(p => p.PageID == page.PageID).FirstOrDefault();

                            if (pa != null)
                            {
                                pa.EnableView = page.EnableView;
                                pa.EnableInsert = page.EnableInsert;
                                pa.EnableUpdate = page.EnableUpdate;
                                pa.EnableDelete = page.EnableDelete;
                                pa.AllAccess = page.AllAccess;
                                pa.Approvable = page.Approvable;

                                if (entity.Entry(pa).State == EntityState.Modified)
                                {
                                    pa.UpdatedBy = Credential?.UserID;
                                    pa.LastUpdateDate = DateTime.Now;
                                    entity.SaveChanges();
                                }
                                pal.Remove(pa);
                            }
                            else
                            {
                                pa = new DataLayer.PageAccess
                                {
                                    RoleID = role.ID,
                                    PageID = page.PageID,
                                    EnableView = page.EnableView,
                                    EnableInsert = page.EnableInsert,
                                    EnableUpdate = page.EnableUpdate,
                                    EnableDelete = page.EnableDelete,
                                    AllAccess = page.AllAccess,
                                    Approvable = page.Approvable,
                                    CreatedBy = Credential?.UserID,
                                    DateCreated = DateTime.Now
                                };
                                entity.PageAccesses.Add(pa);
                                entity.SaveChanges();
                            }
                        }
                        if (pal != null && pal.Count > 0)
                        {
                            foreach (DataLayer.PageAccess pa in pal)
                            {
                                entity.PageAccesses.Remove(pa);

                                if (entity.Entry(pa).State == EntityState.Deleted)
                                {
                                    entity.SaveChanges();
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var role = (from r in entity.Roles
                                where (r.Deleted == null || r.Deleted == false)
                                && r.ID == this.ID
                                select r).FirstOrDefault();
                    if (role != null)
                    {
                        entity.Roles.Remove(role);
                        if (entity.Entry(role).State == EntityState.Deleted)
                        {
                            entity.SaveChanges();
                        }
                    }
                }
                catch (DbUpdateException e)
                {
                    if (e.GetBaseException() is SqlException sqlException)
                    {
                        if (sqlException.Errors.Count > 0)
                        {
                            switch (sqlException.Errors[0].Number)
                            {
                                case 547: // Foreign Key violation
                                    throw new Exception("This Role is in use.");
                                default:
                                    throw;
                            }
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public List<Role> LookupRoles()
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var retval = from r in entity.Roles
                             where (r.Deleted == null || r.Deleted == false)
                             select new Role
                             {
                                 ID = r.ID,
                                 RoleName = r.RoleName,
                                 RoleDescription = r.RoleDescription,
                                 RoleAdmin = r.RoleAdmin
                             };

                return retval.ToList();
            }
        }
    }
}
