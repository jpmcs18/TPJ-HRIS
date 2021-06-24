using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Linq;
using System.Web.Mvc;
using WebTemplate.Models.VesselMovement;
using C = WebTemplate.Models.Vessel.Vessel.Index;

namespace WebTemplate.Controllers.HumanResource
{
    public class VesselsController : BaseController
    {
        public ActionResult Index(C model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Vessels = VesselProcess.Instance.Filter(model.ID, model.Code, model.Description, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_VesselsSearch", model);
            }
            else
            {
                return ViewCustom("_VesselsIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageVessel(short? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    Vessel model = VesselProcess.Instance.Get(id ?? 0);
                    ModelState.Clear();
                    return PartialViewCustom("_Vessel", model);
                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Vessel not found." });
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewVessel()
        {
            try
            {
                Vessel model = new Vessel();
                ModelState.Clear();
                return PartialViewCustom("_Vessel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelVessel(short? id)
        {
            try
            {
                Vessel model = VesselProcess.Instance.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_Vessel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveVessel(Vessel model)
        {
            try
            {
                model = VesselProcess.Instance.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_Vessel", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteVessel(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    VesselProcess.Instance.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Vessel not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Vessel not found." });
            }
            return Json(new { msg = true });
        }
    }
}