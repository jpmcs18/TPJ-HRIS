using ProcessLayer.Computation.CnB;
using ProcessLayer.Entities.CnBs;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportLayer.Reports
{
    public class PrintTimeSheet : SpreadSheetReportBase
    {
        public PrintTimeSheet(string template) : base(template)
        {
        }

        public long? PersonnelID { get; set; }
        public int? DepartmentID { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public override void GenerateReport()
        {
            var TimeSheets = TimelogComputation.Instance.Value.GenerateTimesheet(Start, End, PersonnelID, DepartmentID);
            if (TimeSheets?.Any() ?? false)
            {
                int cnt = 0;
                foreach (var sheet in TimeSheets)
                {
                        DuplicateSheet((cnt++).ToString());
                        GenerateTimeSheet(sheet);
                }
                DeleteSheet();
            }
            else
                throw new Exception("Nothing to print");
            base.GenerateReport();
        }

        private void GenerateTimeSheet(PersonnelTimesheet timesheet)
        {
            WriteToCell(PrintTimeSheetHelper.Instance.Value.CutoffCell, GetPeriod());
            WriteToCell(PrintTimeSheetHelper.Instance.Value.NameCell, timesheet.Personnel.FullName);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalNumberOfDaysCell, timesheet.TotalNoofDays);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalRegulatOTCell, timesheet.TotalRegOT);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalSundayOTCell, timesheet.TotalSunOT);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalHolidayCell, timesheet.TotalHoliday);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalHolidayExcessCell, timesheet.TotalHolExc);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.TotalHazardCell, timesheet.TotalHigRisk);
            WriteToCell(PrintTimeSheetHelper.Instance.Value.HazardTitleCell, $"HIGH RISK ({(int)(timesheet.HighRiskRate * 100)}%)");

            int startrow = PrintTimeSheetHelper.Instance.Value.StartRow - 1;
            foreach(var timelog in timesheet.ComputedTimelogs)
            {
                if (timelog.Date.DayOfWeek == DayOfWeek.Sunday)
                    SetBackgroundColor(startrow, PrintTimeSheetHelper.Instance.Value.DateColumn - 1, startrow, PrintTimeSheetHelper.Instance.Value.HazardColumn, GemBox.Spreadsheet.SpreadsheetColor.FromArgb(220, 220, 220));

                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.DateColumn, timelog.Date.ToString("dd-MMM-yy-ddd"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.AssignedColumn, timelog.Assigned);
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.AMInColumn, timelog.Login?.ToString("hh:mm tt"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.PMOutColumn, timelog.Login == timelog.Logout ? null : timelog.Logout?.ToString("hh:mm tt"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.NoofDaysColumn, timelog.NoofDays > 0 ? timelog.NoofDays.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.RegularOTColumn, timelog.RegOTHours > 0 ? timelog.RegOTHours.ToString("N2") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.SundayOTColumn, timelog.SunOTHours > 0 ? timelog.SunOTHours.ToString("N2") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.HolidayColumn, timelog.Holiday > 0 ? timelog.Holiday.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.HolidayExcessColumn, timelog.HolExcHours > 0 ? timelog.HolExcHours.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.HazardColumn, timelog.HighRiskHours > 0 ? timelog.HighRiskHours.ToString("N2") : "");

                if(!string.IsNullOrEmpty(timelog.HolidayDesc))
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.OutInColumn, timelog.HolidayDesc);
                else if (!string.IsNullOrEmpty(timelog.LeaveDesc))
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.OutInColumn, timelog.LeaveDesc);
                else
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.Value.OutInColumn, timelog.Schedule.Description);
                startrow++;
            }
        }

        private string GetPeriod()
        {
            if (Start.Month == End.Month)
                return $"{Start:MMM dd} - {End:dd yyyy}";
            else
            {
                if (Start.Year == End.Year)
                    return $"{Start:MMM dd} - {End:MMM dd yyyy}";
                else
                    return $"{Start:MMM dd yyyy} - {End:MMM dd yyyy}";
            }
        }
    }
}
