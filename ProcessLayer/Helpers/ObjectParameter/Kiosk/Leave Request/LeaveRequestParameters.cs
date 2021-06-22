namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request
{
    public class LeaveRequestParameters : KioskParametersBase
    {
        public LeaveRequestParameters() { }
        private static LeaveRequestParameters _instance;
        public static LeaveRequestParameters Instance { get { if (_instance == null) _instance = new LeaveRequestParameters(); return _instance; } }

        public string LeaveTypeID = "@LeaveTypeID";
        public string StartDateTime = "@StartDateTime";
        public string EndDateTime = "@EndDateTime";
        public string BulkUse = "@BulkUse";
    }
}
