using System;
using System.Web.Mvc;
using DataAccessLayer.System;
using WebTemplate.Models.Maintenance.Systems.Roles;

namespace WebTemplate.Controllers.Maintenance.Systems
{
    public class RolesController : BaseController
    {
        public ActionResult Index(Index model, int? uid)
        {
            ViewBag.uid = uid;

            var role = new Role();

            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = role.GetList(model.PageNumber, model.GridCount, model.Filter, out int count);
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
                Role = uid > 0 ? new Role(uid.Value) : new Role()
            };

            return PartialViewCustom("_Management", model);
        }

        public ActionResult AddPage(Management model)
        {
            model.Role.AddPage(model.SelectedPageAccess);
            return PartialViewCustom("_Pages", model);
        }
        public ActionResult RemovePage(Management model)
        {
            model.Role.RemovePage(model.SelectedPageAccess);
            return PartialViewCustom("_Pages", model);
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

                if (model.Role.IsExist())
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
                
                model.Role.Save(GetCredentials());
                
                return Json(new { success = true, id = model.Role.ID });
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
                    var role = new Role(uid.Value);
                    role.Delete();
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