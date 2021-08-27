using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintPayrollSheetBHelper : ReportHelperBase
    {
        public static readonly Lazy<PrintPayrollSheetBHelper> Instance = new Lazy<PrintPayrollSheetBHelper>(() => new PrintPayrollSheetBHelper());
        private  PrintPayrollSheetBHelper() : base("PayrollSheetB")
        {
            EndPageRow = Get(nameof(EndPageRow)).ToInt();
            MaxItem = Get(nameof(MaxItem)).ToInt();
            ColumnMaxLength = Get(nameof(ColumnMaxLength)).ToInt();
            ColumnMiddle = Get(nameof(ColumnMiddle)).ToInt();
            ColumnNo = Get(nameof(ColumnNo)).ToInt();
            ColumnEmployeeName = Get(nameof(ColumnEmployeeName)).ToInt();
            ColumnDesignation = Get(nameof(ColumnDesignation)).ToInt();
            ColumnNoOfDays = Get(nameof(ColumnNoOfDays)).ToInt();
            ColumnRate = Get(nameof(ColumnRate)).ToInt();
            ColumnSalary = Get(nameof(ColumnSalary)).ToInt();
            ColumnAllowance = Get(nameof(ColumnAllowance)).ToInt();
            ColumnOT = Get(nameof(ColumnOT)).ToInt();
            ColumnAdditionalPay = Get(nameof(ColumnAdditionalPay)).ToInt();
            ColumnPagibigFund = Get(nameof(ColumnPagibigFund)).ToInt();
            ColumnPagibigLoan = Get(nameof(ColumnPagibigLoan)).ToInt();
            ColumnSSS = Get(nameof(ColumnSSS)).ToInt();
            ColumnSSSLoan = Get(nameof(ColumnSSSLoan)).ToInt();
            ColumnProvidentFund = Get(nameof(ColumnProvidentFund)).ToInt();
            ColumnPhilHealth = Get(nameof(ColumnPhilHealth)).ToInt();
            ColumnWithholdingTax = Get(nameof(ColumnWithholdingTax)).ToInt();
            ColumnVale = Get(nameof(ColumnVale)).ToInt();
            ColumnOutstandingVale = Get(nameof(ColumnOutstandingVale)).ToInt();
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
        public int ColumnAllowance { get; }
        public int ColumnOT { get; }
        public int ColumnAdditionalPay { get; }
        public int ColumnPagibigFund { get; }
        public int ColumnPagibigLoan { get; }
        public int ColumnSSS { get; }
        public int ColumnProvidentFund { get; }
        public int ColumnSSSLoan { get; }
        public int ColumnPhilHealth { get; }
        public int ColumnWithholdingTax { get; }
        public int ColumnVale { get; }
        public int ColumnOutstandingVale { get; }
        public int ColumnNetAmount { get; }
        public int ColumnPreparedByStart { get; }
        public int ColumnPreparedByEnd { get; }
        public int ColumnCheckedByStart { get; }
        public int ColumnCheckedByEnd { get; }
        public int ColumnApprovedByStart { get; }
        public int ColumnApprovedByEnd { get; }

    }
}
