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

        // GET: LeaveRequestsDocument
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.LeaveRequests = LeaveRequestProcess.Instance.Value.GetRequestThatNeedDocument(model.Personnel, model.LeaveTypeID, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model._LeaveType = LeaveTypeProcess.Instance.Value.Get(model.LeaveTypeID);
            model.LeaveTypes = LeaveTypeProcess.Instance.Value.GetLeaveTypesThatHasDocumentNeeded();
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

    }
}