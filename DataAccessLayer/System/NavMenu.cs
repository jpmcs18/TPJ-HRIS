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
    public class NavMenu : IEntity
    {
        public Credentials Credential { get; set; }

        public int ID { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Title is required")]
        public string NavTitle { get; set; }

        [DisplayName("Navigation Order")]
        public int? NavOrder { get; set; }

        public int? NavPageID { get; set; }

        [DisplayName("Navigation Parent Menu")]
        public int? NavParentMenu { get; set; }

        [DisplayName("Navigation Icon Class")]
        public string NavIconClass { get; set; }

        public List<NavMenu> ChildNavMenu { get; set; }

        public List<Page> Pages { get; set; }

        public NavMenu()
        {
        }

        public NavMenu(Credentials credential)
        {
            this.Credential = credential;
        }

        public NavMenu(int id, Credentials credential) : this(id)
        {
            this.Credential = credential;
        }

        public NavMenu(int id)
        {
            using (var entity = new WebDBEntities())
            {
                var navmenu = (from n in entity.NavMenus
                               where n.ID == id
                               select n).FirstOrDefault();

                if (navmenu != null)
                {
                    this.ID = navmenu.ID;
                    this.NavTitle = navmenu.NavTitle;
                    this.NavOrder = navmenu.NavOrder;
                    this.NavPageID = navmenu.NavPageID;
                    this.NavParentMenu = navmenu.NavParentMenu;
                    this.NavIconClass = navmenu.NavIconClass;
                }
            }
        }

        public List<NavMenu> GetList(int pagenumber, int gridcount, string filter, out int count)
        {
            using (var entity = new DataLayer.WebDBEntities())
            {
                var nl = from n in entity.NavMenus
                         select new NavMenu
                         {
                             ID = n.ID,
                             NavTitle = n.NavTitle,
                             NavParentMenu = n.NavParentMenu
                         };

                if (!String.IsNullOrEmpty(filter))
                {
                    nl = from n in nl
                         where n.NavTitle.Contains(filter)
                         select n;
                }

                count = nl.Count();
                return nl.OrderBy(m => m.ID).Skip((pagenumber - 1) * gridcount).Take(gridcount).ToList();
            }
        }

        public bool IsExist()
        {
            using (var entity = new WebDBEntities())
            {
                return entity.NavMenus.Any(n => n.ID == this.ID);
            }
        }

        public void Save(Credentials credential)
        {
            this.Credential = credential;
            Save();
        }

        public void Save()
        {
            this.NavParentMenu = this.NavParentMenu == 0 ? null : this.NavParentMenu;

            using (var entity = new WebDBEntities())
            {
                var navmenu = (from n in entity.NavMenus
                            where n.ID == this.ID
                            select n).FirstOrDefault();

                if (navmenu != null)
                {
                    navmenu.NavTitle = this.NavTitle;
                    navmenu.NavOrder = this.NavOrder;
                    navmenu.NavPageID = this.NavPageID;
                    navmenu.NavParentMenu = this.NavParentMenu;
                    navmenu.NavIconClass = this.NavIconClass;

                    if (entity.Entry(navmenu).State == EntityState.Modified)
                    {
                        entity.SaveChanges();
                    }
                }
                else
                {
                    navmenu = new DataLayer.NavMenu
                    {
                        NavTitle = this.NavTitle,
                        NavOrder = this.NavOrder,
                        NavPageID = this.NavPageID,
                        NavParentMenu = this.NavParentMenu,
                        NavIconClass = this.NavIconClass
                    };
                    entity.NavMenus.Add(navmenu);
                    entity.SaveChanges();

                    this.ID = navmenu.ID;
                }
            }
        }

        public void Delete()
        {
            using (var entity = new WebDBEntities())
            {
                try
                {
                    var navmenu = (from n in entity.NavMenus
                                   where n.ID == this.ID
                                   select n).FirstOrDefault();
                    if (navmenu != null)
                    {
                        entity.NavMenus.Remove(navmenu);
                        if (entity.Entry(navmenu).State == EntityState.Deleted)
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
                                    throw new Exception("This Navigation Menu is in use.");
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

        public List<NavMenu> LookupNavMenu(bool withdefault = false)
        {
            List<NavMenu> retval = new List<NavMenu>();
            using (var entity = new DataLayer.WebDBEntities())
            {
                retval = (from n in entity.NavMenus
                          select new NavMenu
                          {
                              ID = n.ID,
                              NavTitle = n.NavTitle
                          }).ToList();

                if (withdefault)
                {
                    retval.Insert(0, new NavMenu { ID = 0, NavTitle = "-- N/A --" });
                }
            }
            return retval;
        }

        public List<NavMenu> NavMenusByRole(CredentialPages pages)
        {
            var nm = new List<NavMenu>();
            using (var entity = new DataLayer.WebDBEntities())
            {
                //menus of pages
                var pagemenus = pages.Pages.Select(p => p.NavMenuId).Distinct().ToList();
                //get all parentmenu
                var parentid = entity.NavMenus.Where(n => n.NavParentMenu != null).Select(n => n.NavParentMenu ?? 0).Distinct();

                var nms = (from nav in entity.NavMenus
                           where pagemenus.Contains(nav.ID) || parentid.Contains(nav.ID)
                           select new NavMenu()
                           {
                               ID = nav.ID,
                               NavTitle = nav.NavTitle,
                               NavParentMenu = nav.NavParentMenu,
                               NavOrder = nav.NavOrder ?? 0,
                               NavIconClass = nav.NavIconClass
                           });

                nm = nms.OrderBy(n => n.NavOrder).ToList();
            }

            foreach (NavMenu n in nm)
            {
                n.Pages = pages.Pages.Where(p => p.NavMenuId == n.ID).OrderBy(p => p.PageOrder).ToList();
            }

            return GenerateNavMenu(nm, nm, 10);
        }

        private List<NavMenu> GenerateNavMenu(List<NavMenu> basenm, List<NavMenu> retnm, int maxdepth)
        {
            if (maxdepth == 0)
                throw new Exception("Menus are in depth");

            var delete = new List<NavMenu>();
            foreach (NavMenu n in retnm)
            {
                if (basenm.Any(p => p.NavParentMenu == n.ID))
                {
                    var cn = new List<NavMenu>();
                    cn.AddRange(basenm.Where(p => p.NavParentMenu == n.ID));
                    delete.AddRange(cn);
                    n.ChildNavMenu = GenerateNavMenu(basenm, cn, maxdepth - 1);
                }

                if ((n.ChildNavMenu == null || n.ChildNavMenu.Count == 0) && (n.Pages == null || n.Pages.Count == 0))
                {
                    delete.Add(n);
                }
            }

            foreach (NavMenu n in delete)
            {
                retnm.Remove(n);
            }

            return retnm;
        }
    }
}
