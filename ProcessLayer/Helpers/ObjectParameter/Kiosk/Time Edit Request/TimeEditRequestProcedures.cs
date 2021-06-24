namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Time_Edit_Request
{
    public class TimeEditRequestProcedures
    {
        private static TimeEditRequestProcedures _instance;
        public static TimeEditRequestProcedures Instance { get { if (_instance == null) _instance = new TimeEditRequestProcedures(); return _instance; } }

        public string Get = "kiosk.GetTimeEditRequest";
        public string Filter = "kiosk.FilterTimeEditRequest";
        public string FilterApproving = "kiosk.FilterApprovingTimeEditRequest";
        public string CreateOrUpdate = "kiosk.CreateOrUpdateTimeEditRequest";
        public string Approve = "kiosk.ApprovedTimeEditRequest";
        public string Cancel = "kiosk.CancelTimeEditRequest";
        public string Delete = "kiosk.DeleteTimeEditRequest";
        public string GetApprovedTimeEdit = "kiosk.GetApprovedTimeEditRequest";
    }
}
