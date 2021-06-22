using ProcessLayer.Computation.CnB;
using ProcessLayer.Entities.CnBs;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportLayer.Reports
{
    public sealed class PrintTimeSheet : SpreadSheetReportBase
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
            var TimeSheets = TimelogComputation.Instance.GenerateTimesheet(Start, End, PersonnelID, DepartmentID);
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
            WriteToCell(PrintTimeSheetHelper.Instance.CutoffCell, GetPeriod());
            WriteToCell(PrintTimeSheetHelper.Instance.NameCell, timesheet.Personnel.FullName);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalNumberOfDaysCell, timesheet.TotalNoofDays);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalRegulatOTCell, timesheet.TotalRegOT);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalSundayOTCell, timesheet.TotalSunOT);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalHolidayCell, timesheet.TotalHoliday);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalHolidayExcessCell, timesheet.TotalHolExc);
            WriteToCell(PrintTimeSheetHelper.Instance.TotalHazardCell, timesheet.TotalHigRisk);
            WriteToCell(PrintTimeSheetHelper.Instance.HazardTitleCell, $"HIGH RISK ({(int)(timesheet.HighRiskRate * 100)}%)");

            int startrow = PrintTimeSheetHelper.Instance.StartRow;
            foreach(var timelog in timesheet.ComputedTimelogs)
            {
                if (timelog.Date.DayOfWeek == DayOfWeek.Sunday)
                    SetBackgroundColor(startrow, PrintTimeSheetHelper.Instance.DateColumn - 1, startrow, PrintTimeSheetHelper.Instance.HazardColumn, GemBox.Spreadsheet.SpreadsheetColor.FromArgb(220, 220, 220));

                WriteToCell(startrow, PrintTimeSheetHelper.Instance.DateColumn, timelog.Date.ToString("dd-MMM-yy-ddd"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.AssignedColumn, timelog.Assigned);
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.AMInColumn, timelog.Login?.ToString("hh:mm tt"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.PMOutColumn, timelog.Logout?.ToString("hh:mm tt"));
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.NoofDaysColumn, timelog.NoofDays > 0 ? timelog.NoofDays.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.RegularOTColumn, timelog.RegOTHours > 0 ? timelog.RegOTHours.ToString("N2") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.SundayOTColumn, timelog.SunOTHours > 0 ? timelog.SunOTHours.ToString("N2") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.HolidayColumn, timelog.Holiday > 0 ? timelog.Holiday.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.HolidayExcessColumn, timelog.HolExcHours > 0 ? timelog.HolExcHours.ToString("N3") : "");
                WriteToCell(startrow, PrintTimeSheetHelper.Instance.HazardColumn, timelog.HighRiskHours > 0 ? timelog.HighRiskHours.ToString("N2") : "");

                if(!string.IsNullOrEmpty(timelog.HolidayDesc))
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.OutInColumn, timelog.HolidayDesc);
                else if (!string.IsNullOrEmpty(timelog.LeaveDesc))
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.OutInColumn, timelog.LeaveDesc);
                else
                    WriteToCell(startrow, PrintTimeSheetHelper.Instance.OutInColumn, timelog.Schedule.Description);
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
