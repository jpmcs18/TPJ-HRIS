using GemBox.Spreadsheet;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ReportLayer.Reports
{
    public class PrintCrewlist : SpreadSheetReportBase
    {
        public PrintCrewlist(string template) : base(template)
        {
        }

        public List<CrewDetails> Crews { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public Vessel Vessel { get; set; }
        public override void GenerateReport()
        {
            base.GenerateReport();

            var row = PrintCrewListHelper.Instance.Value.StartRow;
            var number = "";
            var isnonumberstart = false;
            int cnt = 0;
            int foreachcounter = 1;
            int largestcount = 0;
            var daterange = "Crew List (" + StartingDate?.ToString("MMMM dd") + " - " + EndingDate?.ToString("MMMM dd, yyyy") + ")";
            var str = "";
            WriteToCell(PrintCrewListHelper.Instance.Value.VesselCell, Vessel?.Description);
            WriteToCell(PrintCrewListHelper.Instance.Value.DateRangeCell, daterange);


            if (Crews != null && Crews.Any())
            {
                Crews.ForEach(crew =>
                {
                    if (crew.FromCrew?.VesselID == null && crew.FromCrew?.SNVesselID == null)
                    {
                        number = (++cnt).ToString().PadLeft(2, '0');
                        WriteToCell(row, PrintCrewListHelper.Instance.Value.NumberColumn, number);
                    }
                    else
                    {
                        if (!isnonumberstart)
                        {
                            isnonumberstart = true;
                            row++;
                        }
                    }
                    str = "";
                    foreachcounter = 0;
                    largestcount = 0;
                    crew.Crew?._Personnel?.FullName?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 22 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.NameColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });

                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.NameColumn, str);

                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;
                    foreachcounter = 0;
                    str = "";
                    (crew.Crew?._Position?.Display ?? crew.Crew?._SNPosition?.Display)?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 8 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.PositionColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });
                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.PositionColumn, str);

                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;

                    WriteToCell(row, PrintCrewListHelper.Instance.Value.OnBoardColumn, crew.Crew?.OnboardDate?.ToString("MM/dd/yyyy"));

                    foreachcounter = 0;
                    str = "";
                    (crew.FromCrew?._Position?.Display ?? crew.FromCrew?._SNPosition?.Display)?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 8 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.FromPositionColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });
                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.FromPositionColumn, str);

                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;
                    foreachcounter = 0;
                    str = "";
                    (crew.FromCrew?._Vessel?.Description ?? crew.FromCrew?._SNVessel?.Description)?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 15 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.FromVesselColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });
                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;
                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.FromVesselColumn, str);

                    WriteToCell(row, PrintCrewListHelper.Instance.Value.FromDateColumn, crew.FromCrew?.OnboardDate?.ToString("MM/dd/yyyy HHmm"));

                    foreachcounter = 0;
                    str = "";
                    (crew.ToCrew?._Position?.Display ?? crew.ToCrew?._SNPosition?.Display)?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 8 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.ToPositionColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });

                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.ToPositionColumn, str);
                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;
                    foreachcounter = 0;
                    str = "";
                    (crew.ToCrew?._Vessel?.Description ?? crew.ToCrew?._SNVessel?.Description)?.Split(' ').ToList().ForEach(d =>
                    {
                        if ((str + (string.IsNullOrEmpty(str) ? "" : " ") + d).Length > 15 && !string.IsNullOrEmpty(str))
                        {
                            WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.ToVesselColumn, str);
                            foreachcounter++;
                            str = d;
                        }
                        else
                        {
                            str += (string.IsNullOrEmpty(str) ? "" : " ") + d;
                        }
                    });
                    WriteToCell(row + foreachcounter, PrintCrewListHelper.Instance.Value.ToVesselColumn, str);
                    largestcount = largestcount > foreachcounter ? largestcount : foreachcounter;
                    WriteToCell(row, PrintCrewListHelper.Instance.Value.ToDateColumn, crew.ToCrew?.OnboardDate?.ToString("MM/dd/yyyy HHmm"));
                    WriteToCell(row, PrintCrewListHelper.Instance.Value.DisembarkedColumn, crew.Disembarked?.ToString("MM/dd/yyyy"));
                    WriteToCell(row, PrintCrewListHelper.Instance.Value.ReferenceColumn, crew.Reference);
                    row = row + largestcount + 1;
                });
            }
        }
    }
}
