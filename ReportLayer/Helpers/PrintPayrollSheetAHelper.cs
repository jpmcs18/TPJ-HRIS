using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintPayrollSheetAHelper : ReportHelperBase
    {
        public static readonly PrintPayrollSheetAHelper Instance = new PrintPayrollSheetAHelper();
        private PrintPayrollSheetAHelper() : base("PayrollSheetA")
        {
            MaxItem = Get(nameof(MaxItem)).ToInt();
            EndPageRow = Get(nameof(EndPageRow)).ToInt();
            ColumnMaxLength = Get(nameof(ColumnMaxLength)).ToInt();
            ColumnMiddle = Get(nameof(ColumnMiddle)).ToInt();
            ColumnNo = Get(nameof(ColumnNo)).ToInt();
            ColumnEmployeeName = Get(nameof(ColumnEmployeeName)).ToInt();
            ColumnDesignation = Get(nameof(ColumnDesignation)).ToInt();
            ColumnNoOfDays = Get(nameof(ColumnNoOfDays)).ToInt();
            ColumnRate = Get(nameof(ColumnRate)).ToInt();
            ColumnSalary = Get(nameof(ColumnSalary)).ToInt();
            ColumnOT = Get(nameof(ColumnOT)).ToInt();
            ColumnPagibigFund = Get(nameof(ColumnPagibigFund)).ToInt();
            ColumnPagibigLoan = Get(nameof(ColumnPagibigLoan)).ToInt();
            ColumnSSS = Get(nameof(ColumnSSS)).ToInt();
            ColumnSSSLoan = Get(nameof(ColumnSSSLoan)).ToInt();
            ColumnProvidentFund = Get(nameof(ColumnProvidentFund)).ToInt();
            ColumnPhilHealth = Get(nameof(ColumnPhilHealth)).ToInt();
            ColumnWithholdingTax = Get(nameof(ColumnWithholdingTax)).ToInt();
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
        public int ColumnDesignation { get; }
        public int ColumnNoOfDays { get; }
        public int ColumnRate { get; }
        public int ColumnSalary { get; }
        public int ColumnOT { get; }
        public int ColumnPagibigFund { get; }
        public int ColumnPagibigLoan { get; }
        public int ColumnSSS { get; }
        public int ColumnSSSLoan { get; }
        public int ColumnProvidentFund { get; }
        public int ColumnPhilHealth { get; }
        public int ColumnWithholdingTax { get; }
        public int ColumnNetAmount { get; }
        public int ColumnPreparedByStart { get; }
        public int ColumnPreparedByEnd { get; }
        public int ColumnCheckedByStart { get; }
        public int ColumnCheckedByEnd { get; }
        public int ColumnApprovedByStart { get; }
        public int ColumnApprovedByEnd { get; }

    }
}
