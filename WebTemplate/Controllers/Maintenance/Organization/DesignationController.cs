using DataAccessLayer.Organization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Organization.Designation;

namespace WebTemplate.Controllers.Maintenance.Organization
{
    public class DesignationController : BaseController
    {
        public ActionResult Index(Index model)
        {
            var designation = new Designation();
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = designation.GetList(model.PageNumber, model.GridCount, model.Filter, out int count);
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
                Designation = uid > 0 ? new Designation(uid.Value) : new Designation()
            };

            return PartialViewCustom("_Management", model);
        }

        public ActionResult ReloadPosition()
        {
            var model = new Management();
            return PartialViewCustom("_SelectPosition", model);
        }

        public ActionResult ReloadDepartment()
        {
            var model = new Management();
            return PartialViewCustom("_SelectDepartment", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Management model)
        {
            try
            {
                if (model.Designation?.UserID.HasValue == true && model.Designation?.ParentUserID.HasValue == true
                    && model.Designation?.UserID == model.Designation?.ParentUserID)
                {
                    ModelState.AddModelError(nameof(model.Designation.UserID), "User and Head cannot be same");
                }

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

                if (model.Designation.IsExist())
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

                model.Designation.Save(GetCredentials());

                return Json(new { success = true, id = model.Designation.ID });
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
        public ActionResult Delete(int? uid)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                {
                    throw new Exception("Access Denied!");
                }

                if (uid > 0)
                {
                    var designation = new Designation(uid.Value);
                    designation.Delete();
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