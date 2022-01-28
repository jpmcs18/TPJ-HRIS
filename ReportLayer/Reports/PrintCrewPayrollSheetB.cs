using GemBox.Spreadsheet;
using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ReportLayer.Bases;
using ReportLayer.Extensions;
using ReportLayer.Helpers;
using System.Linq;

namespace ReportLayer.Reports
{
    public class PrintCrewPayrollSheetB : SpreadSheetReportBase
    {
        public PrintCrewPayrollSheetB(string template) : base(template)
        {
        }

        public CrewPayrollPeriod PayrollPeriod { get; set; }
        private int Row = 1;
        private decimal TotalBasicPay = 0;
        private decimal TotalPagibigFund = 0;
        private decimal TotalPagibigLoan = 0;
        private decimal TotalSSS = 0;
        private decimal TotalProvident = 0;
        private decimal TotalSSSLoan = 0;
        private decimal TotalTax = 0;
        private decimal TotalPhilHealth = 0;
        private decimal TotalVale = 0;
        private decimal TotalNetpay = 0;

        private decimal GrandTotalBasicPay = 0;
        private decimal GrandTotalPagibigFund = 0;
        private decimal GrandTotalPagibigLoan = 0;
        private decimal GrandTotalSSS = 0;
        private decimal GrandTotalProvident = 0;
        private decimal GrandTotalSSSLoan = 0;
        private decimal GrandTotalTax = 0;
        private decimal GrandTotalPhilHealth = 0;
        private decimal GrandTotalVale = 0;
        private decimal GrandTotalNetpay = 0;
        private int Cnt = 0;
        public override void GenerateReport()
        {
            base.GenerateReport();


            int sheets = 0;
            foreach (var vessel in PayrollPeriod.CrewVessel)
            {
                DuplicateSheet(vessel.Vessel.Code);
                WriteCrewPayroll(vessel);
                sheets++;
            }
            if (sheets > 0)
            {
                DeleteSheet();
            }
        }

        private void WriteCrewPayroll(CrewVessel crewVessel)
        {

            WriteHeader(crewVessel.Vessel);

            int index = 0;
            foreach (var payroll in crewVessel.CrewPayrolls)
            {
                WriteClosing(crewVessel.Vessel);
                WriteDetails(payroll, ++index);
            }
            if (Cnt > 0)
            {
                WriteGrandTotal();
            }
        }

        private void WriteClosing(Vessel vessel)
        {
            if (Cnt == 30)
            {
                WriteTotal();
                Row += 2;
                WriteHeader(vessel);
                Cnt = 0;
            }
        }

        private void WriteTotal()
        {
            CellBorder(Row, 0, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("SUB TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, TotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, TotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSS, TotalSSS.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnProvidentFund, TotalProvident.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSSLoan, TotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnWithholdingTax, TotalTax.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPhilHealth, TotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale, TotalVale.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, TotalNetpay.ToString("N2"));
            Row++;

            ResetTotal();

        }
        private void WriteGrandTotal()
        {
            CellBorder(Row, 0, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, GrandTotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, GrandTotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigLoan, GrandTotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSS, GrandTotalSSS.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnProvidentFund, GrandTotalProvident.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSSLoan, GrandTotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnWithholdingTax, GrandTotalTax.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPhilHealth, GrandTotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale, GrandTotalVale.ToString("N2"));
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, GrandTotalNetpay.ToString("N2"));
            Row++;
        }
        private void ResetTotal()
        {
            TotalBasicPay = 0;
            TotalPagibigFund = 0;
            TotalPagibigLoan = 0;
            TotalSSS = 0;
            TotalProvident = 0;
            TotalSSSLoan = 0;
            TotalTax = 0;
            TotalPhilHealth = 0;
            TotalVale = 0;
            TotalNetpay = 0;
        }
        private void WriteHeader(Vessel vessel)
        {
            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNo, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNoOfDays)
                .WriteToCell($"VESSEL:{vessel.Description}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell($"FOR THE PERIOD FROM: {PayrollPeriod.StartDate:MMMM dd yyyy} - {PayrollPeriod.EndDate:MMMM dd yyyy}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

            Row++;

            GetRange(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All)
                .SetFontWeight(700)
                .SetBackgroud(SpreadsheetColor.FromArgb(230, 240, 240));

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNo, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPosition, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnPosition)
                .WriteToCell("POSITION");

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNoOfDays, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnRate, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnRate)
                .WriteToCell("Rate");

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale)
                .WriteToCell("DEDUCTIONS");

            MergeCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, Row + 1, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount)
                .WriteToCell("NET AMOUNT")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .SetWrapText();

            Row++;

            GetRange(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale)
                .SetWrapText()
                .SetFontSize(130)
                .SetFontWeight(100);
            SetRowHeight(Row, 0.7, LengthUnit.Centimeter);

            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, "Pag Ibig Fund");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigLoan, "Pag Ibig Loan");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSS, "SSS");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnProvidentFund, "SSS (MPF)");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSSLoan, "SSS Loan");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnWithholdingTax, "Withholding Tax");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPhilHealth, "PhilHealth");
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale, "Vale");

            Row++;
        }

        private void WriteDetails(CrewPayroll payroll, int index)
        {
            SetRowHeight(Row, 0.5);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetItalic();
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnEmployeeName, payroll.FullName)
                .SetWrapText()
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigFund, payroll.PagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPagibigLoan, (payroll.PagibigLoan + payroll.PagibigCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSS, payroll.SSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnProvidentFund, payroll.ProvidentFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSSSLoan, payroll.SSSLoan.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnWithholdingTax, payroll.Tax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPhilHealth, payroll.PhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnVale, 0.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNetAmount, payroll.NetPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);


            TotalPagibigFund += payroll.PagibigFund;
            TotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            TotalSSS += payroll.SSS;
            TotalProvident += payroll.ProvidentFund;
            TotalSSSLoan += payroll.SSSLoan;
            TotalTax += payroll.Tax;
            TotalPhilHealth += payroll.PhilHealth;
            TotalNetpay += payroll.NetPay;

            GrandTotalPagibigFund += payroll.PagibigFund;
            GrandTotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            GrandTotalSSS += payroll.SSS;
            GrandTotalProvident += payroll.ProvidentFund;
            GrandTotalSSSLoan += payroll.SSSLoan;
            GrandTotalTax += payroll.Tax;
            GrandTotalPhilHealth += payroll.PhilHealth;
            GrandTotalNetpay += payroll.NetPay;
            Cnt++;

            var dailyRates = payroll.CrewPayrollDetails.GroupBy(x => x.DailyRate);
            foreach (var dailyRate in dailyRates)
            {
                WriteClosing(payroll.Vessel);
                decimal noofdays = dailyRate.Where(x => !x.IsAdditionalsOnly).Select(x => x.LoggedDate).Count();
                    noofdays += dailyRate.Where(x => x.IsSunday).Select(x => CrewPayrollParameters.Instance.CrewSundayRate).Sum();
                    noofdays += dailyRate.Where(x => x.IsHoliday).Select(x => CrewPayrollParameters.Instance.CrewHolidayRate).Sum();
                decimal rate = dailyRate.Select(x => x.DailyRate).FirstOrDefault();
                decimal salary = rate * noofdays;

                WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnPosition, dailyRate.Select(x => x.Position?.Description).FirstOrDefault())
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
                WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnRate, rate.ToString("N3"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintCrewPayrollSheetBHelper.Instance.ColumnSalary, salary.ToString("N2"))
                    .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                TotalBasicPay += salary;
                GrandTotalBasicPay += salary;

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
