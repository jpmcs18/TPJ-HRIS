using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Lookup.KioskApprover;

namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class KioskApproverController : BaseController
    {
        // GET: KioskApprover
        public ActionResult Index(Index model)
        {
            //model.Page = model.Page > 1 ? model.Page : 1;
            //model.KioskApprovers = KioskApproverProcess.Instance.Value.Get(model.Name, model.DepartmentID, model.Page, model.GridCount, out int PageCount);
            model.KioskApprovers = new List<KioskApprovers>();
            model.Personnels = new List<Personnel>();
            //model.PageCount = PageCount;

            //if (Request.IsAjaxRequest())
            //{
            //    ModelState.Clear();
            //    return PartialViewCustom("_KioskApproversSearch", model);
            //}
            //else
            //{
                return ViewCustom("_KioskApproversIndex", model);
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCandidateApprover(int? departmentid)
        {
            try
            {
                KioskCandidateApprover model = new KioskCandidateApprover();

                if ((departmentid ?? 0) > 0)
                    model.Personnel = PersonnelProcess.GetPersonnelEligibleToApprove(departmentid ?? 0, "");

                if (model == null)
                {
                    return PartialViewCustom("_KioskApproversNew", model);
                }
                else
                {
                    return PartialViewCustom("_KioskApproversEdit", model);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetCandidatesAndApprovers(Index model)
        {
            try
            {
                //Index model = new Index();
                model.Page = 1; //model.Page > 1 ? model.Page : 1;
                model.GridCount = 0;

                if ((model.DepartmentID ?? 0) > 0)
                {
                    model.KioskApprovers = KioskApproverProcess.Instance.Value.Get(model.Approver, model.DepartmentID);
                    model.Personnels = PersonnelProcess.GetPersonnelEligibleToApprove(model.DepartmentID, model.Personnel);
                }

                //model.PageCount = PageCount;
                return PartialViewCustom("_KioskApproversSearch", model);
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
                    KioskApprover = new KioskApprovers()
                };

                return PartialViewCustom("_KioskApproversNew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOrNewKioskApprover(int? id)
        {
            try
            {
                KioskApprovers model = new KioskApprovers();

                if ((id ?? 0) > 0)
                    model = KioskApproverProcess.Instance.Value.Get(id ?? 0);

                if (model == null)
                {
                    return PartialViewCustom("_KioskApproversNew", model);
                }
                else
                {
                    return PartialViewCustom("_KioskApproversEdit", model);
                }
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveKioskApprover(KioskApprovers model)
        {
            try
            {   
                model = KioskApproverProcess.Instance.Value.CreateOrUpdate(model, User.UserID);
                ModelState.Clear();
                return PartialViewCustom("_KioskApproversNew", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveKioskApprovers(List<KioskApprovers> model, int? deptid, string filter)
        {
            try
            {
                Index models = new Index
                {
                    KioskApprovers = KioskApproverProcess.Instance.Value.CreateOrUpdate(model, User.UserID).Where(r => r.Deleted == false).ToList(),
                };
                models.DepartmentID = deptid ?? model[0].DepartmentID ?? 0;
                models.Personnels = PersonnelProcess.GetPersonnelEligibleToApprove(models.DepartmentID, filter);

                ModelState.Clear();
                return PartialViewCustom("_KioskApproversSearch", models);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelEditing(int? id)
        {
            try
            {
                KioskApprovers model = KioskApproverProcess.Instance.Value.Get(id ?? 0);
                ModelState.Clear();
                return PartialViewCustom("_KioskApprovers", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteKioskApprover(int? id = null)
        {
            if (id.HasValue)
            {
                try
                {
                    KioskApproverProcess.Instance.Value.Delete(id.Value, User.UserID);
                }
                catch
                {
                    return Json(new { msg = false, res = "Approver not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = "Approver not found." });
            }
            return Json(new { msg = true });
        }
    }
}
