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
    public class CrewPayrollSheetAController : BaseController
    {
        // GET: Payroll
        public ActionResult Index(Index model)
        {
            model.Page = 1;
            model.StartDate ??= DateTime.Now.AddMonths(-1);
            model.EndDate ??= DateTime.Now;
            model.Payrolls = CrewPayrollProcess.Instance.GetCrewPayrollBases(model.StartDate, model.EndDate, PayrollSheet.A, model.Page, model.GridCount, out int PageCount);
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
        public ActionResult GetVessels(long payrollPeriodID)
        {
            try
            {
                CrewVesselList crewVessel = new()
                {
                    PayrollBase = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodID, true),
                    Vessels = CrewPayrollProcess.Instance.GetCrewVessel(payrollPeriodID, true)
                };

                ModelState.Clear();
                return PartialViewCustom("_Vessels", crewVessel);
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
                CrewPayrollList crewPayroll = new()
                {
                    PayrollBase = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodID, true),
                    Vessel = VesselProcess.Instance.Get(vesselID),
                    CrewPayrolls = CrewPayrollProcess.Instance.GetCrewPayrolls(payrollPeriodID, vesselID)
                };

                ModelState.Clear();
                return PartialViewCustom("_Payrolls", crewPayroll);
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

                DateTime fd = model.PayrollDetails.FirstOrDefault().LoggedDate;
                DateTime ld = model.PayrollDetails.OrderByDescending(x => x.LoggedDate).FirstOrDefault().LoggedDate.AddMonths(1);

                model.StartDate = new DateTime(fd.Year, fd.Month, 1);
                model.EndDate = (new DateTime(ld.Year, ld.Month, 1)).AddDays(-1);

                ModelState.Clear();
                return PartialViewCustom("_Details", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        public ActionResult PrintPayroll(long payrollPeriodId)
        {
            using (PrintCrewPayrollSheetB report = new(Server.MapPath(PrintCrewPayrollSheetBHelper.Instance.Template)))
            {
                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodId);

                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Crew Payroll Sheet - B | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.AdjustedEndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintVesselPayroll(long payrollPeriodId, int vesselId)
        {
            using (PrintCrewPayrollSheetB report = new(Server.MapPath(PrintCrewPayrollSheetBHelper.Instance.Template)))
            {

                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(payrollPeriodId, true);
                report.PayrollPeriod.CrewVessel = new System.Collections.Generic.List<CrewVessel> { CrewPayrollProcess.Instance.GetCrewVessel(payrollPeriodId, vesselId) };
                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Crew Payroll Sheet - B | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.AdjustedEndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintPayslip(long id, int vesselId)
        {
            using (PrintCrewPayslip report = new(Server.MapPath(PrintCrewPayslipHelper.Instance.Template)))
            {
                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(id, true);
                report.CrewVessel = CrewPayrollProcess.Instance.GetCrewVessel(id, vesselId);
                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Crew Payslip | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        public ActionResult PrintIndividualPayslip(long payrollId)
        {
            CrewPayroll payroll = CrewPayrollProcess.Instance.GetCrewPayroll(payrollId);
            CrewVessel crewVessel = new()
            {
                Vessel = payroll.Vessel,
                CrewPayrolls = new System.Collections.Generic.List<CrewPayroll>
                {
                    payroll
                }
            };
            using (PrintCrewPayslip report = new(Server.MapPath(PrintCrewPayslipHelper.Instance.Template)))
            {
                report.PayrollPeriod = CrewPayrollProcess.Instance.GetCrewPayrollBase(payroll.CrewPayrollPeriodID, true);
                report.CrewVessel = crewVessel;
                report.GenerateReport();
                ViewBag.Content = report.SaveToPDF();
                ViewBag.Title = $"Individual Crew Payslip | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.AdjustedEndDate:MMMM dd yyyy}";
            }
            return View("~/Views/PrintingView.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecomputePersonnelPayroll(long payrollId)
        {
            CrewPayroll model = CrewPayrollProcess.Instance.RecomputeCrewPayroll(payrollId, User.UserID);

            ModelState.Clear();

            return PartialViewCustom("_Payroll", model);
        }
    }
}