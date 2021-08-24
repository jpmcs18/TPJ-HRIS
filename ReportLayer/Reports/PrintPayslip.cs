using GemBox.Spreadsheet;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ReportLayer.Bases;
using ReportLayer.Extensions;
using ReportLayer.Helpers;
using System;
using System.Linq;

namespace ReportLayer.Reports
{
    public sealed class PrintPayslip : SpreadSheetReportBase
    {
        public PrintPayslip(string template) : base(template)
        {
        }
        public PayrollPeriod PayrollPeriod { get; set; }

        public override void GenerateReport()
        {
            base.GenerateReport();

            int cnt = 0;
            foreach (var payroll in PayrollPeriod.Payrolls)
            {
                DuplicateSheet((cnt++).ToString());
                WriteDetails(payroll);
            }
            DeleteSheet();
        }

        private void WriteDetails(Payroll payroll)
        {
            int addRow = 0;
            WriteToCell(PrintPayslipHelper.Instance.EmployeeNameCell, payroll.Personnel.FullName);
            WriteToCell(PrintPayslipHelper.Instance.EmployeeNumberCell,  payroll.Personnel.EmployeeNo);
            WriteToCell(PrintPayslipHelper.Instance.TINNoCell, "TIN: " + payroll.Personnel.TIN);
            WriteToCell(PrintPayslipHelper.Instance.HDMFNoCell, "HDMF No: " + payroll.Personnel.PAGIBIG);
            WriteToCell(PrintPayslipHelper.Instance.SSSNoCell, "SSS No: " + payroll.Personnel.SSS);

            WriteToCell(PrintPayslipHelper.Instance.RegOTHoursCell, payroll.PayrollDetails.Sum(x => x.RegularOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.RegOTRateCell, payroll.RegularOTRate);
            WriteToCell(PrintPayslipHelper.Instance.RegOTPayCell, payroll.RegularOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.SundayOTHoursCell, payroll.PayrollDetails.Sum(x => x.SundayOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.SundayOTRateCell, payroll.SundayOTRate);
            WriteToCell(PrintPayslipHelper.Instance.SundayOTPayCell, payroll.SundayOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.HolidayRegDayCell, payroll.PayrollDetails.Sum(x => x.HolidayOTDays).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.HolidayRegRateCell, payroll.HolidayRegularOTRate);
            WriteToCell(PrintPayslipHelper.Instance.HolidayRegPayCell, payroll.HolidayOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.HolidayExcessOTHoursCell, payroll.PayrollDetails.Sum(x => x.HolidayExcessOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.HolidayExcessOTRateCell, payroll.HolidayExcessOTRate);
            WriteToCell(PrintPayslipHelper.Instance.HolidayExcessOTPayCell, payroll.HolidayExcessOTPay.ToString("N2"));

            if (payroll.NightDifferentialPay > 0)
            {
                WriteToCell(PrintPayslipHelper.Instance.NightDiffHoursCell, "1");
                WriteToCell(PrintPayslipHelper.Instance.HolidayExcessOTRateCell, payroll.NightDifferentialPay.ToString("N3"));
                WriteToCell(PrintPayslipHelper.Instance.HolidayExcessOTPayCell, payroll.NightDifferentialPay.ToString("N2"));
            }

            WriteToCell(PrintPayslipHelper.Instance.TotalOTPayCell, payroll.TotalOTPay.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.GrossSalaryCell, payroll.GrossPay.ToString("N2"));


            WriteToCell(PrintPayslipHelper.Instance.AllRegOTHoursCell, payroll.PayrollDetails.Sum(x => x.RegularOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.AllRegOTRateCell, payroll.RegularOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.AllRegOTPayCell, payroll.RegularOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.AllSundayOTHoursCell, payroll.PayrollDetails.Sum(x => x.SundayOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.AllSundayOTRateCell, payroll.SundayOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.AllSundayOTPayCell, payroll.SundayOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.AllHolidayRegDayCell, payroll.PayrollDetails.Sum(x => x.HolidayOTDays).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.AllHolidayRegRateCell, payroll.HolidayRegularOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.AllHolidayRegPayCell, payroll.HolidayOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.AllHolidayExcessOTHoursCell, payroll.PayrollDetails.Sum(x => x.HolidayExcessOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.AllHolidayExcessOTRateCell, payroll.HolidayExcessOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.AllHolidayExcessOTPayCell, payroll.HolidayOTExcessAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.TotalAllOTPayCell, payroll.TotalOTAllowance.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.SSSCell, payroll.SSS.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.ProvidentFundCell, payroll.ProvidentFund.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.PhilHealthCell, payroll.PhilHealth.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.SalaryLoanCell, payroll.SalaryLoan.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.PagibigLoanCell, payroll.PagibigLoan.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.PagibigCell, payroll.PagibigFund.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.TaxCell, payroll.Tax.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.OtherChargesCell, payroll.Vale.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.ValeCell, payroll.OutstandingVale.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.TotalDedCell, payroll.TotalDeductions.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.NetCell, payroll.OutstandingVale < 0 ? payroll.OutstandingVale.ToString("N2") : payroll.NetPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.BasicTotalDaysCell, payroll.NOofDays);
            WriteToCell(PrintPayslipHelper.Instance.BasicSalaryCell, payroll.BasicPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.AllowanceTotalDaysCell, payroll.NOofDays);
            WriteToCell(PrintPayslipHelper.Instance.AllowanceSalaryCell, payroll.TotalAllowance.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.AddTotalPayCell, payroll.SumOfAllAdditionalPay.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.SignatoryCell, payroll.Personnel.FullName);

            int startRow = PrintPayslipHelper.Instance.LoanStartRow;
            if (payroll.SSSCalamityLoan > 0) 
            {
                InsertRowCopy(startRow, 1);
                WriteToCell(startRow, PrintPayslipHelper.Instance.PayrollPeriodColumn, "SSS Calamity Loan").SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
                WriteToCell(startRow, PrintPayslipHelper.Instance.DepartmentColumn, (payroll.SSSCalamityLoan + 1000).ToString("N2")).SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                startRow++;

            }
            if (payroll.PagibigCalamityLoan > 0)
            {
                InsertRowCopy(startRow, 1);
                WriteToCell(startRow, PrintPayslipHelper.Instance.PayrollPeriodColumn, "Pag Ibig Calamity Loan").SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
                WriteToCell(startRow, PrintPayslipHelper.Instance.DepartmentColumn, (payroll.PagibigCalamityLoan + 100).ToString("N2")).SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            }


            var allowances = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.AllowanceStartRow;

            addRow = allowances.Count() - PrintPayslipHelper.Instance.AllowanceMaxRow;
            if (addRow > 0)
            {
                InsertRow(startRow, addRow);
            }
            foreach (var allowance in allowances)
            {
                var days = allowance.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                var rate = (((allowance.FirstOrDefault().Location?.HazardRate ?? 0) + 1) * payroll.Allowance).ToDecimalPlaces(3);
                WriteToCell(startRow, PrintPayslipHelper.Instance.PayrollPeriodColumn, GetPeriod(allowance.Select(x => x.LoggedDate).First(), allowance.Select(x => x.LoggedDate).Last()));
                WriteToCell(startRow, PrintPayslipHelper.Instance.DepartmentColumn, payroll.Department)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.PositionColumn, payroll.Position)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.DaysColumn, days.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.RateColumn, rate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AmountColumn, (rate * days).ToString("N2"));
                AutofitRow(startRow);
                startRow++;
            }


            addRow = PrintPayslipHelper.Instance.AddRow;

            WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayDescColumn, "No of Days");
            WriteToCell(addRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
            WriteToCell(addRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskPayRate.ToString("N3"));
            WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskPay.ToString("N2"));
            addRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Allowance");
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskAllowanceRate.ToString("N3"));
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskAllowancePay.ToString("N2"));
                addRow++;
            }
            WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Overtime");
            WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalAdditionalOvertimePay.ToString("N2"));
            addRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Allowance");
                WriteToCell(addRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalAdditionalOvertimeAllowancePay.ToString("N2"));
            }
            var basics = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.BasicStartRow;

