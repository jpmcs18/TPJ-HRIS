using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintPayslipHelper : ReportHelperBase
    {
        public static readonly Lazy<PrintPayslipHelper> Instance = new Lazy<PrintPayslipHelper>(() => new PrintPayslipHelper());

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

            RegOTHoursCell = Get(nameof(RegOTHoursCell)).ToString();
            RegOTRateCell = Get(nameof(RegOTRateCell)).ToString();
            RegOTPayCell = Get(nameof(RegOTPayCell)).ToString();

            SundayOTHoursCell = Get(nameof(SundayOTHoursCell)).ToString();
            SundayOTRateCell = Get(nameof(SundayOTRateCell)).ToString();
            SundayOTPayCell = Get(nameof(SundayOTPayCell)).ToString();

            HolidayRegDayCell = Get(nameof(HolidayRegDayCell)).ToString();
            HolidayRegRateCell = Get(nameof(HolidayRegRateCell)).ToString();
            HolidayRegPayCell = Get(nameof(HolidayRegPayCell)).ToString();

            HolidayExcessOTHoursCell = Get(nameof(HolidayExcessOTHoursCell)).ToString();
            HolidayExcessOTRateCell = Get(nameof(HolidayExcessOTRateCell)).ToString();
            HolidayExcessOTPayCell = Get(nameof(HolidayExcessOTPayCell)).ToString();
            TotalOTPayCell = Get(nameof(TotalOTPayCell)).ToString();
            GrossSalaryCell = Get(nameof(GrossSalaryCell)).ToString();

            AddRow = Get(nameof(AddRow)).ToInt();

            AddNoofDaysColumn = Get(nameof(AddNoofDaysColumn)).ToInt();
            AddPayColumn = Get(nameof(AddPayColumn)).ToInt();
            AddPayDescColumn = Get(nameof(AddPayDescColumn)).ToInt();
            AddRateColumn = Get(nameof(AddRateColumn)).ToInt();

            AddTotalPayCell = Get(nameof(AddTotalPayCell)).ToString();
            AllowanceStartRow = Get(nameof(AllowanceStartRow)).ToInt();
            AllowanceMaxRow = Get(nameof(AllowanceMaxRow)).ToInt();

            AllRegOTHoursCell = Get(nameof(AllRegOTHoursCell)).ToString();
            AllRegOTRateCell = Get(nameof(AllRegOTRateCell)).ToString();
            AllRegOTPayCell = Get(nameof(AllRegOTPayCell)).ToString();
            AllSundayOTHoursCell = Get(nameof(AllSundayOTHoursCell)).ToString();
            AllSundayOTRateCell = Get(nameof(AllSundayOTRateCell)).ToString();
            AllSundayOTPayCell = Get(nameof(AllSundayOTPayCell)).ToString();
            AllHolidayRegDayCell = Get(nameof(AllHolidayRegDayCell)).ToString();
            AllHolidayRegRateCell = Get(nameof(AllHolidayRegRateCell)).ToString();
            AllHolidayRegPayCell = Get(nameof(AllHolidayRegPayCell)).ToString();
            AllHolidayExcessOTHoursCell = Get(nameof(AllHolidayExcessOTHoursCell)).ToString();
            AllHolidayExcessOTRateCell = Get(nameof(AllHolidayExcessOTRateCell)).ToString();
            AllHolidayExcessOTPayCell = Get(nameof(AllHolidayExcessOTPayCell)).ToString();
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
        public string RegOTHoursCell { get; set; }
        public string RegOTRateCell { get; set; }
        public string RegOTPayCell { get; set; }
        public string SundayOTHoursCell { get; set; }
        public string SundayOTRateCell { get; set; }
        public string SundayOTPayCell { get; set; }
        public string HolidayRegDayCell { get; set; }
        public string HolidayRegRateCell { get; set; }
        public string HolidayRegPayCell { get; set; }
        public string HolidayExcessOTHoursCell { get; set; }
        public string HolidayExcessOTRateCell { get; set; }
        public string HolidayExcessOTPayCell { get; set; }
        public string NightDiffHoursCell { get; set; }
        public string NightDiffRateCell { get; set; }
        public string NightDiffPayCell { get; set; }
        public string TotalOTPayCell { get; set; }
        public string GrossSalaryCell { get; set; }
        public int AddRow { get; set; }
        public int AddPayDescColumn { get; set; }
        public int AddNoofDaysColumn { get; set; }
        public int AddRateColumn { get; set; }
        public int AddPayColumn { get; set; }
        public string AddTotalPayCell { get; set; }
        public string AllRegOTHoursCell { get; set; }
        public string AllRegOTRateCell { get; set; }
        public string AllRegOTPayCell { get; set; }
        public string AllSundayOTHoursCell { get; set; }
        public string AllSundayOTRateCell { get; set; }
        public string AllSundayOTPayCell { get; set; }
        public string AllHolidayRegDayCell { get; set; }
        public string AllHolidayRegRateCell { get; set; }
        public string AllHolidayRegPayCell { get; set; }
        public string AllHolidayExcessOTHoursCell { get; set; }
        public string AllHolidayExcessOTRateCell { get; set; }
        public string AllHolidayExcessOTPayCell { get; set; }
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
