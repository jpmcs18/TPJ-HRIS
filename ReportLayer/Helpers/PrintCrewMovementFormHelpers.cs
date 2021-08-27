using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportLayer.Helpers
{
    public class PrintCrewMovementFormHelper : ReportHelperBase
    {
        public static readonly Lazy<PrintCrewMovementFormHelper> Instance = new Lazy<PrintCrewMovementFormHelper>(() => new PrintCrewMovementFormHelper());
        private PrintCrewMovementFormHelper() : base("CrewMovementForm")
        {
            TransactionNoCell = Get(nameof(TransactionNoCell)).ToString();
            CrewNameCell = Get(nameof(CrewNameCell)).ToString();
            DateTimeCell = Get(nameof(DateTimeCell)).ToString();
            CurrentDepartmentCell = Get(nameof(CurrentDepartmentCell)).ToString();
            CurrentPositionCell = Get(nameof(CurrentPositionCell)).ToString();
            CurrentVesselCell = Get(nameof(CurrentVesselCell)).ToString();
            CurrentSNPositionCell = Get(nameof(CurrentSNPositionCell)).ToString();
            CurrentSNVesselCell = Get(nameof(CurrentSNVesselCell)).ToString();
            PreviousDepartmentCell = Get(nameof(PreviousDepartmentCell)).ToString();
            PreviousPositionCell = Get(nameof(PreviousPositionCell)).ToString();
            PreviousVesselCell = Get(nameof(PreviousVesselCell)).ToString();
            PreviousSNPositionCell = Get(nameof(PreviousSNPositionCell)).ToString();
            PreviousSNVesselCell = Get(nameof(PreviousSNVesselCell)).ToString();
            RemarksCell = Get(nameof(RemarksCell)).ToString();
            PreparedByCell = Get(nameof(PreparedByCell)).ToString();
            CheckedByCell = Get(nameof(CheckedByCell)).ToString();
        }

        public string TransactionNoCell { get; }
        public string CrewNameCell { get; }
        public string DateTimeCell { get; }
        public string CurrentDepartmentCell { get; }
        public string CurrentPositionCell { get; } 
        public string CurrentVesselCell { get; }
        public string CurrentSNPositionCell { get; }
        public string CurrentSNVesselCell { get; }
        public string PreviousDepartmentCell { get; } 
        public string PreviousPositionCell { get; }
        public string PreviousVesselCell { get; } 
        public string PreviousSNPositionCell { get; }
        public string PreviousSNVesselCell { get; }
        public string RemarksCell { get; }
        public string PreparedByCell { get; }
        public string CheckedByCell { get; }

    }
}
