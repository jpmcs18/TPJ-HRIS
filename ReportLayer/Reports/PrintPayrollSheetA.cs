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
            CellBorder(Row, 0, Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetAHelper.Instance.ColumnRate)
                .WriteToCell("SUB TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, TotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnOT, TotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, TotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigLoan, TotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSS, TotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnProvidentFund, TotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSSLoan, TotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnWithholdingTax, TotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth, TotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, TotalNetpay.ToString("N2"));
            Row++;

            ResetTotal();

        }
        private void WriteGrandTotal()
        {
            CellBorder(Row, 0, Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, MultipleBorders.Horizontal, LineStyle.Medium);

            MergeCell(Row, 0, Row, PrintPayrollSheetAHelper.Instance.ColumnRate)
                .WriteToCell("TOTAL AMOUNT :")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);

            GetRange(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetFontWeight(700);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, GrandTotalBasicPay.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnOT, GrandTotalOT.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, GrandTotalPagibigFund.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigLoan, GrandTotalPagibigLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSS, GrandTotalSSS.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnProvidentFund, GrandTotalProvident.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSSLoan, GrandTotalSSSLoan.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnWithholdingTax, GrandTotalTax.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth, GrandTotalPhilHealth.ToString("N2"));
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, GrandTotalNetpay.ToString("N2"));
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
            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNo, Row, PrintPayrollSheetAHelper.Instance.ColumnNoOfDays)
                .WriteToCell("DEPARTMENT: OFFICE")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnRate, Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount)
                .WriteToCell($"FOR THE PERIOD FROM: {GetPeriod()}")
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);

            Row++;
            GetRange(Row, PrintPayrollSheetAHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetBorder(MultipleBorders.All)
                .SetFontWeight(700)
                .SetBackgroud(SpreadsheetColor.FromArgb(230, 240, 240));

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNo, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnEmployeeName)
                .WriteToCell("NAME OF EMPLOYEE");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnDesignation, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnDesignation)
                .WriteToCell("DESG");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNoOfDays, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnNoOfDays)
                .WriteToCell("No. of Days")
                .SetWrapText();

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnRate, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnRate)
                .WriteToCell("RATE");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnSalary)
                .WriteToCell("SALARY");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnOT, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnOT)
                .WriteToCell("OT");

            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth)
                .WriteToCell("DEDUCTIONS");
           
            MergeCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, Row + 1, PrintPayrollSheetAHelper.Instance.ColumnNetAmount)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center)
                .SetVerticalAlignment(VerticalAlignmentStyle.Center)
                .WriteToCell("NET AMOUNT");

            Row++;

            GetRange(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth)
                .SetWrapText()
                .SetFontSize(130)
                .SetFontWeight(100);
            SetRowHeight(Row, 0.7, LengthUnit.Centimeter);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, "Pag-IBIG Fund");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigLoan, "Pag-IBIG Loan");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSS, "SSS");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSSLoan, "SSS Loan");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnProvidentFund, "SSS (MPF)");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnWithholdingTax, "Withholding Tax");
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth, "PhilHealth Contribution");

            Row++;
        }

        private void WriteDetails(Payroll payroll, int index)
        {
            decimal noofdays = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => x.RegularDay).ToDecimalPlaces(3);
            decimal basicpay = payroll.PayrollDetails.Where(x => !x.IsHazard).Sum(x => (payroll.DailyRate * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2);
            SetRowHeight(Row, 0.5);

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNo, index.ToString())
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right)
                .SetItalic();

            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnEmployeeName, payroll.FullName)
                .SetWrapText()
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Left);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnDesignation, payroll.Position)
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Center);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnRate, payroll.DailyRate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, basicpay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnOT, payroll.TotalOTPay.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigFund, payroll.PagibigFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPagibigLoan, (payroll.PagibigLoan + payroll.PagibigCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSS, payroll.SSS.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSSSLoan, (payroll.SalaryLoan + payroll.SSSCalamityLoan).ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnProvidentFund, payroll.ProvidentFund.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnWithholdingTax, payroll.Tax.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnPhilHealth, payroll.PhilHealth.ToString("N2"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
            WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNetAmount, payroll.NetPay.ToString("N2"))
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

                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnNoOfDays, noofdays.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnRate, dailyrate.ToString("N3"))
                .SetHorizontalAlignment(HorizontalAlignmentStyle.Right);
                WriteToCell(Row, PrintPayrollSheetAHelper.Instance.ColumnSalary, basicpay.ToString("N2"))
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
