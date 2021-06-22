using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Web.Mvc;
using WebTemplate.Models.RequestsApproval.Leave_Request;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class LeaveRequestsController : BaseController
    {
        // GET: LeaveRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.GetApprovingList(model.Personnel, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount, User.UserID);
            model._LeaveType = LeaveTypeProcess.GetLeaveType(model.LeaveTypeID);
            model.LeaveTypes = LeaveTypeProcess.GetLeaveType();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_LeaveRequests", model);
            }
            else
            {
                return ViewCustom("_LeaveRequestsIndex", model);
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
                    LeaveRequestProcess.Instance.Approve(id ?? 0, User.UserID);
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
        public ActionResult Cancel(LeaveRequest Leave)
        {
            if (Leave?.ID != null)
            {
                try
                {
                    LeaveRequestProcess.Instance.Cancel(Leave, User.UserID);
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