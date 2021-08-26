using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.RequestsApproval.Time_Edit_Request;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class TimeEditRequestsController : BaseController
    {
        // GET: TimeEditRequest
        public ActionResult Index(Index model)
        {
            //try
            //{
                model.Page = model.Page > 1 ? model.Page : 1;
                model.TimeEditRequests = TimeEditRequestProcess.Instance.Value.GetApprovingList(model.Personnel, model.IsPending, model.IsApproved, model.IsCancelled, model.LoginDateTime, model.LogoutDateTime, model.Page, model.GridCount, out int PageCount, User.UserID);
                model.PageCount = PageCount;

                if (Request.IsAjaxRequest())
                {
                    ModelState.Clear();
                    return PartialViewCustom("_TimeEditRequests", model);
                }
                else
                {
                    return ViewCustom("_TimeEditRequestsIndex", model);
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
                    ApproveRequestSingle(id ?? 0);
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
        public JsonResult ApproveMultiple(string ids)
        {
            if (ids.Length > 0)
            {
                try
                {
                    ToApplyAction[] toApplyAction = new JavaScriptSerializer().Deserialize<ToApplyAction[]>(ids);
                    for (int i = 0; i < toApplyAction.Count(); i++)
                    {
                        ApproveRequestSingle(toApplyAction[i].ID.ToInt());
                    }
                }
                catch (Exception e)
                {
                    return Json(new { msg = false, res = e.Message });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Leave Request not found." });
            }

            return Json(new { msg = true });
        }

        public void ApproveRequestSingle(long? id)
        {
            TimeEditRequestProcess.Instance.Value.Approve(id ?? 0, User.UserID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(TimeEditRequest te)
        {
            if (te?.ID != null)
            {
                try
                {
                    TimeEditRequestProcess.Instance.Value.Cancel(te, User.UserID);
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