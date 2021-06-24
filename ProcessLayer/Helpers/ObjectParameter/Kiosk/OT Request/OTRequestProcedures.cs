namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.OT_Request
{
    public class OTRequestProcedures
    {
        private static OTRequestProcedures _instance;
        public static OTRequestProcedures Instance { get { if (_instance == null) _instance = new OTRequestProcedures(); return _instance; } }

        public string Get = "kiosk.GetOTRequest";
        public string Filter = "kiosk.FilterOTRequest";
        public string FiltrApproving = "kiosk.FilterApprovingOTRequest";
        public string CreateOrUpdate = "kiosk.CreateOrUpdateOTRequest";
        public string Approve = "kiosk.ApprovedOTRequest";
        public string Cancel = "kiosk.CancelOTRequest";
        public string Delete = "kiosk.DeleteOTRequest";
        public string GetApprovedOT = "kiosk.GetApprovedOTRequest";
    }
}