            addRow = basics.Count() - PrintPayslipHelper.Instance.BasicMaxRow;
            if (addRow > 0)
            {
                InsertRow(startRow, addRow);
            }
            foreach (var basic in basics)
            {
                var days = basic.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                var rate = (((basic.FirstOrDefault().Location?.HazardRate ?? 0) + 1) * payroll.DailyRate).ToDecimalPlaces(3);
                WriteToCell(startRow, PrintPayslipHelper.Instance.PayrollPeriodColumn, GetPeriod(basic.Select(x => x.LoggedDate).First(), basic.Select(x => x.LoggedDate).Last()));
                WriteToCell(startRow, PrintPayslipHelper.Instance.DepartmentColumn, payroll.Department)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.PositionColumn, payroll.Position)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.DaysColumn, days.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.RateColumn, rate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AmountColumn, (rate * days).ToString("N2"));
                AutofitRow(startRow);
                startRow++;
            }
            if(payroll.Allowance == 0)
                DeleteCells(PrintPayslipHelper.Instance.AllowanceStartRow - 2 + (addRow > 0 ? addRow : 0), 0, PrintPayslipHelper.Instance.AllowanceStartRow + 10 + (addRow > 0 ? addRow : 0), PrintPayslipHelper.Instance.AmountColumn);
        }
        private string GetPeriod(DateTime start, DateTime end)
        {
            if (start.Month == end.Month)
                return $"{start:MMM dd} - {end:dd yyyy}";
            else
                if (start.Year == end.Year)
                return $"{start:MMM dd} - {end:MMM dd yyyy}";
            else
                return $"{start:MMM dd yyyy} - {end:MMM dd yyyy}";

        }

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
