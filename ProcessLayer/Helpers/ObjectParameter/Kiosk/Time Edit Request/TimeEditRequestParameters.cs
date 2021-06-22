namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Time_Edit_Request
{
    public class TimeEditRequestParameters : KioskParametersBase
    {
        public TimeEditRequestParameters() { }
        private static TimeEditRequestParameters _instance;
        public static TimeEditRequestParameters Instance { get { if (_instance == null) _instance = new TimeEditRequestParameters(); return _instance; } }
        
        public string LoginDateTime = "@LoginDateTime";
        public string LogoutDateTime = "@LogoutDateTime";
    }
}
