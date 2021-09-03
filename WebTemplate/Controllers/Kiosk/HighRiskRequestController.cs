using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Text;
using System.Web.Mvc;
using WebTemplate.Models.Kiosk.High_Risk_Request;

namespace WebTemplate.Controllers.Kiosk
{
    public class HighRiskRequestController : BaseController
    {
        // GET: HighRiskRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Personnel = PersonnelProcess.GetByUserId(User.UserID, true);
            model.HighRiskRequests = HighRiskRequestProcess.Instance.Value.GetList(model.Personnel?.ID ?? 0, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDate, model.EndingDate, model.Page, model.GridCount, out int PageCount);
            model.HighRiskRequest = new HighRiskRequest();
            model.HighRiskRequest.RequestDate = DateTime.Now;
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MyHighRiskRequest", model);
            }
            else
            {
                return ViewCustom("_HighRiskRequestIndex", model);
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
                    Personnel = PersonnelProcess.GetByUserId(User.UserID, true),
                    HighRiskRequest = new HighRiskRequest()
                };

                return PartialViewCustom("_HighRiskRequestNew", model);
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
                HighRiskRequest model = new HighRiskRequest
                {
                    PersonnelID = User.UserID
                };

                if ((id ?? 0) > 0)
                    model = HighRiskRequestProcess.Instance.Value.Get(id ?? 0);

                if (model.Approved == true || model.Cancelled == true)
                    return PartialViewCustom("_HighRiskRequestView", model);
                else
                    return PartialViewCustom("_HighRiskRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRequest(HighRiskRequest model)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrEmpty(model.Reasons))
                    errors.Append("- <b>Reasons</b> is required<br>");
                if (model.RequestDate == null)
                    errors.Append("- <b>Request Date</b> is required<br>");

                if (errors.Length > 0)
                    return Json(new { msg = false, res = errors.ToString() });

                model = HighRiskRequestProcess.Instance.Value.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_HighRiskRequestEdit", model);
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
                HighRiskRequest model = HighRiskRequestProcess.Instance.Value.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_MyHighRiskRequest", model);
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
                    HighRiskRequestProcess.Instance.Value.Delete(id ?? 0, User.UserID);
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
    }

}