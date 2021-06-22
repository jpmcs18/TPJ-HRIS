using ProcessLayer.Entities;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.Maintenance.Lookup.ApproverPosition;

namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class ApproverPositionController : BaseController
    {
        // GET: ApproverPosition
        public ActionResult Index(Index model)
        {
            model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            model.ItemList = ProcessLayer.Processes.ApproverPositionProcess.GetPage(model.Filter, model.IsApprover, model.PageNumber, model.GridCount, out int Count);
            model.ItemCount = Count;

            if (Request.IsAjaxRequest())
            {
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("Index", model);
            }
        }

        [HttpPost]
        public ActionResult Management(int? uid)
        {
            try
            {
                Management model = new Management()
                {
                    PositionList = PositionProcess.Instance.GetList().ToList()
                };

                if (uid.HasValue)
                {
                    model.ApproverPosition = ProcessLayer.Processes.ApproverPositionProcess.GetByID(uid.Value);
                }
                else
                {
                    model.ApproverPosition = new ProcessLayer.Entities.ApproverPosition();
                }

                return PartialViewCustom("_Management", model);
            }
            catch (Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrUpdate(Position model)
        {
            try
            {
                if (model == null)
                    throw new Exception("Selected Approver By Position not found");

                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();
                    foreach (ModelState modelState in ModelState.Values)
                        foreach (ModelError error in modelState.Errors)
                            errors.Add(error.ErrorMessage);
                    throw new Exception("&middot;" + String.Join("</br>&middot;", errors));
                }

                if ((model.ID <= 0 && !ViewBag.Page.Insert) || (model.ID > 0 && !ViewBag.Page.Update))
                    throw new Exception("Unauthorized Access");

                ProcessLayer.Processes.ApproverPositionProcess.CreateOrUpdate(model, User.UserID);

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(int? uid)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                    throw new Exception("Unauthorized Access");

                if (uid.HasValue)
                    ProcessLayer.Processes.ApproverPositionProcess.Delete(uid.Value, User.UserID);
                else
                    throw new Exception("Selected Item is invalid");

                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}