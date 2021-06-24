using System;
using System.Web.Mvc;
using DataAccessLayer.System;
using WebTemplate.Models.Maintenance.Systems.Page;

namespace WebTemplate.Controllers.Maintenance.Systems
{
    public class PageController : BaseController
    {
        public ActionResult Index(Index model, int? uid)
        {
            ViewBag.uid = uid;

            var page = new Page();

            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = page.GetList(model.PageNumber, model.GridCount, model.Filter, out int count, Properties.Settings.Default.ApplicationName);
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
                _Page = uid > 0 ? new Page(uid.Value) : new Page() { Application = Properties.Settings.Default.ApplicationName }
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

                if (model._Page.IsExist())
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
                
                model._Page.Save(GetCredentials());
                
                return Json(new { success = true, id = model._Page.ID });
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
                    var page = new Page(uid.Value);
                    page._Delete();
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