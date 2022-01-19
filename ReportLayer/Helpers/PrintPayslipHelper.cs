using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintPayslipHelper : ReportHelperBase
    {
        public static readonly PrintPayslipHelper Instance = new PrintPayslipHelper();

        private PrintPayslipHelper() : base("Payslip")
        {
            SignatoryCell = Get(nameof(SignatoryCell)).ToString();
            EmployeeNameCell = Get(nameof(EmployeeNameCell)).ToString();
            TINNoCell = Get(nameof(TINNoCell)).ToString();
            HDMFNoCell = Get(nameof(HDMFNoCell)).ToString();
            SSSNoCell = Get(nameof(SSSNoCell)).ToString();
            EmployeeNumberCell = Get(nameof(EmployeeNumberCell)).ToString();
            BasicStartRow = Get(nameof(BasicStartRow)).ToInt();
            BasicMaxRow = Get(nameof(BasicMaxRow)).ToInt();
            PayrollPeriodColumn = Get(nameof(PayrollPeriodColumn)).ToInt();

            DepartmentColumn = Get(nameof(DepartmentColumn)).ToInt();
            PositionColumn = Get(nameof(PositionColumn)).ToInt();
            DaysColumn = Get(nameof(DaysColumn)).ToInt();
            RateColumn = Get(nameof(RateColumn)).ToInt();
            AmountColumn = Get(nameof(AmountColumn)).ToInt();
            BasicTotalDaysCell = Get(nameof(BasicTotalDaysCell)).ToString();
            BasicSalaryCell = Get(nameof(BasicSalaryCell)).ToString();
            OTStartRow = Get(nameof(OTStartRow)).ToInt();
            OTDescColumn = Get(nameof(OTDescColumn)).ToInt();
            OTHoursColumn = Get(nameof(OTHoursColumn)).ToInt();
            OTRateColumn = Get(nameof(OTRateColumn)).ToInt();
            OTPayColumn = Get(nameof(OTPayColumn)).ToInt();
            OTMaxRow = Get(nameof(OTMaxRow)).ToInt();


            TotalOTPayCell = Get(nameof(TotalOTPayCell)).ToString();
            GrossSalaryCell = Get(nameof(GrossSalaryCell)).ToString();

            AddStartRow = Get(nameof(AddStartRow)).ToInt();

            AddNoofDaysColumn = Get(nameof(AddNoofDaysColumn)).ToInt();
            AddPayColumn = Get(nameof(AddPayColumn)).ToInt();
            AddPayDescColumn = Get(nameof(AddPayDescColumn)).ToInt();
            AddRateColumn = Get(nameof(AddRateColumn)).ToInt();

            AddTotalPayCell = Get(nameof(AddTotalPayCell)).ToString();
            AllowanceStartRow = Get(nameof(AllowanceStartRow)).ToInt();
            AllowanceMaxRow = Get(nameof(AllowanceMaxRow)).ToInt();

            AllStartRow = Get(nameof(AllStartRow)).ToInt();
            AllDescColumn = Get(nameof(AllDescColumn)).ToInt();
            AllHoursColumn = Get(nameof(AllHoursColumn)).ToInt();
            AllRateColumn = Get(nameof(AllRateColumn)).ToInt();
            AllPayColumn = Get(nameof(AllPayColumn)).ToInt();
            AllMaxRow = Get(nameof(AllMaxRow)).ToInt();

            AllowanceTotalDaysCell = Get(nameof(AllowanceTotalDaysCell)).ToString();
            AllowanceSalaryCell = Get(nameof(AllowanceSalaryCell)).ToString();

            TotalAllOTPayCell = Get(nameof(TotalAllOTPayCell)).ToString();
            SSSCell = Get(nameof(SSSCell)).ToString();
            ProvidentFundCell = Get(nameof(ProvidentFundCell)).ToString();
            PhilHealthCell = Get(nameof(PhilHealthCell)).ToString();
            SalaryLoanCell = Get(nameof(SalaryLoanCell)).ToString();
            PagibigLoanCell = Get(nameof(PagibigLoanCell)).ToString();
            PagibigCell = Get(nameof(PagibigCell)).ToString();
            LoanStartRow = Get(nameof(LoanStartRow)).ToInt();
            TaxCell = Get(nameof(TaxCell)).ToString();
            OtherChargesCell = Get(nameof(OtherChargesCell)).ToString();
            ValeCell = Get(nameof(ValeCell)).ToString();
            TotalDedCell = Get(nameof(TotalDedCell)).ToString();
            NetCell = Get(nameof(NetCell)).ToString();
        }

        public string SignatoryCell { get; set; }
        public string EmployeeNameCell { get; set; }
        public string EmployeeNumberCell { get; set; }
        public string TINNoCell { get; set; }
        public string HDMFNoCell { get; set; }
        public string SSSNoCell { get; set; }
        public int BasicStartRow { get; set; }
        public int BasicMaxRow { get; set; }
        public int AllowanceStartRow { get; set; }
        public int AllowanceMaxRow { get; set; }
        public int PayrollPeriodColumn { get; set; }
        public int DepartmentColumn { get; set; }
        public int PositionColumn { get; set; }
        public int DaysColumn { get; set; }
        public int RateColumn { get; set; }
        public int AmountColumn { get; set; }
        public string BasicTotalDaysCell { get; set; }
        public string BasicSalaryCell { get; set; }
        public string AllowanceTotalDaysCell { get; set; }
        public string AllowanceSalaryCell { get; set; }
        public int OTStartRow { get; set; }
        public int OTDescColumn { get; set; }
        public int OTHoursColumn { get; set; }
        public int OTRateColumn { get; set; }
        public int OTPayColumn { get; set; }
        public int OTMaxRow { get; set; }
        public string TotalOTPayCell { get; set; }
        public string GrossSalaryCell { get; set; }
        public int AddStartRow { get; set; }
        public int AddPayDescColumn { get; set; }
        public int AddNoofDaysColumn { get; set; }
        public int AddRateColumn { get; set; }
        public int AddPayColumn { get; set; }
        public string AddTotalPayCell { get; set; }
        public int AllStartRow { get; set; }
        public int AllDescColumn { get; set; }
        public int AllHoursColumn { get; set; }
        public int AllRateColumn { get; set; }
        public int AllPayColumn { get; set; }
        public int AllMaxRow { get; set; }
        public string TotalAllOTPayCell { get; set; }
        public string SSSCell { get; set; }
        public string ProvidentFundCell { get; set; }
        public string PhilHealthCell { get; set; }
        public string SalaryLoanCell { get; set; }
        public string PagibigLoanCell { get; set; }
        public int LoanStartRow { get; set; }
        public string PagibigCell { get; set; }
        public string TaxCell { get; set; }
        public string OtherChargesCell { get; set; }
        public string ValeCell { get; set; }
        public string TotalDedCell { get; set; }
        public string NetCell { get; set; }

    }
}
