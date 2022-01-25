using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Processes;
using ProcessLayer.Processes.CnB;
using ReportLayer.Helpers;
using ReportLayer.Reports;
using System;
using System.Linq;
using System.Web.Mvc;
using WebTemplate.Models.CrewPayroll;

namespace WebTemplate.Controllers.CnB
{
    public class CrewPayrollSheetBController : BaseController
    {
        // GET: Payroll
        public ActionResult Index(Index model)
        {
            model.Page = 1;
            model.StartDate = model.StartDate ?? DateTime.Now.AddMonths(-1);
            model.EndDate = model.EndDate ?? DateTime.Now;
            model.Payrolls = CrewPayrollProcess.Instance.GetCrewPayrollBases(model.StartDate, model.EndDate, PayrollSheet.B, model.Page, model.GridCount, out int PageCount);
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
        public ActionResult GeneratePayroll(int month, int year, int cutoff)
        {
            try
            {
                CrewPayrollPeriod model = CrewPayrollProcess.Instance.GenerateCrewPayroll(month, year, cutoff, PayrollSheet.B, User.UserID);

                ModelState.Clear();

                if (model.CrewVessel.Count() > 0)
                    return PartialViewCustom("_Payroll", model);
                else
                    return Json(new { msg = false, res = "No payrolls generated!" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecomputePersonnelPayroll(long payrollId)
        {
            CrewPayroll model = CrewPayrollProcess.Instance.RecomputeCrewPayroll(payrollId, User.UserID);

            ModelState.Clear();

            return PartialViewCustom("_CrewPayroll", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetVessels(long payrollPeriodID)
        {
            try
            {
                CrewVesselList crewVessel = new CrewVesselList
                {
                    PayrollBase = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodID, true),
                    Vessel = CrewPayrollProcess.Instance.GetCrewVessel(payrollPeriodID, true)
                };

                ModelState.Clear();
                return PartialViewCustom("_CrewVessels", crewVessel);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPayrolls(long payrollPeriodID, int vesselID)
        {
            try
            {
                CrewPayrollList crewPayroll = new CrewPayrollList
                {
                    PayrollBase = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodID, true),
                    Vessel = VesselProcess.Instance.Get(vesselID),
                    CrewPayrolls = CrewPayrollProcess.Instance.GetCrewPayrolls(payrollPeriodID, vesselID)
                };

                ModelState.Clear();
                return PartialViewCustom("_CrewPayroll", crewPayroll);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPayrollDetails(PayrollDetailsManagement model)
        {
            try
            {
                if ((model.Payroll?.ID ?? 0) == 0)
                {
                    return Json(new { msg = false, res = "Payroll not found." });
                }

                model.PayrollDetails = CrewPayrollProcess.Instance.GetCrewPayrollDetails(model.Payroll?.ID ?? 0);
                model.PayrollDeductions = CrewPayrollProcess.Instance.GetCrewPayrollDeductions(model.Payroll?.ID ?? 0);
                model.LoanDeductions = CrewPayrollProcess.Instance.GetCrewLoanDeductions(model.Payroll?.ID ?? 0);
                model.Payroll.Personnel = PersonnelProcess.Get(model.Payroll.Personnel.ID, true);

                var fd = model.PayrollDetails.FirstOrDefault().LoggedDate;
                var ld = model.PayrollDetails.OrderByDescending(x => x.LoggedDate).FirstOrDefault().LoggedDate.AddMonths(1);

                model.StartDate = new DateTime(fd.Year, fd.Month, 1);
                model.EndDate = (new DateTime(ld.Year, ld.Month, 1)).AddDays(-1);

                ModelState.Clear();
                return PartialViewCustom("_CrewPayrollDetails", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        public ActionResult PrintPayroll(long id)
        {
            using (var report = new CrewPrintPayrollSheetB(Server.MapPath(PrintCrewPayrollSheetBHelper.Instance.Template)))
            {
                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(id);

                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Crew Payroll Sheet - B | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintPayslip(long id, int vesselId)
        {
            using (var report = new PrintCrewPayslip(Server.MapPath(PrintPayslipHelper.Instance.Template)))
            {
                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(id, true);
                report.CrewVessel = CrewPayrollProcess.Instance.GetCrewVessel(id, vesselId);
                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Payslip | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintIndividualPayslip(long personnelId, long payPeriodid)
        {
            //using (var report = new PrintPayslip(Server.MapPath(PrintPayslipHelper.Instance.Template)))
            //{
            //    report.PayrollPeriod = PayrollProcess.Instance.GetPersonnelPayroll(personnelId, payPeriodid);

            //    report.GenerateReport();
            //    ViewBag.Content = report.SaveToPDF();
            //    ViewBag.Title = $"Individual Payslip | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
            //}
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintPayslipPerEmployee(long personnelId, int month, int year, int cutOff)
        {
            //using (var report = new PrintPayslip(Server.MapPath(PrintPayslipHelper.Instance.Template)))
            //{
            //    report.PayrollPeriod = PayrollProcess.Instance.GetPersonnelPayroll(personnelId, month, year, cutOff);

            //    report.GenerateReport();
            //    ViewBag.Content = report.SaveToPDF();
            //    ViewBag.Title = $"Individual Payslip | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
            //}
            return View("~/Views/PrintingView.cshtml");
        }
    }

}