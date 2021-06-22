using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Processes.Reports;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Web.Mvc;
using M = WebTemplate.Models.Report;

namespace WebTemplate.Controllers.HumanResource
{
    public class ReportController : BaseController
    {
        public ActionResult Index(M.Index model)
        {
            model.Personnels = ReportProcess.Instance.GetList(model.ReportType, model.Year, model.Month);

            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_ReportSearch", model);
            }
            else
            {
                return ViewCustom("ReportIndex", model);
            }
        }

        [HttpPost]
        public ActionResult PrintReport(ReportType reportType, int year, int month)
        {
            try
            {
                using (var report = new PrintReport(Server.MapPath(PrintReportHelper.Instance.Template)))
                {
                    report.ReportType = reportType;
                    report.Month = month;
                    report.Year = year;
                    report.GenerateReport();
                    ViewBag.Content = report.SaveToPDF();
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