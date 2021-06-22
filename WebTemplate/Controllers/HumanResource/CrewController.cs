using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebTemplate.Models.Groups;
using WebTemplate.Models.Searchable;
using C = WebTemplate.Models.Vessel.Crew;

namespace WebTemplate.Controllers.HumanResource
{
    public class CrewController : BaseController
    {
        public ActionResult Index(C.Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.Crews = CrewProcess.GetList(model.Name, model.VesselID, model.StartDate, model.EndDate, model.PositionID, model.DepartmentID, model.Remarks, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_CrewsSearch", model);
            }
            else
            {
                return ViewCustom("_CrewsIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchCrew(string key, bool first)
        {
            try
            {
                var model = new Search();
                model.Personnels.Personnel = first? new List<Personnel>() : PersonnelProcess.GetList(key);
                model.Key = key;

                ModelState.Clear();
                return PartialViewCustom("_SearchTable", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManageCrew(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    Crew model = CrewProcess.Get(id ?? 0);
                    ModelState.Clear();
                    return PartialViewCustom("_Crew", model);
                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Crew not found." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewCrew()
        {
            try
            {
                Crew model = new Crew();
                ModelState.Clear();
                return PartialViewCustom("_Crew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelCrew(long? id)
        {
            try
            {
                Crew model = CrewProcess.Get(id??0);
                ModelState.Clear();
                return PartialViewCustom("_Crew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCrew(Crew model)
        {
            try
            {
                model = CrewProcess.CreateOrUpdate(model, User.UserID);
                model = CrewProcess.Get(model.ID);

                ModelState.Clear();
                return PartialViewCustom("_Crew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteCrew(long? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    CrewProcess.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Crew not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Crew not found." });
            }
            return Json(new { msg = true });
        }
    }
}