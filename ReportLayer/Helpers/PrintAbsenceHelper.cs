using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintAbsenceHelper : ReportHelperBase
    {
        public static readonly PrintAbsenceHelper Instance = new PrintAbsenceHelper();
        private PrintAbsenceHelper() : base("Absence")
        {
            NameCell = Get(nameof(NameCell)).ToString();
            DepartmentCell = Get(nameof(DepartmentCell)).ToString();
            DateCell = Get(nameof(DateCell)).ToString();
            PositionCell = Get(nameof(PositionCell)).ToString();
            IsAbsentCell = Get(nameof(IsAbsentCell)).ToString();
            IsHalfdayCell = Get(nameof(IsHalfdayCell)).ToString();
            IsUndertimeCell = Get(nameof(IsUndertimeCell)).ToString();
            AbsentDateCell = Get(nameof(AbsentDateCell)).ToString();
            AbsentNoofDaysCell = Get(nameof(AbsentNoofDaysCell)).ToString();
            HalfdayDateCell = Get(nameof(HalfdayDateCell)).ToString();
            HalfdayMorningCell = Get(nameof(HalfdayMorningCell)).ToString();
            HalfdayAfternoonCell = Get(nameof(HalfdayAfternoonCell)).ToString();
            UndertimeDateCell = Get(nameof(UndertimeDateCell)).ToString();
            UndertimeTimeCell = Get(nameof(UndertimeTimeCell)).ToString();
            ReasonsCell = Get(nameof(ReasonsCell)).ToString();
            Reasons2Cell = Get(nameof(Reasons2Cell)).ToString();
            ApprovedByCell = Get(nameof(ApprovedByCell)).ToString();
            NotedByCell = Get(nameof(NotedByCell)).ToString();
        }
        public string NameCell { get; set; }
        public string DepartmentCell { get; set; }
        public string DateCell { get; set; }
        public string PositionCell { get; set; }
        public string IsAbsentCell { get; set; }
        public string IsHalfdayCell { get; set; }
        public string IsUndertimeCell { get; set; }
        public string AbsentDateCell { get; set; }
        public string AbsentNoofDaysCell { get; set; }
        public string HalfdayDateCell { get; set; }
        public string HalfdayMorningCell { get; set; }
        public string HalfdayAfternoonCell { get; set; }
        public string UndertimeDateCell { get; set; }
        public string UndertimeTimeCell { get; set; }
        public string ReasonsCell { get; set; }
        public string Reasons2Cell { get; set; }
        public string ApprovedByCell { get; set; }
        public string NotedByCell { get; set; }

    }
}
