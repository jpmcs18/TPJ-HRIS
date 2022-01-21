using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.Kiosk.Absence_Request;

namespace WebTemplate.Controllers.Kiosk
{
    public class AbsenceRequestController : BaseController
    {
        // GET: AbsenceRequest
        public ActionResult Index(Index model)
        {
            try
            {
                model.Page = model.Page > 1 ? model.Page : 1;
                model.Personnel = PersonnelProcess.GetByUserId(User.UserID, true);
                model.AbsenceRequests = AbsenceRequestProcess.Instance.GetList(model.Personnel?.ID ?? 0, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
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
            catch (Exception ex)
            {
                ViewBag.Message = ex.GetActualMessage();
                return View("~/Views/Security/Error.cshtml");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewRequest()
        {
            try
            {
                Index model = new()
                {
                    Personnel = PersonnelProcess.GetByUserId(User.UserID, true),
                    AbsenceRequest = new AbsenceRequest(),
                };

                return PartialViewCustom("_New", model);
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
                AbsenceRequest model = new();

                if ((id ?? 0) > 0)
                    model = AbsenceRequestProcess.Instance.Get(id ?? 0);

                if (model.Approved == true || model.Cancelled == true)
                    return PartialViewCustom("_View", model);
                else
                    return PartialViewCustom("_Edit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRequest(AbsenceRequest model)
        {
            try
            {
                model._Personnel = PersonnelProcess.Get(model.PersonnelID ?? 0, true);
                model = AbsenceRequestProcess.Instance.CreateOrUpdate(model);
                ModelState.Clear();
                return PartialViewCustom("_Edit", model);
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
                AbsenceRequest model = AbsenceRequestProcess.Instance.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_AbsenceRequest", model);
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
                return Json(new { msg = false, res = "Request not found." });
            }

            return Json(new { msg = true });
        }

        public void DeleteRequestSingle(long? id)
        {
            AbsenceRequestProcess.Instance.Delete(id ?? 0);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Print(long requestId)
        {
            using (var report = new PrintAbsence(Server.MapPath(PrintAbsenceHelper.Instance.Template)))
            {
                report.AbsenceRequest = AbsenceRequestProcess.Instance.Get(requestId);

                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
            }
            return View("~/Views/PrintingView.cshtml");
        }
    }
}