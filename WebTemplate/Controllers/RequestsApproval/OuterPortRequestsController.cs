using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Kiosk;
using ProcessLayer.Processes.Kiosks;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.RequestsApproval.Outer_Port_Request;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class OuterPortRequestsController : BaseController
    {
        // GET: OuterPortRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.OuterPortRequests = OuterPortRequestProcess.Instance.GetList(model.Personnel, 0, model.IsCancelled, model.LoginDateTime, model.LogoutDateTime, model.Page, model.GridCount, out int PageCount);
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_OuterPortRequests", model);
            }
            else
            {
                return ViewCustom("_OuterPortRequestsIndex", model);
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
            OuterPortRequestProcess.Instance.Approve(id ?? 0, User.UserID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(OuterPortRequest te)
        {
            if (te?.ID != null)
            {
                try
                {
                    OuterPortRequestProcess.Instance.Cancel(te, User.UserID);
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