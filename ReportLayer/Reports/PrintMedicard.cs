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
            WriteToCell(PrintMedicardHelper.Instance.NameCell, LeaveRequest._Personnel.FullName);
            WriteToCell(PrintMedicardHelper.Instance.DateCell, LeaveRequest.CreatedOn?.ToString("MM/dd/yyyy"));
            WriteToCell(PrintMedicardHelper.Instance.DepartmentCell, department._Department.Description);
            WriteToCell(PrintMedicardHelper.Instance.PositionCell, position._Position.Description);
            WriteToCell(PrintMedicardHelper.Instance.HospitalCell, LeaveRequest.Hospital);
            WriteToCell(PrintMedicardHelper.Instance.LocationCell, LeaveRequest.Location);

            if (LeaveRequest.IsAbsent ?? false)
            {
                WriteToCell(PrintMedicardHelper.Instance.IsAbsentCell, "X");
                WriteToCell(PrintMedicardHelper.Instance.AbsentDateCell, LeaveRequest.RequestedDate?.ToString("MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.AbsentPeriodCell, LeaveRequest.PeriodStart?.ToString("MM/dd/yyyy") + LeaveRequest.PeriodEnd?.ToString("-MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.AbsentNoofDaysCell, LeaveRequest.NoofDays?.ToString("0.##"));
            }

            if (LeaveRequest.IsHalfDay ?? false)
            {
                WriteToCell(PrintMedicardHelper.Instance.IsHalfdayCell, "X");
                WriteToCell(PrintMedicardHelper.Instance.HalfdayDateCell, LeaveRequest.RequestedDate?.ToString("MM/dd/yyyy"));
                WriteToCell(PrintMedicardHelper.Instance.HalfdayMorningCell, (LeaveRequest.IsMorning ?? false) ? "X" : "");
                WriteToCell(PrintMedicardHelper.Instance.HalfdayAfternoonCell, (LeaveRequest.IsAfternoon ?? false) ? "X" : "");
            }

            string reason1 = LeaveRequest.Reasons.Length > 120 ? LeaveRequest.Reasons.Substring(0, 120) : LeaveRequest.Reasons;
            string reason2 = LeaveRequest.Reasons.Replace(reason1, "");

            WriteToCell(PrintMedicardHelper.Instance.ReasonsCell, reason1);
            WriteToCell(PrintMedicardHelper.Instance.Reasons2Cell, reason2);
        }
    }
}
