using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Web.Mvc;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class OTRequestsController : BaseController
    {
        // GET: OTRequests
        public ActionResult Index(Models.RequestsApproval.OT_Request.Index model)
        {
            //try
            //{
                model.Page = model.Page > 1 ? model.Page : 1;
                model.OTRequests = OTRequestProcess.Instance.GetApprovingList(model.Personnel, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount, User.UserID);
                model.PageCount = PageCount;

                if (Request.IsAjaxRequest())
                {
                    ModelState.Clear();
                    return PartialViewCustom("_OTRequests", model);
                }
                else
                {
                    return ViewCustom("_OTRequestsIndex", model);
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
        public ActionResult Approve(OTRequest ot)
        {

            if (ot?.ID != null)
            {
                try
                {
                    OTRequestProcess.Instance.Approve(ot, User.UserID);
                    OTRequest model = OTRequestProcess.Instance.Get(ot.ID);
                    ModelState.Clear();

                    //return PartialViewCustom("_OTRequests", model);
                    return Json(new { msg = true, res = "" });
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
        public ActionResult Cancel(OTRequest ot)
        {
            if (ot?.ID != null)
            {
                try
                {
                    OTRequestProcess.Instance.Cancel(ot, User.UserID);
                    OTRequest model = OTRequestProcess.Instance.Get(ot.ID);
                    ModelState.Clear();

                    //return PartialViewCustom("_OTRequests", model);
                    return Json(new { msg = true, res = "" });
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