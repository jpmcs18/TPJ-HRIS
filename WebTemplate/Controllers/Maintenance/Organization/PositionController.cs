using DataAccessLayer.Organization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Organization.Position;

namespace WebTemplate.Controllers.Maintenance.Organization
{
    public class PositionController : BaseController
    {
        public ActionResult Index(Index model)
        {
            var position = new Position();
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = position.GetList(model.PageNumber, model.GridCount, model.Filter, out int count);
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
                Position = uid > 0 ? new Position(uid.Value) : new Position()
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
                    var errors = new List<string>();
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
                }

                if (model.Position.IsExist())
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

                model.Position.Save(GetCredentials());

                return Json(new { success = true, id = model.Position.ID });
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
        public ActionResult Delete(int? uid,int? did)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                {
                    throw new Exception("Access Denied!");
                }

                if (uid > 0)
                {
                    var position = new Position(uid.Value);
                    if (did > 0)
                    {
                        position.Delete(GetCredentials(), did.Value);
                    }
                    else
                    {
                        position.Delete();
                    }
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