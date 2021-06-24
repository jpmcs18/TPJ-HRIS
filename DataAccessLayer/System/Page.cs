using System;
using System.Linq;
using System.Data.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Interfaces;
using DataLayer;
using DataAccessLayer.Security;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace DataAccessLayer.System
{
    public class Page : IEntity
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("Page Name")]
        [Required(ErrorMessage = "Page Name is required")]
        public string PageName { get; set; }

        [DisplayName("URL")]
        [Required(ErrorMessage = "URL is required")]
        public string URL { get; set; }

        [DisplayName("Module")]
        [Required(ErrorMessage = "Module is required")]
        public string Module { get; set; }

        public string Remarks { get; set; }

        public bool? Deleted { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? DateCreated { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        [DisplayName("User Manual Link")]
        public string UserManualURL { get; set; }

        public bool? AccessibleByAll { get; set; }

        [DisplayName("Navigation Menu Type")]
        public int? NavMenuId { get; set; }

        [DisplayName("Page Order")]
        public int? PageOrder { get; set; }

        [DisplayName("Namespace")]
        [Required(ErrorMessage = "Namespace is required")]
        public string Namespace { get; set; }

        [DisplayName("Controller")]
        [Required(ErrorMessage = "Controller is required")]
        public string Controller { get; set; }

        [DisplayName("Action")]
        [Required(ErrorMessage = "Action is required")]
        public string Action { get; set; }

        [DisplayName("Application Name")]
        public string Application { get; set; }

        public bool View { get; set; }

        public bool Insert { get; set; }

        public bool Update { get; set; }

        public bool Delete { get; set; }

        public bool Approve { get; set; }

        public Page()
        {
        }

        public Page(Credentials credential)
        {
            this.Credential = credential;
        }

        public Page(int id, Credentials credential) : this(id)
        {
            this.Credential = credential;
        }

        public Page(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var page = (from p in entity.Pages
                            where (p.Deleted == null || p.Deleted == false)
                            && p.ID == id
                            select p).FirstOrDefault();

                if (page != null)
                {
                    this.ID = page.ID;
                    this.PageName = page.PageName;
                    this.URL = page.URL;
                    this.Module = page.Module;
                    this.Remarks = page.Remarks;
                    this.UserManualURL = page.UserManualURL;
                    this.AccessibleByAll = page.accessibleByAll;
                    this.NavMenuId = page.NavMenuId;
                    this.PageOrder = page.PageOrder;
                    this.Namespace = page.Namespace;
                    this.Controller = page.Controller;
                    this.Action = page.Action;
                    this.Application = page.Application;
                }
            }
        }
        
        public List<Page> GetList(int pagenumber, int gridcount, string filter, out int count, string application = null)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var pl = from p in entity.Pages
                         where (p.Deleted == null || p.Deleted == false)
                         select new Page
                         {
                             ID = p.ID,
                             PageName = p.PageName,
                             URL = p.URL,
                             Module = p.Module,
                             Application = p.Application
                         };

                if (!String.IsNullOrEmpty(filter))
                {
                    pl = from p in pl
                         where p.PageName.Contains(filter) || p.URL.Contains(filter)
                         select p;
                }

                if(!String.IsNullOrEmpty(application))
                {
                    pl = pl.Where(p => p.Application == null || p.Application == application);
                }

                count = pl.Count();
                return pl.OrderBy(m => m.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return entity.Pages.Any(p => (p.Deleted == null || p.Deleted == false) && p.ID == this.ID);
            }
        }

        public void Save(Credentials credentials)
        {
            this.Credential = credentials;
            Save();
        }

        public void Save()
        {
            this.NavMenuId = this.NavMenuId == 0 ? null : this.NavMenuId;

            using (var entity = new WebDBEntities())
            {
                var page = (from p in entity.Pages
                            where (p.Deleted == null || p.Deleted == false)
                            && p.ID == this.ID
                            select p).FirstOrDefault();

                if (page != null)
                {
                    page.PageName = this.PageName;
                    page.URL = this.URL;
                    page.Module = this.Module;
                    page.Remarks = this.Remarks;
                    page.UserManualURL = this.UserManualURL;
                    page.accessibleByAll = this.AccessibleByAll;
                    page.NavMenuId = this.NavMenuId;
                    page.PageOrder = this.PageOrder;
                    page.Namespace = this.Namespace;
                    page.Controller = this.Controller;
                    page.Action = this.Action;
                    page.Application = this.Application;

                    if (entity.Entry(page).State == EntityState.Modified)
                    {
                        page.UpdatedBy = Credential?.UserID;
                        page.LastUpdateDate = DateTime.Now;
                        entity.SaveChanges();
                    }
                }
                else
                {
                    page = new DataLayer.Page
                    {
                        PageName = this.PageName,
                        URL = this.URL,
                        Module = this.Module,
                        Remarks = this.Remarks,
                        UserManualURL = this.UserManualURL,
                        accessibleByAll = this.AccessibleByAll,
                        NavMenuId = this.NavMenuId,
                        PageOrder = this.PageOrder,
                        Namespace = this.Namespace,
                        Controller = this.Controller,
                        Action = this.Action,
                        Application = this.Application,
                        CreatedBy = Credential?.UserID,
                        DateCreated = DateTime.Now
                    };
                    entity.Pages.Add(page);
                    entity.SaveChanges();

                    this.ID = page.ID;
                }
            }
        }

        public void _Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var page = (from p in entity.Pages
                                where (p.Deleted == null || p.Deleted == false)
                                && p.ID == this.ID
                                select p).FirstOrDefault();
                    if (page != null)
                    {
                        entity.Pages.Remove(page);
                        if (entity.Entry(page).State == EntityState.Deleted)
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
                                    throw new Exception("This Page is in use.");
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

        public List<Page> LookupPages(string application = null)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var retval = from p in entity.Pages
                             where (p.Deleted == null || p.Deleted == false)
                             select new Page
                             {
                                 ID = p.ID,
                                 PageName = p.PageName,
                                 URL = p.URL,
                                 Application = p.Application
                             };

                if(!String.IsNullOrEmpty(application))
                {
                    retval = retval.Where(a => a.Application == null || a.Application == application);
                }

                return retval.ToList();
            }
        }

        public List<Page> GetRoutes(string application = null)
        {
            using (var entity = new WebDBEntities())
            {
                var rl = from p in entity.Pages
                         where (p.Deleted == null || p.Deleted == false)
                         && p.Namespace != null && p.Controller != null && p.Action != null
                         select new Page
                         {
                             URL = p.URL,
                             Namespace = p.Namespace,
                             Controller = p.Controller,
                             Action = p.Action,
                             Application = p.Application
                         };

                if(String.IsNullOrEmpty(application))
                {
                    rl = rl.Where(p => p.Application == null || Application == application);
                } 

                return rl.ToList();
            }
        }
    }
}
