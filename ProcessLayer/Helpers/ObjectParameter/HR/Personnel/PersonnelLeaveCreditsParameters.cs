namespace ProcessLayer.Helpers.ObjectParameter.Personnel
{
    public class PersonnelLeaveCreditsParameters
    {
        private static PersonnelLeaveCreditsParameters _instance;
        public static PersonnelLeaveCreditsParameters Instance
        {
            get
            {
                if (_instance == null) _instance = new PersonnelLeaveCreditsParameters();
                return _instance;
            }
        }

        public string ID = "@ID";
        public string PersonnelID = "@PersonnelID";
        public string LeaveTypeID = "@LeaveTypeID";
        public string LeaveCredits = "@LeaveCredits";
        public string YearValid = "@YearValid";
    }
}
