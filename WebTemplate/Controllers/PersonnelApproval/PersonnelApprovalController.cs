using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Web.Mvc;
using WebTemplate.Models.PersonnelApproval;

namespace WebTemplate.Controllers.PersonnelApproval
{
    public class PersonnelApprovalController : BaseController
    {
        // GET: PersonnelApproval
        public ActionResult Index(Index model)
        {
            try
            {
                model.Page = model.Page > 1 ? model.Page : 1;
                model.Personnel = PersonnelProcess.GetApprovingPersonnel(model.Filter, model.Page, model.GridCount, out int PageCount);
                model.PageCount = PageCount;

                if (Request.IsAjaxRequest())
                {
                    ModelState.Clear();
                    return PartialViewCustom("_Search", model);
                }
                else
                {
                return ViewCustom("Index", model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.GetActualMessage();
                return View("~/Views/Security/Error.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(long? id)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelProcess.Approved(id ?? 0, User.UserID);
                    return Json(new { msg = true, res = "Approved" });

                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Unable to find personnel." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(long? id)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelProcess.Cancelled(id ?? 0, User.UserID);
                    return Json(new { msg = true, res = "Cancelled" });

                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Unable to find personnel." });
        }
    }
}