using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Kiosks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebTemplate.Models.Kiosk.Outer_Port_Request;
namespace WebTemplate.Controllers.Kiosk
{
    public class OuterPortRequestController : BaseController
    {
        // GET: OuterPortRequest
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.OuterPortRequests = OuterPortRequestProcess.Instance.GetList(model.Key, model.LocationID, model.IsCancelled, model.StartDateTime, model.EndingDateTime, model.Page, model.GridCount, out int PageCount);
            model.Approver = PersonnelProcess.GetByUserId(User.UserID);
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_MyOuterPortRequests", model);
            }
            else
            {
                return ViewCustom("_OuterPortRequestIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchPersonnel(string key)
        {
            try
            {
                var model = new PersonnelList
                {
                    Personnels = PersonnelProcess.GetList(key)
                };
                ModelState.Clear();
                return PartialViewCustom("_Employees", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
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
                    Approver = PersonnelProcess.GetByUserId(User.UserID),
                    OuterPortRequest = new OuterPortRequest()
                };

                return PartialViewCustom("_OuterPortRequestNew", model);
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
                OuterPortRequest model = new OuterPortRequest()
                {
                    _Personnel = PersonnelProcess.Get(personnelid ?? 0)
                };
                model.PersonnelID = model._Personnel?.ID;

                if ((id ?? 0) > 0)
                    model = OuterPortRequestProcess.Instance.Get(id ?? 0);

                if (model.Cancelled == true)
                    return PartialViewCustom("_OuterPortRequestView", model);
                else
                    return PartialViewCustom("_OuterPortRequestEdit", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(OuterPortRequest model)
        {
            try
            {
                model = OuterPortRequestProcess.Instance.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_OuterPortRequestEdit", model);
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
                OuterPortRequest model = OuterPortRequestProcess.Instance.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_OuterPortRequest", model);
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
                    string[] toDelete = JsonConvert.DeserializeObject<string[]>(ids);
                    for (int i = 0; i < toDelete.Count(); i++)
                    {
                        DeleteRequestSingle(toDelete[i].ToInt());
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
            OuterPortRequestProcess.Instance.Delete(id ?? 0);
        }
    }
}