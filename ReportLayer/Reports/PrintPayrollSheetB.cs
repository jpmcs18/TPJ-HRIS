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
    public class PrintPayrollSheetB : SpreadSheetReportBase
    {
        public PrintPayrollSheetB(string template) : base(template)
        {
        }

        public PayrollPeriod PayrollPeriod { get; set; }
        private int Row = 1;
        private decimal TotalBasicPay = 0;
        private decimal TotalAllowance = 0;
        private decimal TotalOT = 0;
        private decimal TotalAdditionalPay = 0;
        private decimal TotalPagibigFund = 0;
        private decimal TotalPagibigLoan = 0;
        private decimal TotalSSS = 0;
        private decimal TotalProvident = 0;
        private decimal TotalSSSLoan = 0;
        private decimal TotalTax = 0;
        private decimal TotalPhilHealth = 0;
        private decimal TotalVale = 0;
        private decimal TotalOutStandingVale = 0;
        private decimal TotalNetpay = 0;
        public override void GenerateReport()
        {
            base.GenerateReport();

            WriteHeader();

            int index = 0;
            int cnt = 0;
            int endRow = PrintPayrollSheetBHelper.Instance.Value.EndPageRow;
            foreach (var payroll in PayrollPeriod.Payrolls)
            {
                if (cnt == PrintPayrollSheetBHelper.Instance.Value.MaxItem-1)
                {
                    WriteTotal();
                    Row = endRow + 1;
                    WriteHeader();
                    cnt = 0;
                    endRow += PrintPayrollSheetBHelper.Instance.Value.EndPageRow;
                }
                WriteDetails(payroll, ++index);
                cnt++;
            }
            if(cnt > 0)
            {
                WriteTotal();
            }
            WriteFooter();
        }
        private void WriteTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, TotalBasicPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, TotalAllowance.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, TotalOT.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, TotalAdditionalPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, TotalPagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, TotalSSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, TotalProvident.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, TotalSSSLoan.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, TotalTax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, TotalPhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, TotalVale.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, TotalOutStandingVale.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, TotalNetpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            Row++;

            ResetTotal();

        }
        private void ResetTotal()
        {
            TotalBasicPay = 0;
            TotalAllowance = 0;
            TotalOT = 0;
            TotalAdditionalPay = 0;
            TotalPagibigFund = 0;
            TotalPagibigLoan = 0;
            TotalSSS = 0;
            TotalProvident = 0;
            TotalSSSLoan = 0;
            TotalTax = 0;
            TotalPhilHealth = 0;
            TotalVale = 0;
            TotalOutStandingVale = 0;
            TotalNetpay = 0;
        }
        private void WriteHeader()
        {
            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell($"PAYROLL SHEET - {PayrollPeriod.Type}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            Row++;

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnMiddle + 1, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell(GetPeriod())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All);
            SetRowHeight(Row, 1);
            SetRowHeight(Row + 1, 1);

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnDesignation, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnRate)
                .WriteToCell("RATE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance)
                .WriteToCell("ALLOWANCE")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay)
                .WriteToCell("ADD'L PAY")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale)
                .WriteToCell("DEDUCTIONS");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale)
                .WriteToCell("OUTSTANDING VALE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell("NET AMOUNT")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .SetWrapText();

            Row++;

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, "Pag-IBIG Fund")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, "Pag-IBIG Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, "SSS")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, "SSS (MPF)")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, "SSS Loan")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, "Withholding Tax")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, "Phil Health Contribution")
                .SetWrapText();

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, "VALE")
                .SetWrapText();

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => payroll.DailyRate * x.RegularDay).ToDecimalPlaces(2);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnEmployeeName, payroll.Personnel.FullName)
                .SetWrapText()
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnDesignation, payroll.Position)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate, payroll.DailyRate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, payroll.SumOfAllAllowance.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, payroll.TotalOTPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, payroll.SumOfAllAdditionalPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, payroll.PagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, (payroll.PagibigLoan + payroll.PagibigCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, payroll.SSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, payroll.ProvidentFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, (payroll.SalaryLoan + payroll.SSSCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, payroll.Tax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, payroll.PhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, payroll.Vale.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, (-1 * payroll.OutstandingVale).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, payroll.NetPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            Row++;


            TotalBasicPay += basicpay;
            TotalAllowance += payroll.SumOfAllAllowance;
            TotalOT += payroll.TotalOTPay;
            TotalAdditionalPay += payroll.SumOfAllAdditionalPay;
            TotalPagibigFund += payroll.PagibigFund;
            TotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            TotalSSS += payroll.SSS;
            TotalProvident += payroll.ProvidentFund;
            TotalSSSLoan += (payroll.SalaryLoan + payroll.SSSCalamityLoan);
            TotalTax += payroll.Tax;
            TotalPhilHealth += payroll.PhilHealth;
            TotalVale += payroll.Vale;
            TotalOutStandingVale += (-1 * payroll.OutstandingVale);
            TotalNetpay += payroll.NetPay;

            var locations = payroll.PayrollDetails.Where(x => x.IsHazard).GroupBy(x => x.Location.ID);
            foreach (var location in locations)
            {
                noofdays = location.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                decimal dailyrate = (payroll.DailyRate * ((location.FirstOrDefault()?.Location?.HazardRate ?? 0) + 1)).ToDecimalPlaces(3);
                basicpay = (dailyrate * noofdays).ToDecimalPlaces(2);

                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays, noofdays.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate, dailyrate.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, basicpay.ToString("N2"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                TotalBasicPay += basicpay;

                Row++;
            }

        }
        private void WriteFooter()
        {
            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPreparedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPreparedByEnd)
            //    .WriteToCell("Prepared By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnCheckedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnCheckedByEnd)
            //    .WriteToCell("Checked By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnApprovedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnApprovedByEnd)
            //    .WriteToCell("Approved By:")
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            //Row++;

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPreparedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPreparedByEnd)
            //    .WriteToCell(PayrollPeriod.PreparedBy)
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
            //    .SetBorder(MultipleBorders.Bottom);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnCheckedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnCheckedByEnd)
            //    .WriteToCell(PayrollPeriod.CheckedBy)
            //    .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
            //    .SetBorder(MultipleBorders.Bottom);

            //MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnApprovedByStart, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnApprovedByEnd)
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
