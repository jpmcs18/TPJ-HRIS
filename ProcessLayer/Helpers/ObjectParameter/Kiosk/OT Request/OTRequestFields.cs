namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.OT_Request
{

    public class OTRequestFields : KioskFieldsBase
    {
        private static OTRequestFields _instance;
        public static OTRequestFields Instance { get { if (_instance == null) _instance = new OTRequestFields(); return _instance; } }

        public string RequestDate = "Request Date";
        public string StartDateTime = "Start Date Time";
        public string EndDateTime = "End Date Time";
        public string IsOffice = "Is Office";
        public string IsEarlyOT = "Is Early OT";
    }
}
