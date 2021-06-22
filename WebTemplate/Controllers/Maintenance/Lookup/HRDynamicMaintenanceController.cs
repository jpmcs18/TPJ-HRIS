using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTemplate.Controllers.Maintenance.Lookup
{
    public class HRDynamicMaintenanceController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return ViewCustom(ProcessLayer.Processes.LookupProcess._GetLookupTables());
        }

        [HttpPost]
        public ActionResult GetList(Models.Maintenance.Lookup.HRDynamicMaintenance.Index model)
        {
            try
            {
                model.PageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
                model.ItemList = ProcessLayer.Processes.LookupProcess._GetLookupPage(model.TableName, model.Filter, model.PageNumber, model.GridCount, out int Count);
                model.ItemCount = Count;
                return PartialViewCustom("_Search", model);
            }
            catch(Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Management(string tablename, int? uid)
        {
            try
            {
                Models.Maintenance.Lookup.HRDynamicMaintenance.Management model = new Models.Maintenance.Lookup.HRDynamicMaintenance.Management()
                {
                    TableName = tablename
                };

                if (uid.HasValue)
                {
                    model.Lookup = ProcessLayer.Processes.LookupProcess._GetLookupByID(tablename, uid.Value);
                }
                else
                {
                    model.Lookup = new ProcessLayer.Entities.Lookup();
                }
                
                return PartialViewCustom("_Management", model);
            }
            catch(Exception ex)
            {
                return Json(new { ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CreateOrUpdate(string tablename, ProcessLayer.Entities.Lookup lookup)
        {
            try
            {
                if ((lookup.ID <= 0 && !ViewBag.Page.Insert) || (lookup.ID > 0 && !ViewBag.Page.Update))
                    throw new Exception("Unauthorized Access");

                ProcessLayer.Processes.LookupProcess._CreateOrUpdateLookup(tablename, lookup, User.UserID);
                return Json(new { IsSuccess = true });
            }
            catch(Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPost]
        public ActionResult Delete(string tablename, int? uid)
        {
            try
            {
                if (!ViewBag.Page.Delete)
                    throw new Exception("Unauthorized Access");

                if (uid.HasValue)
                    ProcessLayer.Processes.LookupProcess._DeleteLookup(tablename, uid.Value, User.UserID);
                else
                    throw new Exception("Selected Item ID is invalid");
                
                return Json(new { IsSuccess = true });
            }
            catch(Exception ex)
            {
                return Json(new { IsSuccess = false, ErrorMsg = ex.InnerException?.InnerException?.Message ?? ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}