using GemBox.Spreadsheet;
using ProcessLayer.Entities;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLayer.Reports
{
    public class PrintCrewMovementForm : SpreadSheetReportBase
    {
        public PrintCrewMovementForm(string template) : base(template)
        {
        }

        public CrewMovement CrewMovement { get; set; }

        public override void GenerateReport()
        {
            base.GenerateReport();

            WriteToCell(PrintCrewMovementFormHelper.Instance.TransactionNoCell, CrewMovement.TransactionNo);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CrewNameCell, CrewMovement._Personnel?.FullName);
            WriteToCell(PrintCrewMovementFormHelper.Instance.DateTimeCell, CrewMovement.OnboardDate?.ToString("MM-dd-yyyy @ hh:mm tt"));
            WriteToCell(PrintCrewMovementFormHelper.Instance.CurrentDepartmentCell, CrewMovement._Department?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CurrentPositionCell, CrewMovement._Position?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CurrentVesselCell, CrewMovement._Vessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CurrentSNPositionCell, CrewMovement._SNPosition?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CurrentSNVesselCell, CrewMovement._SNVessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreviousDepartmentCell, CrewMovement._PreviousCrewMovement?._Department.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreviousPositionCell, CrewMovement._PreviousCrewMovement?._Position?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreviousVesselCell, CrewMovement._PreviousCrewMovement?._Vessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreviousSNPositionCell, CrewMovement._PreviousCrewMovement?._SNPosition?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreviousSNVesselCell, CrewMovement._PreviousCrewMovement?._SNVessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.RemarksCell, CrewMovement.Remarks);
            WriteToCell(PrintCrewMovementFormHelper.Instance.PreparedByCell, CrewMovement._Creator);
            WriteToCell(PrintCrewMovementFormHelper.Instance.CheckedByCell, CrewMovement._Check);
        }

        public override void Dispose()
        {
            CrewMovement = null;
            base.Dispose();
        }
    }
}
