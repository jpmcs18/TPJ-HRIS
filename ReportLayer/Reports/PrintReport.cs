using ProcessLayer.Entities;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Processes.Reports;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportLayer.Reports
{
    public class PrintReport : SpreadSheetReportBase
    {
        public PrintReport(string template) : base(template)
        {
        }
        public ReportType ReportType { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public override void GenerateReport()
        {
            switch (ReportType)
            {
                case ReportType.SALARY:
                    GenerateSalaryReport();
                    break;
                case ReportType.TAX:
                    GenerateTaxReport();
                    break;
                case ReportType.HDMFLOAN:
                    GenerateHDMFLoanReport();
                    break;
                case ReportType.SSSLOAN:
                    GenerateSSSLoanReport();
                    break;
                case ReportType.PHILHEALTH:
                    GeneratePhilHealthReport();
                    break;
                case ReportType.SSSPREMIUM:
                    GenerateSSSPremiumReport();
                    break;
                case ReportType.HDMFPREMIUM:
                    GenerateHDMFPremiumReport();
                    break;
                default:
                    GenerateTaxReport();
                    GenerateHDMFLoanReport();
                    GenerateSSSLoanReport();
                    GeneratePhilHealthReport();
                    GenerateHDMFPremiumReport();
                    GenerateSSSPremiumReport();
                    GenerateSalaryReport();
                    break;
            }
            DeleteSheet();
        }

        private void GenerateSalaryReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var ms = ReportProcess.Instance.Value.GetList(ReportType.SALARY, Year, Month);
            if (!(ms?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("Monthly Salary");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "Monthly Salary");
            WriteToCell(PrintReportHelper.Instance.Value.AmountCell, "Gross Salary");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + ms.Count, 2, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in ms)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.AmountColumn, details.Amount.ToString("N2"));
                row++;
            }
        }

        private void GenerateSSSPremiumReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var sssp = ReportProcess.Instance.Value.GetList(ReportType.SSSPREMIUM, Year, Month);
            if (!(sssp?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("SSS Premium");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "SSS Contribution Report");
            WriteToCell(PrintReportHelper.Instance.Value.PSCell, "SSS Employee Share");
            WriteToCell(PrintReportHelper.Instance.Value.ESCell, "SSS Employer Share");
            WriteToCell(PrintReportHelper.Instance.Value.ECCell, "EC");
            WriteToCell(PrintReportHelper.Instance.Value.SumCell, "Total SSS Contribution");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + sssp.Count, 5, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in sssp)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.PSColumn, details.PS.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ESColumn, details.ES.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ECColumn, details.EC.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.SumColumn, details.Sum.ToString("N2"));
                row++;
            }
        }

        private void GenerateHDMFPremiumReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var hdmfp = ReportProcess.Instance.Value.GetList(ReportType.HDMFPREMIUM, Year, Month);
            if (!(hdmfp?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("HDMF Premium");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "Pag Ibig Contribution Report");
            WriteToCell(PrintReportHelper.Instance.Value.PSCell, "HDMF Employee");
            WriteToCell(PrintReportHelper.Instance.Value.ESCell, "HDMF Employer");
            WriteToCell(PrintReportHelper.Instance.Value.ECCell, "Total HDMF Contribution");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + hdmfp.Count, 4, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in hdmfp)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.PSColumn, details.PS.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ESColumn, details.ES.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ECColumn, details.Sum.ToString("N2"));
                row++;
            }
        }

        private void GeneratePhilHealthReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var ph = ReportProcess.Instance.Value.GetList(ReportType.PHILHEALTH, Year, Month);
            if (!(ph?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("PhilHealth");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "PhilHealth Contribution Report");
            WriteToCell(PrintReportHelper.Instance.Value.PSCell, "PhilHealth Employee Share");
            WriteToCell(PrintReportHelper.Instance.Value.ESCell, "PhilHealth Employer Share");
            WriteToCell(PrintReportHelper.Instance.Value.ECCell, "Total PhilHealth Contribution");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + ph.Count, 4, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in ph)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.PSColumn, details.PS.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ESColumn, details.ES.ToString("N2"));
                WriteToCell(row, PrintReportHelper.Instance.Value.ECColumn, details.Sum.ToString("N2"));
                row++;
            }
        }

        private void GenerateSSSLoanReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var sssl = ReportProcess.Instance.Value.GetList(ReportType.SSSLOAN, Year, Month);
            if (!(sssl?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("SSS Loan");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "SSS Loan Report");
            WriteToCell(PrintReportHelper.Instance.Value.AmountCell, "Amount of Loan");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + sssl.Count, 2, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);


            foreach (var details in sssl)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.AmountColumn, details.Amount.ToString("N2"));
                row++;
            }
        }

        private void GenerateHDMFLoanReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var hdmfl = ReportProcess.Instance.Value.GetList(ReportType.HDMFLOAN, Year, Month);
            if (!(hdmfl?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("HDMF Loan");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "Pag Ibig Loan Report");
            WriteToCell(PrintReportHelper.Instance.Value.AmountCell, "Amount of Loan");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + hdmfl.Count, 2, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in hdmfl)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.AmountColumn, details.Amount.ToString("N2"));
                row++;
            }
        }

        private void GenerateTaxReport()
        {
            var row = PrintReportHelper.Instance.Value.StartRow;
            var tax = ReportProcess.Instance.Value.GetList(ReportType.TAX, Year, Month);
            if (!(tax?.Any() ?? false)) throw new Exception("No data found");

            DuplicateSheet("TAX");
            WriteToCell(PrintReportHelper.Instance.Value.TitleCell, "Witholding Tax Report");
            WriteToCell(PrintReportHelper.Instance.Value.AmountCell, "Witholding Tax");
            WriteToCell(PrintReportHelper.Instance.Value.MonthYearCell, $"For The Month of {new DateTime(Year, Month, 1):MMMM yyyy}");

            CellBorder(row - 1, 0, row + tax.Count, 2, GemBox.Spreadsheet.MultipleBorders.All, GemBox.Spreadsheet.LineStyle.Thin);

            foreach (var details in tax)
            {
                WriteToCell(row, PrintReportHelper.Instance.Value.NameColumn, details.FullName);
                WriteToCell(row, PrintReportHelper.Instance.Value.AmountColumn, details.Amount.ToString("N2"));
                row++;
            }
        }
    }
}
