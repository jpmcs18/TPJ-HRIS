using ProcessLayer.Helpers;

namespace ReportLayer.Helpers
{
    public class PrintVesselMovementHelper : ReportHelperBase
    {

        private static PrintVesselMovementHelper instance;
        public static PrintVesselMovementHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new PrintVesselMovementHelper();
                return instance;
            }
        }

        public PrintVesselMovementHelper() : base("VesselMovement")
        {
            VesselNameCell = Get(nameof(VesselNameCell)).ToString();
            DateCell = Get(nameof(DateCell)).ToString();
            StartRow = Get(nameof(StartRow)).ToInt();
            DateColumn = Get(nameof(DateColumn)).ToInt();
            MovementColumn = Get(nameof(MovementColumn)).ToInt();
            PlaceColumn = Get(nameof(PlaceColumn)).ToInt();
        }

        public string VesselNameCell { get; set; }
        public string DateCell { get; set; }
        public int StartRow { get; set; }
        public int DateColumn { get; set; }
        public int MovementColumn { get; set; }
        public int PlaceColumn { get; set; }

    }
}
