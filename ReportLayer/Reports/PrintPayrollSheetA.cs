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

        private int Row = 1;
        private decimal TotalBasicPay = 0;
        private decimal TotalOT = 0;
        private decimal TotalPagibigFund = 0;
        private decimal TotalPagibigLoan = 0;
        private decimal TotalSSS = 0;
        private decimal TotalProvident = 0;
        private decimal TotalSSSLoan = 0;
        private decimal TotalTax = 0;
        private decimal TotalPhilHealth = 0;
        private decimal TotalNetpay = 0;

        private decimal GrandTotalBasicPay = 0;
        private decimal GrandTotalOT = 0;
        private decimal GrandTotalPagibigFund = 0;
        private decimal GrandTotalPagibigLoan = 0;
        private decimal GrandTotalSSS = 0;
        private decimal GrandTotalProvident = 0;
        private decimal GrandTotalSSSLoan = 0;
        private decimal GrandTotalTax = 0;
        private decimal GrandTotalPhilHealth = 0;
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
            CellBorder(Row, 0, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate)
                .WriteToCell("SUB TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, TotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnOT, TotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, TotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSS, TotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnProvidentFund, TotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSSLoan, TotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnWithholdingTax, TotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth, TotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, TotalNetpay.ToString("N2"));
            Row++;

            ResetTotal();

        }
        private void WriteGrandTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, GrandTotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnOT, GrandTotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, GrandTotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigLoan, GrandTotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSS, GrandTotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnProvidentFund, GrandTotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSSLoan, GrandTotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnWithholdingTax, GrandTotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth, GrandTotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount, GrandTotalNetpay.ToString("N2"));
            Row++;


        }
        private void ResetTotal()
        {
            TotalBasicPay = 0;
            TotalOT = 0;
            TotalPagibigFund = 0;
            TotalPagibigLoan = 0;
            TotalSSS = 0;
            TotalProvident = 0;
            TotalSSSLoan = 0;
            TotalTax = 0;
            TotalPhilHealth = 0;
            TotalNetpay = 0;
        }

        private void WriteHeader()
        {
            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays)
                .WriteToCell("DEPARTMENT: OFFICE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .WriteToCell($"FOR THE PERIOD FROM: {GetPeriod()}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

            Row++;
            GetRange(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, Row + 1, PrintPayrollSheetAHelper.Instance.Value.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All)
                .SetFontWeight(700)
                .SetBackgroud(SpreadsheetColor.FromArgb(230, 240, 240));

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

            GetRange(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth)
                .SetWrapText()
                .SetFontSize(130)
                .SetFontWeight(100);
            SetRowHeight(Row, 0.7, LengthUnit.Centimeter);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigFund, "Pag-IBIG Fund");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPagibigLoan, "Pag-IBIG Loan");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSS, "SSS");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSSSLoan, "SSS Loan");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnProvidentFund, "SSS (MPF)");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnWithholdingTax, "Withholding Tax");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnPhilHealth, "PhilHealth Contribution");

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => (payroll.DailyRate * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2);
            SetRowHeight(Row, 0.5);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetItalic();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnEmployeeName, payroll.FullName)
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

            
            TotalBasicPay += basicpay;
            TotalOT += payroll.TotalOTPay;
            TotalPagibigFund += payroll.PagibigFund;
            TotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            TotalSSS += payroll.SSS;
            TotalProvident += payroll.ProvidentFund;
            TotalSSSLoan += (payroll.SalaryLoan + payroll.SSSCalamityLoan);
            TotalTax += payroll.Tax;
            TotalPhilHealth += payroll.PhilHealth;
            TotalNetpay += payroll.NetPay;

            GrandTotalBasicPay += basicpay;
            GrandTotalOT += payroll.TotalOTPay;
            GrandTotalPagibigFund += payroll.PagibigFund;
            GrandTotalPagibigLoan += (payroll.PagibigLoan + payroll.PagibigCalamityLoan);
            GrandTotalSSS += payroll.SSS;
            GrandTotalProvident += payroll.ProvidentFund;
            GrandTotalSSSLoan += (payroll.SalaryLoan + payroll.SSSCalamityLoan);
            GrandTotalTax += payroll.Tax;
            GrandTotalPhilHealth += payroll.PhilHealth;
            GrandTotalNetpay += payroll.NetPay;
            Cnt++;

            var locations = payroll.PayrollDetails.Where(x => x.IsHazard).GroupBy(x => x.Location.ID);
            foreach (var location in locations)
            {
                WriteClosing();
                noofdays = location.Sum(x => x.RegularDay).ToDecimalPlaces(3);
                decimal dailyrate = (payroll.DailyRate * ((location.FirstOrDefault()?.Location?.HazardRate ?? 0) + 1)).ToDecimalPlaces(3);
                basicpay = (dailyrate * noofdays).ToDecimalPlaces(2);

                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnRate, dailyrate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.Value.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                TotalBasicPay += basicpay;
                Cnt++; 
                Row++;
            }

        }

        private string GetPeriod()
        {
            DateTime start = new DateTime(PayrollPeriod.EndDate.Year, PayrollPeriod.EndDate.Month, 1); 
            DateTime end = (new DateTime(PayrollPeriod.EndDate.Year, PayrollPeriod.EndDate.Month, 1)).AddMonths(1).AddDays(-1);
            return $"{start:MMMM dd yyyy} - {end:MMMM dd yyyy}";
        }

        public override void Dispose()
        {
            PayrollPeriod = null;
            base.Dispose();
        }
    }
}
