using DataAccessLayer.Organization;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Organization.Department;

namespace WebTemplate.Controllers.Maintenance.Organization
{
    public class DepartmentController : BaseController
    {
        public ActionResult Index(Index model)
        {
            var department = new Department();
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = department.GetList(model.PageNumber, model.GridCount, model.Filter, out int count);
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
                Department = uid > 0 ? new Department(uid.Value) : new Department()
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

                if (model.Department.IsExist())
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

                model.Department.Save(GetCredentials());

                return Json(new { success = true, id = model.Department.ID });
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
                    var department = new Department(uid.Value);
                    if (did > 0)
                    {
                        department.Delete(GetCredentials(), did.Value);
                    }
                    else
                    {
                        department.Delete();
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