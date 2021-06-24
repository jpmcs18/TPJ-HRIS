using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Linq;
using System.Web.Mvc;
using WebTemplate.Models.CrewMovement;
using CM = WebTemplate.Models.CrewMovement;

namespace WebTemplate.Controllers.Movement
{
    public class CrewMovementController : BaseController
    {
        //GET: CrewMovement
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.CrewList = PersonnelProcess.GetList(model.Name, null, model.Page, model.GridCount, out int PageCount, isCrew: true).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_CrewMovementSearch", model);
            }
            else
            {
                return ViewCustom("_CrewMovementIndex", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewMovementManagement(CM.CrewMovement model)
        {
            try
            {
                model.Personnel = PersonnelProcess.Get(model.PersonnelID, true);
                model.CrewMovements = CrewMovementProcess.GetList(model.PersonnelID, null, model.StartingDate, model.EndingDate, true);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementManagement", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrewMovementListSearch(CM.CrewMovement model)
        {
            try
            {
                model.Personnel = PersonnelProcess.Get(model.PersonnelID, true);
                model.CrewMovements = CrewMovementProcess.GetList(model.PersonnelID, null, model.StartingDate, model.EndingDate, true);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementListSearch", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrNewCrewMovement(long? id, long? personnelid, bool isview = false, bool isLast = false, bool editPosted = false)
        {
            try
            {
                ProcessLayer.Entities.CrewMovement model = new ProcessLayer.Entities.CrewMovement();

                if ((id ?? 0) > 0)
                    model = CrewMovementProcess.Get(id ?? 0);
                else if ((personnelid ?? 0) > 0)
                    model = CrewMovementProcess.GetLastMovement(personnelid ?? 0);

                if (model == null)
                    model = new ProcessLayer.Entities.CrewMovement();


                ModelState.Clear();

                ViewBag.IsLast = isLast;
                ViewBag.EditPosted = editPosted;

                if (isview && !ViewBag.EditPosted)
                    return PartialViewCustom("_CrewMovementView", model);
                else
                    return PartialViewCustom("_CrewMovementNew", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCrewActualMovement(ActualMovement model)
        {
            try
            {
                model.CrewActualMovement = CrewMovementProcess.GetCrewActualMovement(model.PersonnelID, model.VesselID, model.StartingDate, model.EndingDate, true);

                ModelState.Clear();
                return PartialViewCustom("_CrewActualMovement", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelCrewMovement(long PersonnelID)
        {
            try
            {
                CrewMovement model = new CrewMovement
                {
                    Personnel = PersonnelProcess.Get(PersonnelID, true)
                };
                model.CrewMovements = CrewMovementProcess.GetList(PersonnelID, null, model.StartingDate, model.EndingDate, true);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementListSearch", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCrewMovement(ProcessLayer.Entities.CrewMovement model)
        {
            try
            {
                model = CrewMovementProcess.CreateOrUpdate(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementNew", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePostedCrewMovement(ProcessLayer.Entities.CrewMovement model)
        {
            try
            {
                model = CrewMovementProcess.UpdatePostedMovement(model, User.UserID);

                ModelState.Clear();
                ViewBag.EditPosted = false;
                return PartialViewCustom("_CrewMovementNew", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatusCrewMovement(ProcessLayer.Entities.CrewMovement model)
        {
            try
            {
                model = CrewMovementProcess.UpdateStatus(model, User.UserID);
                var lastMovement = CrewMovementProcess.GetLastMovement(model.PersonnelID);

                ModelState.Clear();
                ViewBag.IsLast = model.ID == lastMovement.PreviousCrewMovementID;
                return PartialViewCustom("_CrewMovementNew", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelStatusCrewMovement(ProcessLayer.Entities.CrewMovement model)
        {
            try
            {
                model = CrewMovementProcess.Cancel(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_CrewMovementNew", model);

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

        [HttpPost]
        public ActionResult PrintCrewMovementForm(long id)
        {
            try
            {
                using (var report = new PrintCrewMovementForm(Server.MapPath(PrintCrewMovementFormHelper.Instance.Template)))
                {
                    report.CrewMovement = CrewMovementProcess.Get(id);
                    report.GenerateReport();
                    ViewBag.Content = report.SaveToPDF();
                    //ViewBag.Title = $"Crew Movement | {report.CrewMovement.:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
                }
                return View("~/Views/PrintingView.cshtml");
            }
            catch (Exception ex)
            {
                //return Json(new { msg = false, res = ex.GetActualMessage() });
                return View("~/Views/Security/ServerError.cshtml", ex.GetActualMessage());
            }
        }

    }
}