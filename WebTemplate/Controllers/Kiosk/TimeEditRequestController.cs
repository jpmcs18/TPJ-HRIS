using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Text;
using System.Web.Mvc;
using WebTemplate.Models.Kiosk.Time_Edit_Request;

namespace WebTemplate.Controllers.Kiosk
{
    public class TimeEditRequestController : BaseController
    {
        // GET: TimeEditRequest
        public ActionResult Index(Index model)
        {
            model.TimeEditRequests = TimeEditRequestProcess.Instance.GetList(model.Key, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.LoginDateTime, model.LogoutDateTime, model.Page, model.GridCount, out int PageCount);
            model.Personnel = PersonnelProcess.GetByUserId(User.UserID);
            model.PageCount = PageCount;
            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MyTimeEditRequests", model);
            }
            else
            {
                return ViewCustom("_TimeEditRequestIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New()
        {
            try
            {
                Index model = new Index
                {
                    Personnel = PersonnelProcess.GetByUserId(User.UserID),
                    TimeEditRequest = new TimeEditRequest()
                };

                return PartialViewCustom("_TimeEditRequestNew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrNew(long? id, long? personnelid)
        {
            try
            {
                TimeEditRequest model = new TimeEditRequest()
                {
                    _Personnel = PersonnelProcess.Get(personnelid ?? 0)
                };
                model.PersonnelID = model._Personnel?.ID;

                if ((id ?? 0) > 0)
                    model = TimeEditRequestProcess.Instance.Get(id ?? 0);

                if (model.Approved == true || model.Cancelled == true)
                    return PartialViewCustom("_TimeEditRequestView", model);
                else
                    return PartialViewCustom("_TimeEditRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(TimeEditRequest model)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (errors.Length > 0)
                    return Json(new { msg = false, res = errors.ToString() });

                model = TimeEditRequestProcess.Instance.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_TimeEditRequestEdit", model);
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
                TimeEditRequest model = TimeEditRequestProcess.Instance.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_TimeEditRequest", model);
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
                    TimeEditRequestProcess.Instance.Approve(id ?? 0, User.UserID);
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
        public ActionResult Cancel(TimeEditRequest te)
        {
            if (te?.ID != null)
            {
                try
                {
                    TimeEditRequestProcess.Instance.Cancel(te, User.UserID);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(TimeEditRequest te)
        {
            if (te?.ID != null)
            {
                try
                {
                    TimeEditRequestProcess.Instance.Delete(te);
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