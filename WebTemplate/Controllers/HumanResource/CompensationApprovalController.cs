using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using System;
using System.Linq;
using System.Web.Mvc;
using C = WebTemplate.Models.CompensationApproval;

namespace WebTemplate.Controllers.HumanResource
{
    public class CompensationApprovalController : BaseController
    {
        public ActionResult Index(C.Index model)
        {
            
            model.Page = model.Page > 1 ? model.Page : 1;
            model.PersonnelCompensations = PersonnelCompensationProcess.Instance.FilterCompensationToApprove(model.Filter, model.Page, model.GridCount, out int PageCount).ToList();
            model.PageCount = PageCount;

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_Search", model);
            }
            else
            {
                return ViewCustom("_Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(long? id)
        {
            if(id.HasValue)
            {
                try {
                    PersonnelCompensationProcess.Instance.Approve(id ?? 0, User.UserID);
                    return Json(new { msg = true });
                }
                catch(Exception ex) {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Invalid Request" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disapprove(long? id, string remarks)
        {
            if (id.HasValue)
            {
                try
                {
                    PersonnelCompensationProcess.Instance.Disapprove(id ?? 0, remarks, User.UserID);
                    return Json(new { msg = true });
                }
                catch (Exception ex)
                {
                    return Json(new { msg = false, res = ex.GetActualMessage() });
                }
            }
            else
                return Json(new { msg = false, res = "Invalid Request" });
        }
    }
}