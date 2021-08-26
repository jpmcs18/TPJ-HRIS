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

            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.TransactionNoCell, CrewMovement.TransactionNo);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CrewNameCell, CrewMovement._Personnel?.FullName);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.DateTimeCell, CrewMovement.OnboardDate?.ToString("MM-dd-yyyy @ hh:mm tt"));
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CurrentDepartmentCell, CrewMovement._Department?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CurrentPositionCell, CrewMovement._Position?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CurrentVesselCell, CrewMovement._Vessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CurrentSNPositionCell, CrewMovement._SNPosition?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CurrentSNVesselCell, CrewMovement._SNVessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreviousDepartmentCell, CrewMovement._PreviousCrewMovement?._Department.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreviousPositionCell, CrewMovement._PreviousCrewMovement?._Position?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreviousVesselCell, CrewMovement._PreviousCrewMovement?._Vessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreviousSNPositionCell, CrewMovement._PreviousCrewMovement?._SNPosition?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreviousSNVesselCell, CrewMovement._PreviousCrewMovement?._SNVessel?.Description);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.RemarksCell, CrewMovement.Remarks);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.PreparedByCell, CrewMovement._Creator);
            WriteToCell(PrintCrewMovementFormHelper.Instance.Value.CheckedByCell, CrewMovement._Check);
        }

        public override void Dispose()
        {
            CrewMovement = null;
            base.Dispose();
        }
    }
}
