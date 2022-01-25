using GemBox.Spreadsheet;
using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ReportLayer.Bases;
using ReportLayer.Extensions;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
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
            WriteToCell(PrintPayslipHelper.Instance.EmployeeNameCell, payroll.Personnel.FullName);
            WriteToCell(PrintPayslipHelper.Instance.EmployeeNumberCell, payroll.Personnel.EmployeeNo);
            WriteToCell(PrintPayslipHelper.Instance.TINNoCell, "TIN: " + payroll.Personnel.TIN);
            WriteToCell(PrintPayslipHelper.Instance.HDMFNoCell, "HDMF No: " + payroll.Personnel.PAGIBIG);
            WriteToCell(PrintPayslipHelper.Instance.SSSNoCell, "SSS No: " + payroll.Personnel.SSS);

            WriteToCell(PrintPayslipHelper.Instance.TotalOTPayCell, payroll.TotalOTPay.ToString("N2"));
            WriteToCell(PrintPayslipHelper.Instance.GrossSalaryCell, payroll.GrossPay.ToString("N2"));

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


            List<Location> locations = payroll.PayrollDetails.Where(x => x.IsHazard && x.IsHoliday && x.HolidayOTDays > 0).Select(x => x.Location).Distinct().ToList();
            if (payroll.Allowance > 0)
            {
                startRow = PrintPayslipHelper.Instance.AllStartRow;

                WriteToCell(startRow, PrintPayslipHelper.Instance.AllDescColumn, "Regular:");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.RegularOTHours).ToString("N2"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllRateColumn, payroll.RegularOTAllowance.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllPayColumn, payroll.RegularOTAllowancePay.ToString("N2"));
                startRow++;

                WriteToCell(startRow, PrintPayslipHelper.Instance.AllDescColumn, "Sunday Overtime:");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.SundayOTHours).ToString("N2"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllRateColumn, payroll.SundayOTAllowance.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllPayColumn, payroll.SundayOTAllowancePay.ToString("N2"));
                startRow++;

                WriteToCell(startRow, PrintPayslipHelper.Instance.AllDescColumn, "Holiday (Reg):");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable && !x.IsHazard).Sum(x => x.HolidayOTDays).ToString("N2"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllRateColumn, payroll.HolidayRegularOTAllowance.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllPayColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable && !x.IsHazard).Sum(x => payroll.HolidayRegularOTAllowance * x.HolidayOTDays).ToDecimalPlaces(2).ToString("N2"));
                startRow++;

                for (int i = 0; i < locations.Count; i++)
                {
                    if ((startRow - PrintPayslipHelper.Instance.AllStartRow) >= PrintPayslipHelper.Instance.AllMaxRow)
                    {
                        InsertRowCopy(startRow, 1);
                    }

                    WriteToCell(startRow, PrintPayslipHelper.Instance.AllDescColumn, $"Holiday ({locations[i].Prefix}):");
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AllHoursColumn, payroll.PayrollDetails.Where(x => x.Location?.ID == locations[i].ID && !x.IsNonTaxable).Sum(x => x.HolidayOTDays).ToString("N2"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AllRateColumn, (payroll.HolidayRegularOTAllowance * ((locations[i].HazardRate ?? 0) + 1)).ToString("N3"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AllPayColumn, payroll.PayrollDetails.Where(x => x.Location?.ID == locations[i].ID && !x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTAllowance * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2).ToString("N2"));
                    startRow++;
                }

                if ((startRow - PrintPayslipHelper.Instance.AllStartRow) >= PrintPayslipHelper.Instance.AllMaxRow)
                {
                    InsertRowCopy(startRow, 1);
                }

                WriteToCell(startRow, PrintPayslipHelper.Instance.AllDescColumn, $"Holiday (Excess):");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.HolidayExcessOTHours).ToString("N2"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllRateColumn, payroll.HolidayExcessOTAllowance.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AllPayColumn, payroll.HolidayOTExcessAllowancePay.ToString("N2"));
                startRow++;
            }
            var allowances = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.AllowanceStartRow;

            addRow = allowances.Count() - PrintPayslipHelper.Instance.AllowanceMaxRow;
            if (addRow > 0)
            {
                InsertRowCopy(startRow, addRow);
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


            startRow = PrintPayslipHelper.Instance.AddStartRow;

            WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, "No of Days");
            WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskPayRate.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskNotOP.ToString("N2"));
            startRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Allowance");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskPresent.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskAllowanceRate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskNotOPAll.ToString("N2"));
                startRow++;
            }
            WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Overtime");
            WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalAdditionalOvertimePay.ToString("N2"));
            startRow++;
            if (payroll.Allowance > 0)
            {
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, "Allowance");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalAdditionalOvertimeAllowancePay.ToString("N2"));
                startRow++;
            }

            if (payroll.HighRiskOPPresent > 0)
            {
                if (startRow - PrintPayslipHelper.Instance.AddStartRow >= 4)
                {
                    InsertRowCopy(startRow, 1);
                }

                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, $"{payroll.OPLocation} High Risk ({(int)(PayrollParameters.Instance.HighRiskRate * 100)}%)");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskOPPresent.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskPayRate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskOP.ToString("N2"));
                startRow++;
                if (payroll.Allowance > 0)
                {
                    if (startRow - PrintPayslipHelper.Instance.AddStartRow >= 4)
                    {
                        InsertRowCopy(startRow, 1);
                    }

                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, $"{payroll.OPLocation} High Risk Allowance ({(int)(PayrollParameters.Instance.HighRiskRate * 100)}%)");
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.HighRiskOPPresent.ToString("N3"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.HighRiskAllowanceRate.ToString("N3"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalHighRiskOPAll.ToString("N2"));
                    startRow++;
                }
            }

            if (payroll.TotalExtensionPay > 0)
            {
                if (startRow - PrintPayslipHelper.Instance.AddStartRow >= 4)
                {
                    InsertRowCopy(startRow, 1);
                }

                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, $"Additional {(int)(PayrollParameters.Instance.ExtensionRate * 100)}% for Extension");
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.ExtensionPresent.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.ExtensionRate.ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalExtensionPay.ToString("N2"));
                startRow++;
                if (payroll.Allowance > 0)
                {
                    if (startRow - PrintPayslipHelper.Instance.AddStartRow >= 4)
                    {
                        InsertRowCopy(startRow, 1);
                    }

                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayDescColumn, $"Additional {(int)(PayrollParameters.Instance.ExtensionRate * 100)}% for Extension Allowance");
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddNoofDaysColumn, payroll.ExtensionPresent.ToString("N3"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddRateColumn, payroll.ExtensionAllowanceRate.ToString("N3"));
                    WriteToCell(startRow, PrintPayslipHelper.Instance.AddPayColumn, payroll.TotalExtensionAllowancePay.ToString("N2"));
                    startRow++;
                }

            }

            startRow = PrintPayslipHelper.Instance.OTStartRow;

            WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, "Regular:");
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.RegularOTHours).ToString("N2"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTRateColumn, payroll.RegularOTRate.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.RegularOTPay.ToString("N2"));
            startRow++;

            WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, "Sunday Overtime:");
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.SundayOTHours).ToString("N2"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTRateColumn, payroll.SundayOTRate.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.SundayOTPay.ToString("N2"));
            startRow++;

            WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, "Holiday (Reg):");
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable && !x.IsHazard).Sum(x => x.HolidayOTDays).ToString("N2"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTRateColumn, payroll.HolidayRegularOTRate.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable && !x.IsHazard).Sum(x => payroll.HolidayRegularOTRate * x.HolidayOTDays).ToDecimalPlaces(2).ToString("N2"));
            startRow++;

            for (int i = 0; i < locations.Count; i++)
            {
                if ((startRow - PrintPayslipHelper.Instance.OTStartRow) >= PrintPayslipHelper.Instance.OTMaxRow)
                {
                    InsertRowCopy(startRow, 1);
                }

                WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, $"Holiday ({locations[i].Prefix}):");
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, payroll.PayrollDetails.Where(x => x.Location?.ID == locations[i].ID && !x.IsNonTaxable).Sum(x => x.HolidayOTDays).ToString("N2"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTRateColumn, (payroll.HolidayRegularOTRate * ((locations[i].HazardRate ?? 0) + 1)).ToString("N3"));
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.PayrollDetails.Where(x => x.Location?.ID == locations[i].ID && !x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTRate * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2).ToString("N2"));
                startRow++;
            }

            if ((startRow - PrintPayslipHelper.Instance.OTStartRow) >= PrintPayslipHelper.Instance.OTMaxRow)
            {
                InsertRowCopy(startRow, 1);
            }

            WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, $"Holiday (Excess):");
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, payroll.PayrollDetails.Where(x => !x.IsNonTaxable).Sum(x => x.HolidayExcessOTHours).ToString("N2"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTRateColumn, payroll.HolidayExcessOTRate.ToString("N3"));
            WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.HolidayExcessOTPay.ToString("N2"));
            startRow++;

            if (payroll.NightDifferentialPay > 0)
            {
                if ((startRow - PrintPayslipHelper.Instance.OTStartRow) >= PrintPayslipHelper.Instance.OTMaxRow)
                {
                    InsertRowCopy(startRow, 1);
                }
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTDescColumn, $"Night Differential:");
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTHoursColumn, "1");
                WriteToCell(startRow, PrintPayslipHelper.Instance.OTPayColumn, payroll.NightDifferentialPay.ToString("N2"));
                startRow++;
            }



            var basics = payroll.PayrollDetails.GroupBy(x => x.Location?.ID ?? 0);

            startRow = PrintPayslipHelper.Instance.BasicStartRow;

            addRow = basics.Count() - PrintPayslipHelper.Instance.BasicMaxRow;
            if (addRow > 0)
            {
                InsertRowCopy(startRow, addRow);
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
            if (payroll.Allowance == 0)
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
