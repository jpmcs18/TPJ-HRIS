using ProcessLayer.Helpers;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTemplate.Controllers.CnB
{
    public class TimesheetController : Controller
    {
        // GET: Timesheet
        public ActionResult Index()
        {
            return View("_TimeSheetIndex");
        }
    }
}