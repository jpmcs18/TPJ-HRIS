using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Linq;
using System.Web.Mvc;
using ProcessLayer.Entities;
using WebTemplate.Models.VesselMovement;
using ReportLayer.Reports;
using ReportLayer.Helpers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WebTemplate.Controllers.Movement
{
    public class VesselMovementController : BaseController
    {
        // GET: VesselMovement
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.VesselList = VesselProcess.Instance.Filter(null, model.Code, model.Description, model.Page, model.GridCount,
                out int PageCount).ToList();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VesselMovementManagement(VesselMovementList model)
        {
            try
            {
                model.Vessel = VesselProcess.Instance.Get(model.VesselID);
                model.VesselMovements =
                    VesselMovementProcess.GetList(model.VesselID, model.StartingDate, model.EndingDate); 
                //New vessel voyage addition validation

                ViewBag.AllowAdd = VesselMovementProcess.CanAddNewVesselMovement(model.VesselID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Search", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrNewVesselVoyage(long? ID, int? VesselID)
        {
            try
            {
                VesselMovement model = VesselMovementProcess.Get(ID ?? 0, true) ?? new VesselMovement();
                model.VesselID = VesselID ?? 0;
                model._Vessel = VesselProcess.Instance.Get(VesselID);
                model.VoyageStartDate = model.ID > 0 ? model.VoyageStartDate : DateTime.Now;
                model.VesselMovementCrewList = VesselMovementProcess.GetMovementCrews(model.ID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Manage", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string key)
        {
            var model = PersonnelProcess.SearchCrew(key);

            ModelState.Clear();
            return PartialViewCustom("/Voyage/_PersonnelList", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelVesselMovement(long? id)
        {
            try
            {
                VesselMovement model = VesselMovementProcess.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_Movement", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdateVesselMovement(VesselMovement model)
        {
            try
            {
                foreach (var crew in model.VesselMovementCrewList.Where(m => m.Deleted ?? false))
                {
                    VesselMovementProcess.DeleteCrew(crew.ID, User.UserID);
                }

                model = VesselMovementProcess.CreateOrUpdate(model, User.UserID);
                model = VesselMovementProcess.Get(model.ID, true) ?? new VesselMovement();

                model.VesselMovementCrewList = VesselMovementProcess.GetMovementCrews(model.ID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Manage", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteVesselMovement(int? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    DeleteVesselSingle(id ?? 0);
                }
                catch
                {
                    return Json(new {msg = false, res = "Vessel Movement not found."});
                }
            }
            else
            {
                return Json(new {msg = false, res = "Vessel Movement not found."});
            }

            return Json(new {msg = true});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckedMovement(long id)
        {
            try
            {
                VesselMovement model = VesselMovementProcess.Checked(id, User.UserID);
                model = VesselMovementProcess.Get(id, true) ?? new VesselMovement();
                model.VesselMovementCrewList = VesselMovementProcess.GetMovementCrews(model.ID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Manage", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovedMovement(long id)
        {
            try
            {
                VesselMovement model = VesselMovementProcess.Approved(id, User.UserID);
                model = VesselMovementProcess.Get(id, true) ?? new VesselMovement();
                model.VesselMovementCrewList = VesselMovementProcess.GetMovementCrews(model.ID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Manage", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
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
                        DeleteVesselSingle(toApplyAction[i].ID.ToInt());
                    }
                }
                catch (Exception e)
                {
                    return Json(new { msg = false, res = e.Message });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Personnel Group not found." });
            }

            return Json(new { msg = true });
        }

        public void DeleteVesselSingle(int? id = null)
        {
            VesselMovementProcess.Delete(id ?? 0, User.UserID);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVesselCrews(CrewList model)
        {
            try
            {
                model.StartingDate ??= DateTime.Now.AddMonths(-1);
                model.EndingDate ??= DateTime.Now;
                model.Vessel = VesselProcess.Instance.Get(model.VesselID);
                model.Crews = VesselMovementProcess.GetCrewDetailList(model.VesselID, model.StartingDate, model.EndingDate);
                ModelState.Clear();
                return PartialViewCustom("_CrewSearch", model);
            }
            catch (Exception ex)
            {
                return Json(new {msg = false, res = ex.GetActualMessage()});
            }
        }

        [HttpPost]
        public ActionResult PrintVesselMovement(VesselMovementList model)
        {
                using (var report = new PrintVesselMovement(Server.MapPath(PrintVesselMovementHelper.Instance.Template)))
                {
                    report.StartDate = model.StartingDate;
                    report.EndDate = model.EndingDate;
                    report.Vessel = VesselProcess.Instance.Get(model.VesselID);
                    report.VesselMovements = VesselMovementProcess.GetList(model.VesselID, model.StartingDate, model.EndingDate);
                    report.GenerateReport();
                    ViewBag.Content = report.SaveToPDF();
                }
                return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintCrewList(CrewList model)
        {
                using (var report = new PrintCrewlist(Server.MapPath(PrintCrewListHelper.Instance.Template)))
                {
                    report.Crews = new System.Collections.Generic.List<CrewDetails>();
                    report.StartingDate = model.StartingDate;
                    report.EndingDate = model.EndingDate;
                    report.Vessel = VesselProcess.Instance.Get(model.VesselID);
                    report.Crews = VesselMovementProcess.GetCrewDetailList(model.VesselID, model.StartingDate, model.EndingDate);
                    report.GenerateReport();
                    ViewBag.Content = report.SaveToPDF();
                }
                return View("~/Views/PrintingView.cshtml");
        }
        [HttpPost]
        public ActionResult ProceedToChecking(long id)
        {
            try
            {
                VesselMovement model = VesselMovementProcess.Checked(id, User.UserID);
                model = VesselMovementProcess.Get(id, true) ?? new VesselMovement();
                model.VesselMovementCrewList = VesselMovementProcess.GetMovementCrews(model.ID);

                ModelState.Clear();
                return PartialViewCustom("/Voyage/_Manage", model);

            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }
    }
}
