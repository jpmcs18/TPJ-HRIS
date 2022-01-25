using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintCrewPayslipHelper : ReportHelperBase
    {
        public static readonly PrintCrewPayslipHelper Instance = new Lazy<PrintCrewPayslipHelper>(() => new PrintCrewPayslipHelper()).Value;

        private PrintCrewPayslipHelper() : base("CrewPayslip")
        {
            SignatoryCell = Get(nameof(SignatoryCell)).ToString();
            EmployeeNameCell = Get(nameof(EmployeeNameCell)).ToString();
            TINNoCell = Get(nameof(TINNoCell)).ToString();
            HDMFNoCell = Get(nameof(HDMFNoCell)).ToString();
            SSSNoCell = Get(nameof(SSSNoCell)).ToString();
            BasicStartRow = Get(nameof(BasicStartRow)).ToInt();
            BasicMaxRow = Get(nameof(BasicMaxRow)).ToInt();
            PayrollPeriodColumn = Get(nameof(PayrollPeriodColumn)).ToInt();
            VesselColumn = Get(nameof(VesselColumn)).ToInt();
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
            PagibigCell = Get(nameof(PagibigCell)).ToString();
            PagibigLoanCell = Get(nameof(PagibigLoanCell)).ToString();
            SSSCell = Get(nameof(SSSCell)).ToString();
            ProvidentFundCell = Get(nameof(ProvidentFundCell)).ToString();
            SSSLoanCell = Get(nameof(SSSLoanCell)).ToString();
            TaxCell = Get(nameof(TaxCell)).ToString();
            PhilHealthCell = Get(nameof(PhilHealthCell)).ToString();
            ValeCell = Get(nameof(ValeCell)).ToString();
            TotalDedCell = Get(nameof(TotalDedCell)).ToString();
            NetCell = Get(nameof(NetCell)).ToString();
        }

        public string SignatoryCell { get; set; }
        public string EmployeeNameCell { get; set; }
        public string TINNoCell { get; set; }
        public string HDMFNoCell { get; set; }
        public string SSSNoCell { get; set; }
        public int BasicStartRow { get; set; }
        public int BasicMaxRow { get; set; }
        public int PayrollPeriodColumn { get; set; }
        public int VesselColumn { get; set; }
        public int PositionColumn { get; set; }
        public int DaysColumn { get; set; }
        public int RateColumn { get; set; }
        public int AmountColumn { get; set; }
        public string BasicTotalDaysCell { get; set; }
        public string BasicSalaryCell { get; set; }
        public int OTStartRow { get; set; }
        public int OTDescColumn { get; set; }
        public int OTHoursColumn { get; set; }
        public int OTRateColumn { get; set; }
        public int OTPayColumn { get; set; }
        public int OTMaxRow { get; set; }
        public string TotalOTPayCell { get; set; }
        public string PagibigCell { get; set; }
        public string PagibigLoanCell { get; set; }
        public string SSSCell { get; set; }
        public string ProvidentFundCell { get; set; }
        public string SSSLoanCell { get; set; }
        public string TaxCell { get; set; }
        public string PhilHealthCell { get; set; }
        public string ValeCell { get; set; }
        public string SalaryLoanCell { get; set; }
        public string TotalDedCell { get; set; }
        public string NetCell { get; set; }

    }
}
