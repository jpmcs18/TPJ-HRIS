using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Processes;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;

namespace ReportLayer.Reports
{
    public class PrintMedicard : SpreadSheetReportBase
    {
        public PrintMedicard(string template) : base(template)
        {
        }

        public LeaveRequest LeaveRequest { get; set; }
        public override void GenerateReport()
        {
            base.GenerateReport();

            PersonnelDepartment department = PersonnelDepartmentProcess.GetCurrentDepartment(LeaveRequest._Personnel.ID, LeaveRequest.CreatedOn ?? LeaveRequest.RequestedDate ?? DateTime.Now);
            PersonnelPosition position = PersonnelPositionProcess.GetCurrentPosition(LeaveRequest._Personnel.ID, LeaveRequest.CreatedOn ?? LeaveRequest.RequestedDate ?? DateTime.Now);
            WriteToCell(PrintMedicardHelper.Instance.Value.NameCell, LeaveRequest._Personnel.FullName);
            WriteToCell(PrintMedicardHelper.Instance.Value.DateCell, LeaveRequest.CreatedOn?.ToString("MM/dd/yyyy"));
            WriteToCell(PrintMedicardHelper.Instance.Value.DepartmentCell, department._Department.Description);
            WriteToCell(PrintMedicardHelper.Instance.Value.PositionCell, position._Position.Description);
            WriteToCell(PrintMedicardHelper.Instance.Value.HospitalCell, LeaveRequest.Hospital);
            WriteToCell(PrintMedicardHelper.Instance.Value.LocationCell, LeaveRequest.Location);

            if (LeaveRequest.IsAbsent ?? false)
            {
                WriteToCell(PrintMedicardHelper.Instance.Value.IsAbsentCell, "X");
                WriteToCell(PrintMedicardHelper.Instance.Value.AbsentDateCell, LeaveRequest.RequestedDate?.ToString("MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.Value.AbsentPeriodCell, LeaveRequest.PeriodStart?.ToString("MM/dd/yyyy") + LeaveRequest.PeriodEnd?.ToString("-MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.Value.AbsentNoofDaysCell, LeaveRequest.NoofDays?.ToString("0.##"));
            }

            if (LeaveRequest.IsHalfDay ?? false)
            {
                WriteToCell(PrintMedicardHelper.Instance.Value.IsHalfdayCell, "X");
                WriteToCell(PrintMedicardHelper.Instance.Value.HalfdayDateCell, LeaveRequest.RequestedDate?.ToString("MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.Value.HalfdayMorningCell, (LeaveRequest.IsMorning ?? false) ? "X" : "");
                WriteToCell(PrintMedicardHelper.Instance.Value.HalfdayAfternoonCell, (LeaveRequest.IsAfternoon ?? false) ? "X" : "");
            }

            string reason1 = LeaveRequest.Reasons.Length > 120 ? LeaveRequest.Reasons.Substring(0, 120) : LeaveRequest.Reasons;
            string reason2 = LeaveRequest.Reasons.Replace(reason1, "");

            WriteToCell(PrintMedicardHelper.Instance.Value.ReasonsCell, reason1);
            WriteToCell(PrintMedicardHelper.Instance.Value.Reasons2Cell, reason2);
        }
    }
}
