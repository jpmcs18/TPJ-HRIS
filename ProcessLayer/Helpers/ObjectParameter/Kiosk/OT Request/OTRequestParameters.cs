namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.OT_Request
{
    public class OTRequestParameters : KioskParametersBase
    {
        public OTRequestParameters() { }
        private static OTRequestParameters _instance;
        public static OTRequestParameters Instance { get { if (_instance == null) _instance = new OTRequestParameters(); return _instance; } }

        public string StartDate = "@StartDate";
        public string EndDate = "@EndDate";
        public string RequestDate = "@RequestDate";
        public string IsOffice = "@IsOffice";
    }
}
