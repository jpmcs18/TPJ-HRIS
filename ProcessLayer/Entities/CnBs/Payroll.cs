using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Entities.CnB
{
    public class Payroll
    {
        public long ID { get; set; }
        public long PayrollPeriodID { get; set; }
        public Personnel Personnel { get; set; }
        public string FullName { get { return Personnel.LastName + ", " + Personnel.FirstName; } }
        public decimal NOofDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal Allowance { get; set; }
        public decimal TotalAllowance { get; set; }
        public decimal RegularOTRate { get; set; }
        public decimal RegularOTAllowance { get; set; }
        public decimal SundayOTRate { get; set; }
        public decimal SundayOTAllowance { get; set; }
        public decimal HolidayRegularOTRate { get; set; }
        public decimal HolidayRegularOTAllowance { get; set; }
        public decimal HolidayExcessOTRate { get; set; }
        public decimal HolidayExcessOTAllowance { get; set; }
        public decimal NightDifferentialRate1 { get; set; }
        public decimal NightDifferentialRate2 { get; set; }
        public decimal RegularOTPay { get; set; }
        public decimal SundayOTPay { get; set; }
        public decimal HolidayOTPay { get; set; }
        public decimal HolidayExcessOTPay { get; set; }
        public decimal NightDifferentialPay { get; set; }
        public decimal RegularOTAllowancePay { get; set; }
        public decimal SundayOTAllowancePay { get; set; }
        public decimal HolidayOTAllowancePay { get; set; }
        public decimal HolidayOTExcessAllowancePay { get; set; }
        public decimal OutstandingVale { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public decimal BasicPay { get; set; }
        public decimal TotalOTPay { get; set; }
        public decimal TotalOTAllowance { get; set; }
        public decimal AdditionalPayRate { get; set; }
        public decimal AdditionalAllowanceRate { get; set; }
        public decimal HighRiskPayRate { get; set; }
        public decimal TotalHighRiskPay { get; set; }
        public decimal HighRiskAllowanceRate { get; set; }
        public decimal TotalHighRiskAllowancePay { get; set; }
        public decimal ExtensionRate { get; set; }
        public decimal TotalExtensionPay { get; set; }
        public decimal ExtensionAllowanceRate { get; set; }
        public decimal TotalExtensionAllowancePay { get; set; }
        public decimal TotalAdditionalPay { get; set; }
        public decimal TotalAdditionalAllowancePay { get; set; }
        public decimal TotalAdditionalOvertimePay { get; set; }
        public decimal TotalAdditionalOvertimeAllowancePay { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public decimal Tax { get; set; }
        public List<PayrollDetails> PayrollDetails { get; set; } = new List<PayrollDetails>();
        public List<PayrollDeductions> PayrollDeductions { get; set; } = new List<PayrollDeductions>();
        public List<LoanDeductions> LoanDeductions { get; set; } = new List<LoanDeductions>();

        public decimal PagibigFund { get { return PayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibig")).Sum(x => x.Amount) ?? 0; } }
        public decimal SalaryLoan { get { return LoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("sssloan")).Sum(x => x.Amount); } }
        public decimal SSSCalamityLoan { get { return LoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("ssscalamity")).Sum(x => x.Amount); } }
        public decimal PagibigCalamityLoan { get { return LoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibigcalamity")).Sum(x => x.Amount); } }
        public decimal PagibigLoan { get { return LoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibigloan")).Sum(x => x.Amount); } }
        public decimal SSS { get { return PayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("sss")).Sum(x => x.Amount) ?? 0; } }
        public decimal ProvidentFund { get { return PayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("provident")).Sum(x => x.Amount) ?? 0; } }
        public decimal PhilHealth { get { return PayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("philhealth")).Sum(x => x.Amount) ?? 0; } }
        public decimal Vale { get { return LoanDeductions.Where(x => x.PersonnelLoan._Loan.isPersonal ?? false).Sum(x => x.Amount); } }
        public decimal NoOfDaysPresent { get { return PayrollDetails.Where(x => x.IsPresent).Sum(x => x.RegularDay); } }
        public decimal ExtensionPresent { get { return PayrollDetails.Where(x => x.IsExtended).Count(); } }
        public decimal HighRiskPresent { get { return PayrollDetails.Where(x => x.IsHighRisk && (x.Location?.ID ?? 0) == 0).Count(); } }
        public decimal HighRiskOPPresent { get { return PayrollDetails.Where(x => x.IsHighRisk && (x.Location?.ID ?? 0) > 0).Count(); } }
        public decimal TotalHighRiskNotOP { get { return (HighRiskPresent * HighRiskPayRate).ToDecimalPlaces(2); } }
        public decimal TotalHighRiskNotOPAll { get { return (HighRiskPresent * HighRiskAllowanceRate).ToDecimalPlaces(2); } }
        public decimal TotalHighRiskOP { get { return (HighRiskOPPresent * HighRiskPayRate).ToDecimalPlaces(2); } }
        public decimal TotalHighRiskOPAll { get { return (HighRiskOPPresent * HighRiskAllowanceRate).ToDecimalPlaces(2); } }
        public string OPLocation { get { return PayrollDetails.Where(x => x.IsHighRisk && (x.Location?.ID ?? 0) > 0).Select(x => x.Location?.Description).FirstOrDefault(); } }
        //Additional Columns
        public decimal SumOfAllAllowance { get { return TotalAllowance + TotalOTAllowance; } }
        public decimal SumOfAllAdditionalPay { get { return TotalAdditionalPay + TotalAdditionalAllowancePay + TotalAdditionalOvertimePay + TotalAdditionalOvertimeAllowancePay; } }
        public bool Modified { get; set; } = false;
        public List<LeaveRequest> LeaveRequests { get; set; }
        public List<ComputedLeaveCredits> ComputedLeaveCredits { get; set; }
    }
}

