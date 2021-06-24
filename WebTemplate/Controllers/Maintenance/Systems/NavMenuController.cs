using System;
using System.Web.Mvc;
using DataAccessLayer.System;
using WebTemplate.Models.Maintenance.Systems.NavMenu;

namespace WebTemplate.Controllers.Maintenance.Systems
{
    public class NavMenuController : BaseController
    {
        public ActionResult Index(Index model)
        {
            var navmenu = new NavMenu();

            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = navmenu.GetList(model.PageNumber, model.GridCount, model.Filter, out int count);
            model.ItemCount = count;

            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        public ActionResult Management(int? uid)
        {
            var model = new Management
            {
                NavMenu = uid > 0 ? new NavMenu(uid.Value) : new NavMenu()
            };

            return PartialViewCustom("_Management", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Management model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new System.Collections.Generic.List<string>();
                    foreach (ModelState modelState in ModelState.Values)
                        foreach (ModelError error in modelState.Errors)
                            errors.Add(error.ErrorMessage);
                    throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
                }

                if (model.NavMenu.IsExist())
                {
                    if (!ViewBag.Page.Update)
                    {
                        throw new Exception("Access Denied!");
                    }
                }
                else
                {
                    if (!ViewBag.Page.Insert)
                    {
                        throw new Exception("Access Denied!");
                    }
                }
                
                model.NavMenu.Save();
                
                return Json(new { success = true, id = model.NavMenu.ID });
            }
            catch (Exception ex)
            {
                string error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                error = error.Replace("\'", "").Replace(Environment.NewLine, "");
                return Json(new { success = false, msg = error });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? uid, int? did)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                {
                    throw new Exception("Access Denied!");
                }

                if (uid > 0)
                {
                    var navmenu = new NavMenu(uid.Value);
                    navmenu.Delete();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                string error = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message;
                error = error.Replace("\'", "").Replace(Environment.NewLine, "");
                return Json(new { success = false, msg = error });
            }
        }
    }
}