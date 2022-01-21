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
            CellBorder(Row, 0, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("SUB TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, TotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAllowance, TotalAllowance.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOT, TotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay, TotalAdditionalPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, TotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSS, TotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnProvidentFund, TotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSSLoan, TotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnWithholdingTax, TotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPhilHealth, TotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnVale, TotalVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale, TotalOutStandingVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, TotalNetpay.ToString("N2"));
            Row++;

            ResetTotal();

        }
        private void WriteGrandTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, GrandTotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAllowance, GrandTotalAllowance.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOT, GrandTotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay, GrandTotalAdditionalPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, GrandTotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigLoan, GrandTotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSS, GrandTotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnProvidentFund, GrandTotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSSLoan, GrandTotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnWithholdingTax, GrandTotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPhilHealth, GrandTotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnVale, GrandTotalVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale, GrandTotalOutStandingVale.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, GrandTotalNetpay.ToString("N2"));
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
            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays)
                .WriteToCell("DEPARTMENT: OFFICE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell($"FOR THE PERIOD FROM: {PayrollPeriod.StartDate:MMMM dd yyyy} - {PayrollPeriod.EndDate:MMMM dd yyyy}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All)
                .SetFontWeight(700)
                .SetBackgroud(SpreadsheetColor.FromArgb(230, 240, 240));

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnDesignation, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnRate, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("Rate");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAllowance, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnAllowance)
                .WriteToCell("ALLOWANCE");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOT, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnAdditionalPay)
                .WriteToCell("Additional Pay")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.ColumnVale)
                .WriteToCell("DEDUCTIONS");

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnOutstandingVale)
                .WriteToCell("Outstanding Vale")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNetAmount, Row + 1, PrintPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell("NET AMOUNT")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .SetWrapText();

            Row++;

            GetRange(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetBHelper.Instance.ColumnVale)
                .SetWrapText()
                .SetFontSize(130)
                .SetFontWeight(100);
            SetRowHeight(Row, 0.7, LengthUnit.Centimeter);

            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigFund, "Pag Ibig Fund");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPagibigLoan, "Pag Ibig Loan");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSS, "SSS");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnProvidentFund, "SSS (MPF)");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSSSLoan, "SSS Loan");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnWithholdingTax, "Withholding Tax");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnPhilHealth, "PhilHealth Contribution");
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnVale, "Vale");

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => payroll.DailyRate * x.RegularDay).ToDecimalPlaces(2);
            SetRowHeight(Row, 0.5);
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetItalic();
            WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnEmployeeName, payroll.FullName)
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

                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnRate, dailyrate.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetBHelper.Instance.ColumnSalary, basicpay.ToString("N2"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                TotalBasicPay += basicpay;
                Cnt++;
                Row++;
            }

        }

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
