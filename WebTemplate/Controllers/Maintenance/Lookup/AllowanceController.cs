using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Lookup.Allowance;

namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class AllowanceController : BaseController
    {
        //// GET: Allowance
        //public ActionResult Index()
        //{
        //    Index model = new Index()
        //    {
        //        AllowanceList = ProcessLayer.Processes.AllowanceProcess.GetWithCurrency()
        //    };
        //
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialViewCustom("_Search", model);
        //    }
        //    else
        //    {
        //        return ViewCustom("Index", model);
        //    }
        //}
        //
        //[HttpPost]
        //public ActionResult Management(int? uid)
        //{
        //    try
        //    {
        //        Management model = new Management()
        //        {
        //            CurrencyList = ProcessLayer.Processes.CurrencyProcess.GetList()
        //        };
        //
        //        if (uid.HasValue)
        //        {
        //            model.Allowance = ProcessLayer.Processes.AllowanceProcess.GetByID(uid.Value);
        //        }
        //        else
        //        {
        //            model.Allowance = new ProcessLayer.Entities.Allowance();
        //        }
        //
        //        return PartialViewCustom("_Management", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
        //    }
        //}
        //
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CreateOrUpdate(Management model)
        //{
        //    try
        //    {
        //        if (model.Allowance == null)
        //            throw new Exception("Selected Allowance not found");
        //
        //        if (!ModelState.IsValid)
        //        {
        //            var errors = new System.Collections.Generic.List<string>();
        //            foreach (ModelState modelState in ModelState.Values)
        //                foreach (ModelError error in modelState.Errors)
        //                    errors.Add(error.ErrorMessage);
        //            throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
        //        }
        //
        //        if ((model.Allowance.ID <= 0 && !ViewBag.Page.Insert) || (model.Allowance.ID > 0 && !ViewBag.Page.Update))
        //            throw new Exception("Unauthorized Access");
        //
        //        ProcessLayer.Processes.AllowanceProcess.CreateOrUpdate(model.Allowance, User.UserID);
        //
        //        return Json(new { IsSuccess = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
        //    }
        //}
        //
        //[HttpPost]
        //public ActionResult Delete(int? uid)
        //{
        //    try
        //    {
        //        if (!ViewBag.Page.Delete)
        //        {
        //            throw new Exception("Unauthorized Access");
        //        }
        //
        //        if (uid.HasValue)
        //            ProcessLayer.Processes.AllowanceProcess.Delete(uid.Value, User.UserID);
        //        else
        //            throw new Exception("Selected Item is invalid");
        //
        //        return Json(new { IsSuccess = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
        //    }
        //}
    }
}