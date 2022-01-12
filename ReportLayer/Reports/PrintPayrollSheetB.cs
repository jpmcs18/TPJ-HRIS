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

        private decimal GrandTotalBasicPay = 0;
        private decimal GrandTotalAllowance = 0;
        private decimal GrandTotalOT = 0;
        private decimal GrandTotalAdditionalPay = 0;
        private decimal GrandTotalPagibigFund = 0;
        private decimal GrandTotalPagibigLoan = 0;
        private decimal GrandTotalSSS = 0;
        private decimal GrandTotalProvident = 0;
        private decimal GrandTotalSSSLoan = 0;
        private decimal GrandTotalTax = 0;
        private decimal GrandTotalPhilHealth = 0;
        private decimal GrandTotalVale = 0;
        private decimal GrandTotalOutStandingVale = 0;
        private decimal GrandTotalNetpay = 0;
        private int Cnt = 0;
        public override void GenerateReport()
        {
            base.GenerateReport();

            WriteHeader();

            int index = 0;
            foreach (var payroll in PayrollPeriod.Payrolls)
            {
                WriteClosing();
                WriteDetails(payroll, ++index);
            }
            if (Cnt > 0)
            {
                WriteGrandTotal();
            }
            WriteFooter();
        }

        private void WriteClosing()
        {
            if (Cnt == 30)
            {
                WriteTotal();
                Row += 2;
                WriteHeader();
                Cnt = 0;
            }
        }

        private void WriteTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate)
                .WriteToCell("SUB TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, TotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, TotalAllowance.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, TotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, TotalAdditionalPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, TotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, TotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, TotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, TotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, TotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, TotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, TotalVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, TotalOutStandingVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, TotalNetpay.ToString("N2"));
            Row++;

            ResetTotal();

        }
        private void WriteGrandTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, GrandTotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, GrandTotalAllowance.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, GrandTotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, GrandTotalAdditionalPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, GrandTotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, GrandTotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, GrandTotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, GrandTotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, GrandTotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, GrandTotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, GrandTotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, GrandTotalVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, GrandTotalOutStandingVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, GrandTotalNetpay.ToString("N2"));
            Row++;


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
            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays)
                .WriteToCell("DEPARTMENT: OFFICE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell($"FOR THE PERIOD FROM: {PayrollPeriod.StartDate:MMMM dd yyyy} - {PayrollPeriod.EndDate:MMMM dd yyyy}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All)
                .SetFontWeight(700)
                .SetBackgroud(SpreadsheetColor.FromArgb(230, 240, 240));

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnDesignation, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnRate, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnRate)
                .WriteToCell("Rate");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnAllowance)
                .WriteToCell("ALLOWANCE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOT, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnAdditionalPay)
                .WriteToCell("Additional Pay")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale)
                .WriteToCell("DEDUCTIONS");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnOutstandingVale)
                .WriteToCell("Outstanding Vale")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount, Row + 1, PrintPayrollSheetBHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell("NET AMOUNT")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .SetWrapText();

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale)
                .SetWrapText()
                .SetFontSize(130)
                .SetFontWeight(100);
            SetRowHeight(Row, 0.7, LengthUnit.Centimeter);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigFund, "Pag Ibig Fund");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPagibigLoan, "Pag Ibig Loan");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSS, "SSS");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnProvidentFund, "SSS (MPF)");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnSSSLoan, "SSS Loan");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnWithholdingTax, "Withholding Tax");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnPhilHealth, "PhilHealth Contribution");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnVale, "Vale");

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => payroll.DailyRate * x.RegularDay).ToDecimalPlaces(2);
            SetRowHeight(Row, 0.5);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetItalic();
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.Value.ColumnEmployeeName, payroll.FullName)
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

            GrandTotalBasicPay += basicpay;
            GrandTotalAllowance += payroll.SumOfAllAllowance;
            GrandTotalOT += payroll.TotalOTPay;
            GrandTotalAdditionalPay += payroll.SumOfAllAdditionalPay;
            GrandTotalPagibigFund += payroll.PagibigFund;
            GrandTotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            GrandTotalSSS += payroll.SSS;
            GrandTotalProvident += payroll.ProvidentFund;
            GrandTotalSSSLoan += (payroll.SalaryLoan + payroll.SSSCalamityLoan);
            GrandTotalTax += payroll.Tax;
            GrandTotalPhilHealth += payroll.PhilHealth;
            GrandTotalVale += payroll.Vale;
            GrandTotalOutStandingVale += (-1 * payroll.OutstandingVale);
            GrandTotalNetpay += payroll.NetPay;
            Cnt++;

            var locations = payroll.PayrollDetails.Where(x => x.IsHazard).GroupBy(x => x.Location.ID);
            foreach (var location in locations)
            {
                WriteClosing();
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
                Cnt++;
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

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
