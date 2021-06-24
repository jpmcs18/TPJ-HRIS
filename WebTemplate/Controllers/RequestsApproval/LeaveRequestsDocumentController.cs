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
    public class LeaveRequestsDocumentController : BaseController
    {
        private const string SAVE_LOCATION = "LeaveSaveLocation";

        // GET: LeaveRequestsDocument
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.GetRequestThatNeedDocument(model.Personnel, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model._LeaveType = LeaveTypeProcess.GetLeaveType(model.LeaveTypeID);
            model.LeaveTypes = LeaveTypeProcess.GetLeaveTypesThatHasDocumentNeeded();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_LeaveRequestsDocument", model);
            }
            else
            {
                return ViewCustom("_LeaveRequestsDocumentIndex", model);
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

                    LeaveRequestProcess.Instance.UploadDocument(leaveRequestId, file, User.UserID);
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
    }
}