using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Text;
using System.Web.Mvc;
using WebTemplate.Models.Kiosk.OT_Request;

namespace WebTemplate.Controllers.Kiosk
{
    public class OTRequestController : BaseController
    {
        // GET: OTRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Personnel = PersonnelProcess.GetByUserId(User.UserID, true);
            model.OTRequests = OTRequestProcess.Instance.Value.GetList(model.Personnel?.ID ?? 0, model.IsExpired, model.IsPending, model.IsApproved, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MyOTRequests", model);
            }
            else
            {
                return ViewCustom("_OTRequestIndex", model);
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
                    Personnel = PersonnelProcess.GetByUserId(User.UserID, true),
                    OTRequest = new OTRequest()
                };
                model.OTRequest.RequestDate = DateTime.Now;

                return PartialViewCustom("_OTRequestNew", model);
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
                OTRequest model = new OTRequest
                { 
                    PersonnelID = User.UserID
                };

                if ((id ?? 0) > 0)
                    model = OTRequestProcess.Instance.Value.Get(id ?? 0);

                if (model.Approved == true || model.Cancelled == true)
                    return PartialViewCustom("_OTRequestView", model);
                else 
                    return PartialViewCustom("_OTRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(OTRequest model)
        {
            try
            {
                StringBuilder errors = new StringBuilder();

                if (errors.Length > 0)
                    return Json(new { msg = false, res = errors.ToString() });

                model = OTRequestProcess.Instance.Value.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_OTRequestEdit", model);
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
                OTRequest model = OTRequestProcess.Instance.Value.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_MyOTRequest", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OTRequest ot)
        {
            if (ot?.ID != null)
            {
                try
                {
                    OTRequestProcess.Instance.Value.Delete(ot);
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