using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.Kiosk.Leave_Request;

namespace WebTemplate.Controllers.Kiosk
{
    public class LeaveRequestController : BaseController
    {
        // GET: LeaveRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.GetList(model.Key, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model.Personnel = PersonnelProcess.GetByUserId(User.UserID);
            model.LeaveTypes = LeaveTypeProcess.Instance.GetList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MyLeaveRequests", model);
            }
            else
            {
                return ViewCustom("_LeaveRequestIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequest()
        {
            try
            {
                Index model = new Index
                {
                    Personnel = PersonnelProcess.GetByUserId(User.UserID),
                    LeaveRequest = new LeaveRequest(),
                };

                model.LeaveTypes = PersonnelLeaveCreditProcess.GetLeaveWithCredits(model.Personnel?.ID ?? 0, DateTime.Now.Year);

                return PartialViewCustom("_LeaveRequestNew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRequest(long? id)
        {
            try
            {
                LeaveRequest model = new LeaveRequest
                {
                    PersonnelID = User.UserID
                };

                if ((id ?? 0) > 0)
                    model = LeaveRequestProcess.Instance.Get(id ?? 0);

                if (model.Approved == true || model.Cancelled == true)
                    return PartialViewCustom("_LeaveRequestView", model);
                else
                    return PartialViewCustom("_LeaveRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRequest(LeaveRequest model)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(model.Reasons))
                    errors.Append("- <b>Reasons</b> is required<br>");

                if (errors.Length > 0)
                    return Json(new { msg = false, res = errors.ToString() });
                model._Personnel = PersonnelProcess.Get(model.PersonnelID ?? 0, true);
                model = LeaveRequestProcess.Instance.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_LeaveRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelEditing(long? id)
        {
            try
            {
                LeaveRequest model = LeaveRequestProcess.Instance.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_LeaveRequest", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long? id)
        {
            if (id != null)
            {
                try
                {
                    DeleteRequestSingle(id);
                    return Json(new { msg = true, res = "Delete Success" });
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
        public JsonResult DeleteMultiple(string ids)
        {
            if (ids.Length > 0)
            {
                try
                {
                    ToApplyAction[] toApplyAction = new JavaScriptSerializer().Deserialize<ToApplyAction[]>(ids);
                    for (int i = 0; i < toApplyAction.Count(); i++)
                    {
                        DeleteRequestSingle(toApplyAction[i].ID.ToInt());
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

        public void DeleteRequestSingle(long? id)
        {
            LeaveRequestProcess.Instance.Delete(id ?? 0, User.UserID);
        }
    }
}