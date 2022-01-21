using Newtonsoft.Json;
using ProcessLayer.Helpers;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models.DynamicLookup;

namespace WebTemplate.Controllers.DynamicLookup
{
    public class DynamicLookupController : BaseController
    {
        // GET: DynamicLookup
        public ActionResult Index(Index model)
        {
            if (model.LookupName == null) model.LookupName = Lookups.Position;
            var process = GetProcess(model.LookupName.Value);
            model.Page = model.Page > 1 ? model.Page : 1;
            int PageCount;
            if (LookupHelper.DynamicLookups.Contains(model.LookupName ?? Lookups.Position))
            {
                model.DynamicList = process.Filter(model.LookupName?.ToString().Replace('_', ' '), model.Key, model.Page, model.GridCount, out PageCount);
                model.PageCount = PageCount;
            }
            else
            {
                model.DynamicList = process.Filter(model.Key, model.Page, model.GridCount, out PageCount);
                model.PageCount = PageCount;
            }

            if(model.Page > model.PageCount)
            {
                model.Page = 1;
                if (LookupHelper.DynamicLookups.Contains(model.LookupName ?? Lookups.Position))
                {
                    model.DynamicList = process.Filter(model.LookupName?.ToString().Replace('_', ' '), model.Key, model.Page, model.GridCount, out PageCount);
                    model.PageCount = PageCount;
                }
                else
                {
                    model.DynamicList = process.Filter(model.Key, model.Page, model.GridCount, out PageCount);
                    model.PageCount = PageCount;
                }
            }

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
        public ActionResult Get(dynamic id,  Lookups lookup)
        {
            dynamic xid = null;
            try { xid = id[0]; } catch { }
            try
            {
                LookupData model = new LookupData
                {
                    LookupName = lookup,
                };

                var process = GetProcess(lookup);

                if (LookupHelper.DynamicLookups.Contains(lookup))
                {
                    model.Data = xid is null ? null : process.Get(lookup.ToString().Replace('_', ' '), id[0]);
                }
                else
                {
                    model.Data = xid is null ? null : process.Get(id[0]);
                }


                ModelState.Clear();
                return PartialViewCustom("_DynamicLookup", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(string item, Lookups lookup)
        {
            try
            {
                var process = GetProcess(lookup);

                if (LookupHelper.DynamicLookups.Contains(lookup))
                {
                    process.CreateOrUpdate(lookup.ToString().Replace('_', ' '), item, User.UserID);
                }
                else
                {
                    process.CreateOrUpdate(item, User.UserID);
                }

                return Json(new { msg = true, res = "Saved" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(dynamic id, Lookups lookup)
        {
            if (id != null)
            {
                try
                {
                    var process = GetProcess(lookup);

                    if (LookupHelper.DynamicLookups.Contains(lookup))
                    {
                        process.Delete(lookup.ToString().Replace('_', ' '), id[0], User.UserID); ;
                    }
                    else
                    {
                        process.Delete(id[0], User.UserID);
                    }

                    return Json(new { msg = true, res = "Delete Success" });
                }
                catch
                {
                    return Json(new { msg = false, res = $"{lookup.ToString().Replace('_', ' ')} not found." });
                }
            }
            else
            {
                return Json(new { msg = false, res = $"{lookup.ToString().Replace('_', ' ')} not found." });
            }
        }

        [NonAction] 
        public dynamic GetProcess(Lookups lookup)
        {
            switch (lookup)
            {
                case Lookups.Position:
                    return PositionProcess.Instance;
                case Lookups.Location:
                    return LocationProcess.Instance;
                case Lookups.Late_Deduction:
                    return LateDeductionProcess.Instance;
                case Lookups.Deduction:
                    return DeductionProcess.Instance;
                case Lookups.Leave_Type:
                    return LeaveTypeProcess.Instance;
                case Lookups.Payroll_Parameters:
                    return ParametersProcess.Instance;
                case Lookups.Non_Working_Days:
                    return NonWorkingDaysProcess.Instance;
                case Lookups.Schedule:
                    return ScheduleTypeProcess.Instance;
                case Lookups.Tax_Table:
                    return TaxTableProcess.Instance;
                case Lookups.PhilHealth:
                    return PhilHealthProcess.Instance;
                case Lookups.HDMF:
                    return HDMFProcess.Instance;
                case Lookups.SSS:
                    return SSSProcess.Instance;
                case Lookups.Department_Position:
                    return DepartmentPositionProcess.Instance;
                case Lookups.Vessel:
                    return VesselProcess.Instance;
                case Lookups.Non_Taxable_Days:
                    return NonTaxableDayProcess.Instance;
                case Lookups.Leave_Default_Credits:
                    return LeaveDefaultCreditsProcess.Instance;
                case Lookups.Crew_Position_Salary:
                    return CrewPositionSalaryProcess.Instance;
                default:
                    return DynamicLookupProcess.Instance;
            }
        }
    }
}