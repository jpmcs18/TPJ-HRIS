using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Web.Mvc;
using WebTemplate.Models.RequestsApproval.Leave_Request;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class LeaveRequestsForNoteController : BaseController
    {
        // GET: LeaveRequestsForNote
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.Value.GetRequestThatNeedDocument(model.Personnel, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model._LeaveType = LeaveTypeProcess.Instance.Value.Get(model.LeaveTypeID);
            model.LeaveTypes = LeaveTypeProcess.Instance.Value.GetList();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Note(long? id)
        {
            if (id.HasValue)
            {
                try
                {
                    LeaveRequestProcess.Instance.Value.Note(id ?? 0, User.UserID);
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
                    LeaveRequestProcess.Instance.Value.Cancel(Leave, User.UserID);
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