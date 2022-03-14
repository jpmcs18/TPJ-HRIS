using ProcessLayer.Entities;
using ReportLayer.Bases;
using ReportLayer.Helpers;
using System;
using System.Collections.Generic;

namespace ReportLayer.Reports
{
    public class PrintVesselMovement : SpreadSheetReportBase
    {
        public PrintVesselMovement(string template) : base(template)
        {
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Vessel Vessel { get; set; }
        public List<VesselMovement> VesselMovements { get; set; }
        public override void GenerateReport()
        {
            base.GenerateReport();

            WriteToCell(PrintVesselMovementHelper.Instance.DateCell, StartDate.ToString("MMMM dd, yyyy") + " - " + EndDate.ToString("MMMM dd, yyyy"));
            WriteToCell(PrintVesselMovementHelper.Instance.VesselNameCell, Vessel.Description);
            //var startRow = PrintVesselMovementHelper.Instance.StartRow;
            //foreach(var movement in VesselMovements)
            //{
            //    WriteToCell(startRow, PrintVesselMovementHelper.Instance.DateColumn, movement.MovementDate.ToString("MMMM dd, yyyy"));
            //    WriteToCell(startRow, PrintVesselMovementHelper.Instance.MovementColumn, movement._VesselMovementType.Description);
            //    startRow++;
            //}
        }
    }
}
