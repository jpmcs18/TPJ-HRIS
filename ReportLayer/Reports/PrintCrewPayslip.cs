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
    public class PrintCrewPayslip : SpreadSheetReportBase
    {
        public PrintCrewPayslip(string template) : base(template)
        {
        }
        public CrewPayrollPeriod PayrollPeriod { get; set; }
        public CrewVessel CrewVessel { get; set; }

        public override void GenerateReport()
        {
            base.GenerateReport();

            int cnt = 0;
            foreach (var payroll in CrewVessel.CrewPayrolls)
            {
                DuplicateSheet((cnt++).ToString());
                WriteDetails(payroll);
            }
            DeleteSheet();
        }

        private void WriteDetails(CrewPayroll payroll)
        {
            int addRow = 0;

            var totalOTPay = payroll.CrewPayrollDetails.Where(x => x.IsSunday).Sum(x => x.DailyRate * CrewPayrollParameters.Instance.CrewSundayRate);
                totalOTPay += payroll.CrewPayrollDetails.Where(x => x.IsHoliday).Sum(x => x.DailyRate * CrewPayrollParameters.Instance.CrewHolidayRate);

            WriteToCell(PrintCrewPayslipHelper.Instance.EmployeeNameCell, payroll.Personnel.FullName);
            WriteToCell(PrintCrewPayslipHelper.Instance.TINNoCell, "TIN: " + payroll.Personnel.TIN);
            WriteToCell(PrintCrewPayslipHelper.Instance.HDMFNoCell, "HDMF No: " + payroll.Personnel.PAGIBIG);
            WriteToCell(PrintCrewPayslipHelper.Instance.SSSNoCell, "SSS No: " + payroll.Personnel.SSS);

            var totalNoOfDays = payroll.CrewPayrollDetails.Where(x => !x.IsAdditionalsOnly).Count();
            WriteToCell(PrintCrewPayslipHelper.Instance.BasicTotalDaysCell, totalNoOfDays.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.BasicSalaryCell, payroll.BasicPay.ToString("N2"));

            WriteToCell(PrintCrewPayslipHelper.Instance.TotalOTPayCell, totalOTPay.ToString("N2"));

            WriteToCell(PrintCrewPayslipHelper.Instance.PagibigLoanCell, payroll.PagibigLoan.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.PagibigCell, payroll.PagibigFund.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.SSSCell, payroll.SSS.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.ProvidentFundCell, payroll.ProvidentFund.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.SSSLoanCell, payroll.SSSLoan.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.TaxCell, payroll.Tax.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.PhilHealthCell, payroll.PhilHealth.ToString("N2"));
            WriteToCell(PrintCrewPayslipHelper.Instance.ValeCell, 0.ToString("N2"));

            WriteToCell(PrintCrewPayslipHelper.Instance.TotalDedCell, payroll.TotalDeductions.ToString("N2"));

            WriteToCell(PrintCrewPayslipHelper.Instance.NetCell, payroll.NetPay.ToString("N2"));
            
            WriteToCell(PrintCrewPayslipHelper.Instance.SignatoryCell, payroll.Personnel.FullName);

            var startRow = PrintCrewPayslipHelper.Instance.OTStartRow;

            var regRate = payroll.CrewPayrollDetails.Where(x => !x.IsCorrected).Select(x => x.DailyRate).FirstOrDefault();
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "REGULAR : ");
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, 0.ToString("N2"));
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, (regRate * CrewPayrollParameters.Instance.CrewRegularRate).ToString("N3"));
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, 0.ToString("N2"));
            startRow++;

            var sundayOts = payroll.CrewPayrollDetails.Where(x => x.IsSunday).GroupBy(x => x.DailyRate);
            if (sundayOts?.Any() ?? false)
            {
                foreach (var sundayOt in sundayOts)
                {
                    decimal count = sundayOt.Count();
                    decimal rate = sundayOt.Select(x => x.DailyRate).FirstOrDefault() * CrewPayrollParameters.Instance.CrewSundayRate; 
                    decimal amount = count * rate;
                    if ((startRow - PrintCrewPayslipHelper.Instance.OTStartRow) >= PrintCrewPayslipHelper.Instance.OTMaxRow)
                    {
                        InsertRowCopy(startRow, 1);
                    }
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "Addt'l 30% Sunday : ");
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, count.ToString("N2"));
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, rate.ToString("N3"));
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, amount.ToString("N2"));
                    startRow++;
                }
            }
            else
            {
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "Addt'l 30% Sunday : ");
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, 0.ToString("N2"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, (regRate * CrewPayrollParameters.Instance.CrewSundayRate).ToString("N3"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, 0.ToString("N2"));
                startRow++;
            }

            var holidayOts = payroll.CrewPayrollDetails.Where(x => x.IsHoliday).GroupBy(x => x.DailyRate);
            if (holidayOts?.Any() ?? false)
            {
                foreach (var holidayOt in holidayOts)
                {
                    decimal count = holidayOt.Count();
                    decimal rate = holidayOt.Select(x => x.DailyRate).FirstOrDefault() * CrewPayrollParameters.Instance.CrewHolidayRate;
                    decimal amount = count * rate;
                    if ((startRow - PrintCrewPayslipHelper.Instance.OTStartRow) >= PrintCrewPayslipHelper.Instance.OTMaxRow)
                    {
                        InsertRowCopy(startRow, 1);
                    }
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "Addt'l 130% HOLIDAY (REG) : ");
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, count.ToString("N2"));
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, rate.ToString("N3"));
                    WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, amount.ToString("N2"));
                    startRow++;
                }
            }
            else
            {
                if ((startRow - PrintCrewPayslipHelper.Instance.OTStartRow) >= PrintCrewPayslipHelper.Instance.OTMaxRow)
                {
                    InsertRowCopy(startRow, 1);
                }

                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "Addt'l 130% HOLIDAY (REG) : ");
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, 0.ToString("N2"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, (regRate * CrewPayrollParameters.Instance.CrewHolidayRate).ToString("N3"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, 0.ToString("N2"));
                startRow++;
            }
             
            if ((startRow - PrintCrewPayslipHelper.Instance.OTStartRow) >= PrintCrewPayslipHelper.Instance.OTMaxRow)
            {
                InsertRowCopy(startRow, 1);
            }
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTDescColumn, "HOLIDAY (EXCESS) : ");
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTHoursColumn, 0.ToString("N2"));
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTRateColumn, ((regRate / 8) * CrewPayrollParameters.Instance.CrewHolidayRate).ToString("N3"));
            WriteToCell(startRow, PrintCrewPayslipHelper.Instance.OTPayColumn, 0.ToString("N2"));
            startRow++;

            var basics = payroll.CrewPayrollDetails.Where(x => !x.IsAdditionalsOnly).OrderBy(x => x.LoggedDate).GroupBy(x => x.DailyRate);

            startRow = PrintCrewPayslipHelper.Instance.BasicStartRow;

            addRow = basics.Count() - PrintCrewPayslipHelper.Instance.BasicMaxRow;
            if (addRow > 0)
            {
                InsertRowCopy(startRow, addRow);
            }
            foreach (var basic in basics)
            {
                decimal dailyRate = basic.Select(x => x.DailyRate).FirstOrDefault();
                decimal noOfDays = basic.Count();
                var vessel = basic.Select(x => x.Vessel?.Description).FirstOrDefault();
                var position = basic.Select(x => x.Position?.Description).FirstOrDefault();
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.PayrollPeriodColumn, GetPeriod(basic.Select(x => x.LoggedDate).First(), basic.Select(x => x.LoggedDate).Last()));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.VesselColumn, vessel)
                .SetWrapText();
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.PositionColumn, position)
                .SetWrapText();
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.DaysColumn, noOfDays.ToString("N3"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.RateColumn, dailyRate.ToString("N3"));
                WriteToCell(startRow, PrintCrewPayslipHelper.Instance.AmountColumn, (dailyRate * noOfDays).ToString("N2"));
                AutofitRow(startRow);
                startRow++;
            } 
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
