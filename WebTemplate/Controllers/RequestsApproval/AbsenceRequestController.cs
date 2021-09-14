using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Web.Mvc;
using WebTemplate.Models.RequestsApproval.Absence;

namespace WebTemplate.Controllers.RequestsApproval
{
    public class AbsenceRequestController : BaseController
    {
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.AbsenceRequests = AbsenceRequestProcess.Instance.Value.GetApprovingList(model.Personnel, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount, User.UserID);
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_AbsenceRequests", model);
            }
            else
            {
                return ViewCustom("_AbsenceRequestsIndex", model);
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
                    AbsenceRequestProcess.Instance.Value.Approved(id ?? 0, User.UserID);
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
        public ActionResult Cancel(AbsenceRequest absence)
        {
            if (absence?.ID != null)
            {
                try
                {
                    AbsenceRequestProcess.Instance.Value.Cancel(absence, User.UserID);
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