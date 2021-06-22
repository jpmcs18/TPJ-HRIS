namespace ProcessLayer.Helpers.ObjectParameter.Personnel
{
    public class PersonnelLeaveCreditsFields
    {
        private static PersonnelLeaveCreditsFields _instance;
        public static PersonnelLeaveCreditsFields Instance
        {
            get
            {
                if (_instance == null) _instance = new PersonnelLeaveCreditsFields();
                return _instance;
            }
        }
        
        public string ID = "ID";
        public string PersonnelID = "Personnel ID";
        public string LeaveTypeID = "Leave Type ID";
        public string LeaveCredits = "Leave Credits";
        public string YearValid = "Year Valid";
    }
}
