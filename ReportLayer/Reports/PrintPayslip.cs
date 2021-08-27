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
    public class PrintPayslip : SpreadSheetReportBase
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
            WriteToCell(PrintPayslipHelper.Instance.Value.EmployeeNameCell, payroll.Personnel.FullName);
            WriteToCell(PrintPayslipHelper.Instance.Value.EmployeeNumberCell,  payroll.Personnel.EmployeeNo);
            WriteToCell(PrintPayslipHelper.Instance.Value.TINNoCell, "TIN: " + payroll.Personnel.TIN);
            WriteToCell(PrintPayslipHelper.Instance.Value.HDMFNoCell, "HDMF No: " + payroll.Personnel.PAGIBIG);
            WriteToCell(PrintPayslipHelper.Instance.Value.SSSNoCell, "SSS No: " + payroll.Personnel.SSS);

            WriteToCell(PrintPayslipHelper.Instance.Value.RegOTHoursCell, payroll.PayrollDetails.Sum(x => x.RegularOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.RegOTRateCell, payroll.RegularOTRate);
            WriteToCell(PrintPayslipHelper.Instance.Value.RegOTPayCell, payroll.RegularOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.SundayOTHoursCell, payroll.PayrollDetails.Sum(x => x.SundayOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.SundayOTRateCell, payroll.SundayOTRate);
            WriteToCell(PrintPayslipHelper.Instance.Value.SundayOTPayCell, payroll.SundayOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayRegDayCell, payroll.PayrollDetails.Sum(x => x.HolidayOTDays).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayRegRateCell, payroll.HolidayRegularOTRate);
            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayRegPayCell, payroll.HolidayOTPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayExcessOTHoursCell, payroll.PayrollDetails.Sum(x => x.HolidayExcessOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayExcessOTRateCell, payroll.HolidayExcessOTRate);
            WriteToCell(PrintPayslipHelper.Instance.Value.HolidayExcessOTPayCell, payroll.HolidayExcessOTPay.ToString("N2"));

            if (payroll.NightDifferentialPay > 0)
            {
                WriteToCell(PrintPayslipHelper.Instance.Value.NightDiffHoursCell, "1");
                WriteToCell(PrintPayslipHelper.Instance.Value.HolidayExcessOTRateCell, payroll.NightDifferentialPay.ToString("N3"));
                WriteToCell(PrintPayslipHelper.Instance.Value.HolidayExcessOTPayCell, payroll.NightDifferentialPay.ToString("N2"));
            }

            WriteToCell(PrintPayslipHelper.Instance.Value.TotalOTPayCell, payroll.TotalOTPay.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.GrossSalaryCell, payroll.GrossPay.ToString("N2"));


            WriteToCell(PrintPayslipHelper.Instance.Value.AllRegOTHoursCell, payroll.PayrollDetails.Sum(x => x.RegularOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.AllRegOTRateCell, payroll.RegularOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.Value.AllRegOTPayCell, payroll.RegularOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.AllSundayOTHoursCell, payroll.PayrollDetails.Sum(x => x.SundayOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.AllSundayOTRateCell, payroll.SundayOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.Value.AllSundayOTPayCell, payroll.SundayOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayRegDayCell, payroll.PayrollDetails.Sum(x => x.HolidayOTDays).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayRegRateCell, payroll.HolidayRegularOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayRegPayCell, payroll.HolidayOTAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayExcessOTHoursCell, payroll.PayrollDetails.Sum(x => x.HolidayExcessOTHours).ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayExcessOTRateCell, payroll.HolidayExcessOTAllowance);
            WriteToCell(PrintPayslipHelper.Instance.Value.AllHolidayExcessOTPayCell, payroll.HolidayOTExcessAllowancePay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.TotalAllOTPayCell, payroll.TotalOTAllowance.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.SSSCell, payroll.SSS.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.ProvidentFundCell, payroll.ProvidentFund.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.PhilHealthCell, payroll.PhilHealth.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.SalaryLoanCell, payroll.SalaryLoan.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.PagibigLoanCell, payroll.PagibigLoan.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.PagibigCell, payroll.PagibigFund.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.TaxCell, payroll.Tax.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.OtherChargesCell, payroll.Vale.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.ValeCell, payroll.OutstandingVale.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.TotalDedCell, payroll.TotalDeductions.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.NetCell, payroll.OutstandingVale < 0 ? payroll.OutstandingVale.ToString("N2") : payroll.NetPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.BasicTotalDaysCell, payroll.NOofDays);
            WriteToCell(PrintPayslipHelper.Instance.Value.BasicSalaryCell, payroll.BasicPay.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.AllowanceTotalDaysCell, payroll.NOofDays);
            WriteToCell(PrintPayslipHelper.Instance.Value.AllowanceSalaryCell, payroll.TotalAllowance.ToString("N2"));

            WriteToCell(PrintPayslipHelper.Instance.Value.AddTotalPayCell, payroll.SumOfAllAdditionalPay.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.Value.SignatoryCell, payroll.Personnel.FullName);

            int startRow = PrintPayslipHelper.Instance.Value.LoanStartRow;
            if (payroll.SSSCalamityLoan > 0) 
            {
                InsertRowCopy(startRow, 1);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PayrollPeriodColumn, "SSS Calamity Loan").SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DepartmentColumn, (payroll.SSSCalamityLoan + 1000).ToString("N2")).SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                startRow++;

            }
            if (payroll.PagibigCalamityLoan > 0)
            {
                InsertRowCopy(startRow, 1);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PayrollPeriodColumn, "Pag Ibig Calamity Loan").SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DepartmentColumn, (payroll.PagibigCalamityLoan + 100).ToString("N2")).SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            }


            var allowances = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.Value.AllowanceStartRow;

            addRow = allowances.Count() - PrintPayslipHelper.Instance.Value.AllowanceMaxRow;
            if (addRow > 0)
            {
                InsertRow(startRow, addRow);
            }
            foreach (var allowance in allowances)
            {
                var days = allowance.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                var rate = (((allowance.FirstOrDefault().Location?.HazardRate ?? 0) + 1) * payroll.Allowance).ToDecimalPlaces(3);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PayrollPeriodColumn, GetPeriod(allowance.Select(x => x.LoggedDate).First(), allowance.Select(x => x.LoggedDate).Last()));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DepartmentColumn, payroll.Department)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PositionColumn, payroll.Position)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DaysColumn, days.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.RateColumn, rate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.AmountColumn, (rate * days).ToString("N2"));
                AutofitRow(startRow);
                startRow++;
            }


            addRow = PrintPayslipHelper.Instance.Value.AddRow;

            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayDescColumn, "No of Days");
            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddRateColumn, payroll.HighRiskPayRate.ToString("N3"));
            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayColumn, payroll.TotalHighRiskPay.ToString("N2"));
            addRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayDescColumn, "Allowance");
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddRateColumn, payroll.HighRiskAllowanceRate.ToString("N3"));
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayColumn, payroll.TotalHighRiskAllowancePay.ToString("N2"));
                addRow++;
            }
            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayDescColumn, "Overtime");
            WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayColumn, payroll.TotalAdditionalOvertimePay.ToString("N2"));
            addRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayDescColumn, "Allowance");
                WriteToCell(addRow, PrintPayslipHelper.Instance.Value.AddPayColumn, payroll.TotalAdditionalOvertimeAllowancePay.ToString("N2"));
            }
            var basics = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.Value.BasicStartRow;

            addRow = basics.Count() - PrintPayslipHelper.Instance.Value.BasicMaxRow;
            if (addRow > 0)
            {
                InsertRow(startRow, addRow);
            }
            foreach (var basic in basics)
            {
                var days = basic.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                var rate = (((basic.FirstOrDefault().Location?.HazardRate ?? 0) + 1) * payroll.DailyRate).ToDecimalPlaces(3);
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PayrollPeriodColumn, GetPeriod(basic.Select(x => x.LoggedDate).First(), basic.Select(x => x.LoggedDate).Last()));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DepartmentColumn, payroll.Department)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.PositionColumn, payroll.Position)
                .SetWrapText();
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.DaysColumn, days.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.RateColumn, rate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.Value.AmountColumn, (rate * days).ToString("N2"));
                AutofitRow(startRow);
                startRow++;
            }
            if(payroll.Allowance == 0)
                DeleteCells(PrintPayslipHelper.Instance.Value.AllowanceStartRow - 2 + (addRow > 0 ? addRow : 0), 0, PrintPayslipHelper.Instance.Value.AllowanceStartRow + 10 + (addRow > 0 ? addRow : 0), PrintPayslipHelper.Instance.Value.AmountColumn);
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
