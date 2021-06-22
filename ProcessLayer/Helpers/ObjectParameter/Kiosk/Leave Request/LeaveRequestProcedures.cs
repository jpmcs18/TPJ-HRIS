namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request
{
    public class LeaveRequestProcedures
    {
        private static LeaveRequestProcedures _instance;
        public static LeaveRequestProcedures Instance { get { if (_instance == null) _instance = new LeaveRequestProcedures(); return _instance; } }

        public string Get = "kiosk.GetLeaveRequest";
        public string Filter = "kiosk.FilterLeaveRequest";
        public string FilterApproving = "kiosk.FilterApprovingLeaveRequest";
        public string FilterRequestThatNeedDocument = "kiosk.FilterLeaveRequestThatNeedDocument";
        public string CreateOrUpdate = "kiosk.CreateOrUpdateLeaveRequest";
        public string Approve = "kiosk.ApprovedLeaveRequest";
        public string Cancel = "kiosk.CancelLeaveRequest";
        public string Delete = "kiosk.DeleteLeaveRequest";
        public string GetApprovedLeave = "kiosk.GetApprovedLeaveRequest";
        public string UploadDocument = "kiosk.UploadDocumentForLeave";
    }
}
