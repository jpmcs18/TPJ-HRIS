using ProcessLayer.Helpers;
using System;

namespace ReportLayer.Helpers
{
    public sealed class PrintCrewListHelper : ReportHelperBase
    {
        public static readonly Lazy<PrintCrewListHelper> Instance = new Lazy<PrintCrewListHelper>(() => new PrintCrewListHelper());

        private PrintCrewListHelper() : base("Crewlist")
        {
            DateRangeCell = Get(nameof(DateRangeCell)).ToString();
            VesselCell = Get(nameof(VesselCell)).ToString();
            StartRow = Get(nameof(StartRow)).ToInt();
            NumberColumn = Get(nameof(NumberColumn)).ToInt();
            NameColumn = Get(nameof(NameColumn)).ToInt();
            PositionColumn = Get(nameof(PositionColumn)).ToInt();
            OnBoardColumn = Get(nameof(OnBoardColumn)).ToInt();
            FromPositionColumn = Get(nameof(FromPositionColumn)).ToInt();
            FromVesselColumn = Get(nameof(FromVesselColumn)).ToInt();
            FromDateColumn = Get(nameof(FromDateColumn)).ToInt();
            ToPositionColumn = Get(nameof(ToPositionColumn)).ToInt();
            ToVesselColumn = Get(nameof(ToVesselColumn)).ToInt();
            ToDateColumn = Get(nameof(ToDateColumn)).ToInt();
            DisembarkedColumn = Get(nameof(DisembarkedColumn)).ToInt();
            ReferenceColumn = Get(nameof(ReferenceColumn)).ToInt();
        }

        public string DateRangeCell { get; }
        public string VesselCell { get; }
        public int StartRow { get; }
        public int NumberColumn { get; } 
        public int NameColumn { get; }
        public int PositionColumn { get; }
        public int OnBoardColumn { get; } 
        public int FromPositionColumn { get; } 
        public int FromVesselColumn { get; } 
        public int FromDateColumn { get; } 
        public int ToPositionColumn { get; } 
        public int ToVesselColumn { get; } 
        public int ToDateColumn { get; } 
        public int DisembarkedColumn { get; } 
        public int ReferenceColumn { get; } 

    }
}
