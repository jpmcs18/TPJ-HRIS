namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request
{
    public class LeaveRequestFields : KioskFieldsBase
    {
        private static LeaveRequestFields _instance;
        public static LeaveRequestFields Instance { get { if (_instance == null) _instance = new LeaveRequestFields(); return _instance; } }

        public string LeaveTypeID = "Leave Type ID";
        public string StartDateTime = "Start Date Time";
        public string EndDateTime = "End Date Time";
        public string ApprovedLeaveCredits = "Approved Leave Credits";
        public string File = "File";
    }
}
