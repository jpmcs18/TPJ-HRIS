using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Lookup.PositionSalary;

namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class PositionSalaryController : BaseController
    {
        // GET: PositionSalary
        public ActionResult Index(Index model)
        {
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = ProcessLayer.Processes.PositionSalaryProcess.GetPage(model.Filter, model.PageNumber, model.GridCount, out int Count);
            model.ItemCount = Count;

            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        [HttpPost]
        public ActionResult Management(int? uid)
        {
            try
            {
                Management model = new Management()
                {
                    PositionList = PositionProcess.Instance.Value.GetList().ToList()
                };

                if (uid.HasValue)
                {
                    model.PositionSalary = PositionSalaryProcess.GetByID(uid.Value);
                }
                else
                {
                    model.PositionSalary = new ProcessLayer.Entities.PositionSalary();
                }

                return PartialViewCustom("_Management", model);
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(Management model)
        {
            try
            {
                if (model.PositionSalary == null)
                    throw new Exception("Selected Position Salary not found");

                if (!ModelState.IsValid)
                {
                    var errors = new System.Collections.Generic.List<string>();
                    foreach (ModelState modelState in ModelState.Values)
                        foreach (ModelError error in modelState.Errors)
                            errors.Add(error.ErrorMessage);
                    throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
                }

                if ((model.PositionSalary.ID <= 0 && !ViewBag.Page.Insert) || (model.PositionSalary.ID > 0 && !ViewBag.Page.Update))
                    throw new Exception("Unauthorized Access");

                ProcessLayer.Processes.PositionSalaryProcess.CreateOrUpdate(model.PositionSalary, User.UserID);

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(int? uid)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                    throw new Exception("Unauthorized Access");

                if (uid.HasValue)
                    ProcessLayer.Processes.PositionSalaryProcess.Delete(uid.Value, User.UserID);
                else
                    throw new Exception("Selected Item is invalid");

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}