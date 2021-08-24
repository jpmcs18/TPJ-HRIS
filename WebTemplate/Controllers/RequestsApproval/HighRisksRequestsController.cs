using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Web.Mvc;
using WebTemplate.Models.RequestsApproval.High_Risk;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class HighRisksRequestsController : BaseController
    {
        // GET: HighRiskRequest
        public ActionResult Index(Index model)
        {
            //try { 
                model.Page = model.Page > 1 ? model.Page : 1;
                model.HighRiskRequests = HighRiskRequestProcess.Instance.GetApprovingList(model.Personnel, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
                model.PageCount = PageCount;

                if (Request.IsAjaxRequest())
                {
                    ModelState.Clear();
                    return PartialViewCustom("_HighRiskRequests", model);
                }
                else
                {
                    return ViewCustom("_HighRiskRequestsIndex", model);
                }
            //}
            //catch (Exception ex)
            //{
            //    string msg = ex.Message.ToString();
            //    ViewBag.Message = msg ?? "You don't have the right to access this page.";
            //    return View("~/Views/Security/Unauthorized.cshtml");
            //    //return View("ServerError.cshtml", ex.GetActualMessage());
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(long? id)
        {
            if (id.HasValue)
            {
                try
                {
                    HighRiskRequestProcess.Instance.Approve(id ?? 0, User.UserID);
                    return Json(new { msg = true, res = "Request Approved!" });

                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Unable to find request." });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(HighRiskRequest Leave)
        {
            if (Leave?.ID != null)
            {
                try
                {
                    HighRiskRequestProcess.Instance.Cancel(Leave, User.UserID);
                    return Json(new { msg = true, res = "Request Cancelled!" });

                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Unable to find request." });

        }

    }
}