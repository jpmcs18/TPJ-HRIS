using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WebTemplate.Controllers.HumanResource
{
    public class CrewMovementController : BaseController
    {
        // GET: CrewMovement
        //public ActionResult Index(Models.CrewMovement.Index model)
        //{
        //    model.Page = model.Page > 1 ? model.Page : 1;
        //    model.CrewList = CrewProcess.GetList(null, model.Code, model.Description, model.Page, model.GridCount, out int PageCount).ToList();
        //    model.PageCount = PageCount;

        //    if (Request.IsAjaxRequest())
        //    {
        //        ModelState.Clear();
        //        return PartialViewCustom("_CrewMovementSearch", model);
        //    }
        //    else
        //    {
        //        return ViewCustom("_CrewMovementIndex", model);
        //    }
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewMovementManagement(CrewMovement model)
        {
            try
            {
                //model.Crew = CrewProcess.Get(model.CrewID, true);
                //model.CrewMovements = CrewMovementProcess.GetList(model.CrewID, model.StartinDate, model.EndingDate);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementManagement", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrNewCrewMovement(long? id)
        {
            try
            {
                //ProcessLayer.Entities.CrewMovement model = new ProcessLayer.Entities.CrewMovement
                //{
                //    PersonnelID = id ?? 0,
                //    _Personnel = CrewProcess.Get(id ?? 0)
                //};

                ModelState.Clear();
                //return PartialViewCustom("_Crew", model);
                return PartialViewCustom("_Crew");
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelCrewMovement(long? id)
        {
            try
            {
                ProcessLayer.Entities.CrewMovement model = CrewMovementProcess.Get(id ?? 0);
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
        public ActionResult CreateOrUpdateCrewMovement(ProcessLayer.Entities.CrewMovement model)
        {
            try
            {
                model = CrewMovementProcess.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovement", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCrewMovement(long? id)
        {
            if (id.HasValue)
            {
                try
                {
                    CrewMovementProcess.Delete(id ?? 0, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Crew Movement not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Crew Movement not found." });
            }
            return Json(new { msg = true });
        }
    }
}