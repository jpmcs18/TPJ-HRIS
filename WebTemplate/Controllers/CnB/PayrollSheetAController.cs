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
using WebTemplate.Models.Payroll;

namespace WebTemplate.Controllers.CnB
{
    public class PayrollSheetAController : BaseController
    {
        // GET: Payroll
        public ActionResult Index(Index model)
        {
            model.Page = model.Page > 1 ? model.Page : 1;
            model.StartDate = model.StartDate ?? DateTime.Now.AddMonths(-1);
            model.EndDate = model.EndDate ?? DateTime.Now;
            model.Payrolls = PayrollProcess.Instance.GetPayrollBases(model.StartDate, model.EndDate, PayrollSheet.A, model.Page, model.GridCount, out int PageCount);
            model.PageCount = PageCount;
            if (Request.IsAjaxRequest())
            {
                ModelState.Clear();
                return PartialViewCustom("_PayrollSheetASearch", model);
            }
            else
            {
                return ViewCustom("_PayrollSheetAIndex", model);
            }
        }
        

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GeneratePayroll(PayrollPeriod model)
        //{
        //    try
        //    {
        //        model = PayrollProcess.Instance.GeneratePayroll(model, PayrollSheet.A, User.UserID);

        //        ModelState.Clear();

        //        if (model.Payrolls.Count() > 0)
        //            return PartialViewCustom("_PayrollSheetA", model);
        //        else
        //            return Json(new { msg = false, res = "No payrolls generated!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { msg = false, res = ex.GetActualMessage() });
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GeneratePayroll(int month, int year)
        {
            try
            {
                //PayrollPeriod model = PayrollProcess.Instance.GeneratePayroll(month, year, 0, PayrollSheet.A, User.UserID);

                //ModelState.Clear();

                //if (model.Payrolls.Count() > 0)
                //    return PartialViewCustom("_PayrollSheetA", model);
                //else
                //    return Json(new { msg = false, res = "No payrolls generated!" });
                return Json(new { msg = false, res = "Under Maintenance!" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPayroll(PayrollManagement model)
        {
            try
            {
                model.Payrolls = PayrollProcess.Instance.GetPayrollList(model.PayrollBase.ID);

                ModelState.Clear();
                return PartialViewCustom("_PayrollSheetAs", model);
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
                model.PayrollDetails = PayrollProcess.Instance.GetPayrollDetails(model.Payroll?.ID ?? 0);
                model.PayrollDeductions = PayrollProcess.Instance.GetPayrollDeductions(model.Payroll?.ID ?? 0);
                model.LoanDeductions = PayrollProcess.Instance.GetLoanDeductions(model.Payroll?.ID ?? 0);
                model.Payroll.Personnel = PersonnelProcess.Get(model.Payroll.Personnel.ID, true);

                if (model.PayrollDetails.Any())
                {
                    var fd = model.PayrollDetails.FirstOrDefault().LoggedDate;
                    var ld = model.PayrollDetails.OrderByDescending(x => x.LoggedDate).FirstOrDefault().LoggedDate.AddMonths(1);

                    model.StartDate = new DateTime(fd.Year, fd.Month, 1);
                    model.EndDate = (new DateTime(ld.Year, ld.Month, 1)).AddDays(-1);
                }

                ModelState.Clear();
                return PartialViewCustom("_PayrollSheetADetails", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePayrollStatus(Payroll model)
        {
            try
            {
                model = PayrollProcess.Instance.UpdatePayrollStatus(model, User.UserID);

                ModelState.Clear();
                return PartialViewCustom("_PayrollSheetA", model);
            }
            catch (Exception ex)
            {
                return Json(new { msg = false, res = ex.GetActualMessage() });
            }
        }

        [HttpPost]
        public ActionResult PrintPayroll(long id)
        {
            try
            {
                using (var report = new PrintPayrollSheetA(Server.MapPath(PrintPayrollSheetAHelper.Instance.Template)))
                {
                    report.PayrollPeriod = PayrollProcess.Instance.GetPayrollBase(id);
                    report.GenerateReport();
                    ViewBag.Content = report.SaveToPDF();
                    ViewBag.Title = $"Payroll Sheet - A | {report.PayrollPeriod.StartDate:MMMM dd yyyy} - {report.PayrollPeriod.EndDate:MMMM dd yyyy}";
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