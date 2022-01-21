using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintReportHelper : ReportHelperBase
    {
        public static readonly PrintReportHelper Instance = new PrintReportHelper();

        private PrintReportHelper() : base("Report")
        {
            TitleCell = Get(nameof(TitleCell)).ToString();
            MonthYearCell = Get(nameof(MonthYearCell)).ToString();
            AmountCell = Get(nameof(AmountCell)).ToString();
            SumCell = Get(nameof(SumCell)).ToString();
            PSCell = Get(nameof(PSCell)).ToString();
            ESCell = Get(nameof(ESCell)).ToString();
            ECCell = Get(nameof(ECCell)).ToString();
            StartRow = Get(nameof(StartRow)).ToInt();
            NameColumn = Get(nameof(NameColumn)).ToInt();
            AmountColumn = Get(nameof(AmountColumn)).ToInt();
            SumColumn = Get(nameof(SumColumn)).ToInt();
            PSColumn = Get(nameof(PSColumn)).ToInt();
            ESColumn = Get(nameof(ESColumn)).ToInt();
            ECColumn = Get(nameof(ECColumn)).ToInt();
        }

        public string TitleCell { get; set; }
        public string MonthYearCell { get; set; }
        public string AmountCell { get; set; }
        public string SumCell { get; set; }
        public string PSCell { get; set; }
        public string ESCell { get; set; }
        public string ECCell { get; set; }
        public int StartRow { get; set; }
        public int NameColumn { get; set; }
        public int AmountColumn { get; set; }
        public int SumColumn { get; set; }
        public int PSColumn { get; set; }
        public int ESColumn { get; set; }
        public int ECColumn { get; set; }
    }
}
