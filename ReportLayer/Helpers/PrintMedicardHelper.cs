using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintMedicardHelper : ReportHelperBase
    {
        public static readonly PrintMedicardHelper Instance = new PrintMedicardHelper();
        private PrintMedicardHelper() : base("Medicard")
        {
            NameCell = Get(nameof(NameCell)).ToString();
            DepartmentCell = Get(nameof(DepartmentCell)).ToString();
            DateCell = Get(nameof(DateCell)).ToString();
            PositionCell = Get(nameof(PositionCell)).ToString();
            HospitalCell = Get(nameof(HospitalCell)).ToString();
            LocationCell = Get(nameof(LocationCell)).ToString();
            IsAbsentCell = Get(nameof(IsAbsentCell)).ToString();
            IsHalfdayCell = Get(nameof(IsHalfdayCell)).ToString();
            AbsentDateCell = Get(nameof(AbsentDateCell)).ToString();
            AbsentPeriodCell = Get(nameof(AbsentPeriodCell)).ToString();
            AbsentNoofDaysCell = Get(nameof(AbsentNoofDaysCell)).ToString();
            HalfdayDateCell = Get(nameof(HalfdayDateCell)).ToString();
            HalfdayMorningCell = Get(nameof(HalfdayMorningCell)).ToString();
            HalfdayAfternoonCell = Get(nameof(HalfdayAfternoonCell)).ToString();
            ReasonsCell = Get(nameof(ReasonsCell)).ToString();
            Reasons2Cell = Get(nameof(Reasons2Cell)).ToString();
            ApprovedByCell = Get(nameof(ApprovedByCell)).ToString();
            NotedByCell = Get(nameof(NotedByCell)).ToString();
        }
        public string NameCell { get; set; }
        public string DepartmentCell { get; set; }
        public string DateCell { get; set; }
        public string PositionCell { get; set; }
        public string HospitalCell { get; set; }
        public string LocationCell { get; set; }
        public string IsAbsentCell { get; set; }
        public string IsHalfdayCell { get; set; }
        public string AbsentDateCell { get; set; }
        public string AbsentPeriodCell { get; set; }
        public string AbsentNoofDaysCell { get; set; }
        public string HalfdayDateCell { get; set; }
        public string HalfdayMorningCell { get; set; }
        public string HalfdayAfternoonCell { get; set; }
        public string ReasonsCell { get; set; }
        public string Reasons2Cell { get; set; }
        public string ApprovedByCell { get; set; }
        public string NotedByCell { get; set; }

    }
}
