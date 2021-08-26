using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintTimeSheetHelper : ReportHelperBase
    {
        public static readonly Lazy<PrintTimeSheetHelper> Instance = new Lazy<PrintTimeSheetHelper>(() => new PrintTimeSheetHelper());
        private PrintTimeSheetHelper() : base("TimeSheet") {
            CutoffCell = Get(nameof(CutoffCell)).ToString();
            NameCell = Get(nameof(NameCell)).ToString();
            ScheduleCell = Get(nameof(ScheduleCell)).ToString();
            TotalNumberOfDaysCell = Get(nameof(TotalNumberOfDaysCell)).ToString();
            TotalRegulatOTCell = Get(nameof(TotalRegulatOTCell)).ToString();
            TotalSundayOTCell = Get(nameof(TotalSundayOTCell)).ToString();
            TotalHolidayCell = Get(nameof(TotalHolidayCell)).ToString();
            TotalHolidayExcessCell = Get(nameof(TotalHolidayExcessCell)).ToString();
            TotalHazardCell = Get(nameof(TotalHazardCell)).ToString();
            HazardTitleCell = Get(nameof(HazardTitleCell)).ToString();
            StartRow = Get(nameof(StartRow)).ToInt();
            DateColumn = Get(nameof(DateColumn)).ToInt();
            AssignedColumn = Get(nameof(AssignedColumn)).ToInt();
            AMInColumn = Get(nameof(AMInColumn)).ToInt();
            OutInColumn = Get(nameof(OutInColumn)).ToInt();
            PMOutColumn = Get(nameof(PMOutColumn)).ToInt();
            NoofDaysColumn = Get(nameof(NoofDaysColumn)).ToInt();
            RegularOTColumn = Get(nameof(RegularOTColumn)).ToInt();
            SundayOTColumn = Get(nameof(SundayOTColumn)).ToInt();
            HolidayColumn = Get(nameof(HolidayColumn)).ToInt();
            HolidayExcessColumn = Get(nameof(HolidayExcessColumn)).ToInt();
            HazardColumn = Get(nameof(HazardColumn)).ToInt();

        }

        public string CutoffCell { get; set; }
        public string NameCell { get; set; }
        public string ScheduleCell { get; set; }
        public string TotalNumberOfDaysCell { get; set; }
        public string TotalRegulatOTCell { get; set; }
        public string TotalSundayOTCell { get; set; }
        public string TotalHolidayCell { get; set; }
        public string TotalHolidayExcessCell { get; set; }
        public string TotalHazardCell { get; set; }
        public string HazardTitleCell { get; set; }
        public int StartRow { get; set; }
        public int DateColumn { get; set; }
        public int AssignedColumn { get; set; }
        public int AMInColumn { get; set; }
        public int OutInColumn { get; set; }
        public int PMOutColumn { get; set; }
        public int NoofDaysColumn { get; set; }
        public int RegularOTColumn { get; set; }
        public int SundayOTColumn { get; set; }
        public int HolidayColumn { get; set; }
        public int HolidayExcessColumn { get; set; }
        public int HazardColumn { get; set; }
    }
}
