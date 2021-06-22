using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Time_Edit_Request
{
    public class TimeEditRequestFields : KioskFieldsBase
    {
        private static TimeEditRequestFields _instance;
        public static TimeEditRequestFields Instance { get { if (_instance == null) _instance = new TimeEditRequestFields(); return _instance; } }
        
        public string LoginDateTime = "Login Date Time";
        public string LogoutDateTime = "Logout Date Time";
    }
}
