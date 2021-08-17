using GemBox.Spreadsheet;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ReportLayer.Bases;
using ReportLayer.Extensions;
using ReportLayer.Helpers;
using System.Linq;

namespace ReportLayer.Reports
{
    public sealed class PrintPayrollSheetB : SpreadSheetReportBase
    {
        public PrintPayrollSheetB(string template) : base(template)
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
            int endRow = PrintPayrollSheetBHelper.Instance.EndPageRow;
            foreach (var payroll in PayrollPeriod.Payrolls)
            {
                if (cnt == PrintPayrollSheetBHelper.Instance.MaxItem)
                {
                    Row = endRow + 1;
                    WriteHeader();
                    cnt = 0;
                    endRow += PrintPayrollSheetBHelper.Instance.EndPageRow;
                }
                WriteDetails(payroll, ++index);
                cnt++;
            }

            WriteFooter();
        }

        private void WriteHeader()
        {
            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell($"PAYROLL SHEET - {PayrollPeriod.Type}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            Row++;

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnMiddle + 1, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell(GetPeriod())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All);
            SetRowHeight(Row, 1);
            SetRowHeight(Row + 1, 1);

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnDesignation, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnRate, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("RATE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAllowance, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnAllowance)
                .WriteToCell("ALLOWANCE")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOT, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay)
                .WriteToCell("ADD'L PAY")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.ColumnVale)
                .WriteToCell("DEDUCTIONS");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale)
                .WriteToCell("OUTSTANDING VALE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell("NET AMOUNT")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .SetWrapText();

            Row++;

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, "Pag-IBIG Fund")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigLoan, "Pag-IBIG Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSS, "SSS")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnProvidentFund, "SSS (MPF)")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSSLoan, "SSS Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnWithholdingTax, "Withholding Tax")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPhilHealth, "Phil Health Contribution")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnVale, "VALE")
                .SetWrapText();

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => (payroll.DailyRate * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnEmployeeName, payroll.Personnel.FullName)
                .SetWrapText()
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnDesignation, payroll.Position)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnRate, payroll.DailyRate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAllowance, payroll.SumOfAllAllowance.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOT, payroll.TotalOTPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay, payroll.SumOfAllAdditionalPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, payroll.PagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigLoan, (payroll.PagibigLoan + payroll.PagibigCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSS, payroll.SSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnProvidentFund, payroll.ProvidentFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSSLoan, (payroll.SalaryLoan + payroll.SSSCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnWithholdingTax, payroll.Tax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPhilHealth, payroll.PhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnVale, payroll.Vale.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale, (-1 * payroll.OutstandingVale).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, payroll.NetPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            Row++;

            var locations = payroll.PayrollDetails.Where(x => x.IsHazard).GroupBy(x => x.Location.ID);
            foreach (var location in locations)
            {
                noofdays = location.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                decimal dailyrate = (payroll.DailyRate * ((location.FirstOrDefault()?.Location?.HazardRate ?? 0) + 1)).ToDecimalPlaces(3);
                basicpay = (dailyrate * noofdays).ToDecimalPlaces(2);

                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnRate, dailyrate.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, basicpay.ToString("N2"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

                Row++;
            }

        }
        private void WriteFooter()
        {
            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPreparedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnPreparedByEnd)
            //    .WriteToCell("Prepared By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnCheckedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnCheckedByEnd)
            //    .WriteToCell("Checked By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnApprovedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnApprovedByEnd)
            //    .WriteToCell("Approved By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //Row++;

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPreparedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnPreparedByEnd)
            //    .WriteToCell(PayrollPeriod.PreparedBy)
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
            //    .SetBorder(MultipleBorders.Bottom);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnCheckedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnCheckedByEnd)
            //    .WriteToCell(PayrollPeriod.CheckedBy)
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
            //    .SetBorder(MultipleBorders.Bottom);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnApprovedByStart, Row, PrintPayrollSheetBHelper.Instance.ColumnApprovedByEnd)
            //    .WriteToCell(PayrollPeriod.ApprovedBy)
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
            //    .SetBorder(MultipleBorders.Bottom);

            //Row++;
        }

        private string GetPeriod()
        {
            return $"{PayrollPeriod.StartDate:MMMM dd yyyy} - {PayrollPeriod.EndDate:MMMM dd yyyy}";
        }

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
