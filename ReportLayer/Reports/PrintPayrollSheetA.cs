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
    public class PrintPayrollSheetA : SpreadSheetReportBase
    {
        public PrintPayrollSheetA(string template) : base(template)
        {
        }

        public PayrollPeriod PayrollPeriod { get; set; }

        private int Row { get; set; } = 1;

        public override void GenerateReport()
        {
            base.GenerateReport();

            WriteHeader();
            int index = 0;
            int cnt = 0;
            int endRow = PrintPayrollSheetAHelper.Instance.Value.EndPageRow;
            foreach (var payroll in PayrollPeriod.Payrolls)
            {
                if (cnt == PrintPayrollSheetAHelper.Instance.Value.MaxItem)
                {
                    Row = endRow + 1;
                    WriteHeader();
                    cnt = 0;
                    endRow += PrintPayrollSheetAHelper.Instance.Value.EndPageRow;
                }
                WriteDetails(payroll, ++index);
                cnt++;
            }

        }

        private void WriteHeader()
        {
            MergeCell(Row, 0, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell($"PAYROLL SHEET - {PayrollPeriod.Type}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
            Row++;

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnMiddle + 1, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell(GetPeriod())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            Row++;
            GetRange(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All);

            SetRowHeight(Row, 1);
            SetRowHeight(Row + 1, 1);
            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnDesignation, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnRate)
                .WriteToCell("RATE");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnOT, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth)
                .WriteToCell("DEDUCTIONS");
           
            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .WriteToCell("NET AMOUNT");

            Row++;

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, "Pag-IBIG Fund")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigLoan, "Pag-IBIG Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSS, "SSS")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSSLoan, "SSS Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnProvidentFund, "SSS (MPF)")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnWithholdingTax, "Withholding Tax")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth, "PhilHealth Contribution")
                .SetWrapText();

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => (payroll.DailyRate * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnEmployeeName, payroll.Personnel.FullName)
                .SetWrapText()
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnDesignation, payroll.Position)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate, payroll.DailyRate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnOT, payroll.TotalOTPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, payroll.PagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigLoan, (payroll.PagibigLoan + payroll.PagibigCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSS, payroll.SSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSSLoan, (payroll.SalaryLoan + payroll.SSSCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnProvidentFund, payroll.ProvidentFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnWithholdingTax, payroll.Tax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth, payroll.PhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, payroll.NetPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            Row++;


            var locations = payroll.PayrollDetails.Where(x => x.IsHazard).GroupBy(x => x.Location.ID);
            foreach (var location in locations)
            {
                noofdays = location.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                decimal dailyrate = (payroll.DailyRate * ((location.FirstOrDefault()?.Location?.HazardRate ?? 0) + 1)).ToDecimalPlaces(3);
                basicpay = (dailyrate * noofdays).ToDecimalPlaces(2);

                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate, dailyrate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

                Row++;
            }

        }

        private string GetPeriod()
        {
            DateTime start = new DateTime(PayrollPeriod.EndDate.Year, PayrollPeriod.EndDate.Month, 1); 
            DateTime end = (new DateTime(PayrollPeriod.EndDate.Year, PayrollPeriod.EndDate.Month + 1, 1)).AddDays(-1);
            return $"{start:MMMM dd yyyy} - {end:MMMM dd yyyy}";
        }

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
