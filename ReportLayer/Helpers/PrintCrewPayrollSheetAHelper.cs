using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintCrewPayrollSheetAHelper : ReportHelperBase
    {
        public static readonly PrintCrewPayrollSheetAHelper Instance = new Lazy<PrintCrewPayrollSheetAHelper>(() => new PrintCrewPayrollSheetAHelper()).Value;
        private PrintCrewPayrollSheetAHelper() : base("CrewPayrollSheetA")
        {
            EndPageRow = Get(nameof(EndPageRow)).ToInt();
            MaxItem = Get(nameof(MaxItem)).ToInt();
            ColumnMaxLength = Get(nameof(ColumnMaxLength)).ToInt();
            ColumnNo = Get(nameof(ColumnNo)).ToInt();
            ColumnEmployeeName = Get(nameof(ColumnEmployeeName)).ToInt();
            ColumnPosition = Get(nameof(ColumnPosition)).ToInt();
            ColumnNoOfDays = Get(nameof(ColumnNoOfDays)).ToInt();
            ColumnRate = Get(nameof(ColumnRate)).ToInt();
            ColumnSalary = Get(nameof(ColumnSalary)).ToInt();
            ColumnPagibigFund = Get(nameof(ColumnPagibigFund)).ToInt();
            ColumnPagibigLoan = Get(nameof(ColumnPagibigLoan)).ToInt();
            ColumnSSS = Get(nameof(ColumnSSS)).ToInt();
            ColumnProvidentFund = Get(nameof(ColumnProvidentFund)).ToInt();
            ColumnSSSLoan = Get(nameof(ColumnSSSLoan)).ToInt();
            ColumnWithholdingTax = Get(nameof(ColumnWithholdingTax)).ToInt();
            ColumnPhilHealth = Get(nameof(ColumnPhilHealth)).ToInt();
            ColumnVale = Get(nameof(ColumnVale)).ToInt();
            ColumnNetAmount = Get(nameof(ColumnNetAmount)).ToInt();
            ColumnPreparedByStart = Get(nameof(ColumnPreparedByStart)).ToInt();
            ColumnPreparedByEnd = Get(nameof(ColumnPreparedByEnd)).ToInt();
            ColumnCheckedByStart = Get(nameof(ColumnCheckedByStart)).ToInt();
            ColumnCheckedByEnd = Get(nameof(ColumnCheckedByEnd)).ToInt();
            ColumnApprovedByStart = Get(nameof(ColumnApprovedByStart)).ToInt();
            ColumnApprovedByEnd = Get(nameof(ColumnApprovedByEnd)).ToInt();
        }

        public int EndPageRow { get; }
        public int MaxItem { get; }
        public int ColumnMaxLength { get; }
        public int ColumnMiddle { get; }
        public int ColumnNo { get; }
        public int ColumnEmployeeName { get; }
        public int ColumnPosition { get; }
        public int ColumnNoOfDays { get; }
        public int ColumnRate { get; }
        public int ColumnSalary { get; }
        public int ColumnPagibigFund { get; }
        public int ColumnPagibigLoan { get; }
        public int ColumnSSS { get; }
        public int ColumnProvidentFund { get; }
        public int ColumnSSSLoan { get; }
        public int ColumnPhilHealth { get; }
        public int ColumnWithholdingTax { get; }
        public int ColumnVale { get; }
        public int ColumnNetAmount { get; }
        public int ColumnPreparedByStart { get; }
        public int ColumnPreparedByEnd { get; }
        public int ColumnCheckedByStart { get; }
        public int ColumnCheckedByEnd { get; }
        public int ColumnApprovedByStart { get; }
        public int ColumnApprovedByEnd { get; }

    }
}
