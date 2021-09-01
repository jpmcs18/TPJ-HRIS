using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.RequestsApproval.Leave_Request;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class LeaveRequestsController : BaseController
    {
        private const string SAVE_LOCATION = "LeaveSaveLocation";
        // GET: LeaveRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.Value.GetApprovingList(model.Personnel, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount, User.UserID);
            model._LeaveType = LeaveTypeProcess.Instance.Value.Get(model.LeaveTypeID);
            model.LeaveTypes = LeaveTypeProcess.Instance.Value.GetList();
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
        public JsonResult UploadDocument(long leaveRequestId, HttpPostedFileBase fileBase)
        {
            try
            {
                var appSettingPath = ConfigurationManager.AppSettings[SAVE_LOCATION];
                var directory = appSettingPath.Contains("~") ? Server.MapPath(appSettingPath) : appSettingPath;

                if (fileBase != null && fileBase.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(fileBase.FileName);
                    var file = Hash(leaveRequestId.ToString()) + DateTime.Now.ToString("MMddyyyyHHmmss") + fileName;
                    var path = Path.Combine(directory, file);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    fileBase.SaveAs(path);

                    LeaveRequestProcess.Instance.Value.UploadDocument(leaveRequestId, file, User.UserID);
                }
                else
                    return Json(new { msg = false, res = "Nothing to upload" });

                return Json(new { msg = true, res = "Uploaded" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
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
                    LeaveRequestProcess.Instance.Value.Approve(id ?? 0, User.UserID);
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