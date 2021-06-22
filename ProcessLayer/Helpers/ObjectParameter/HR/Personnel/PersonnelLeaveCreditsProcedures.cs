namespace ProcessLayer.Helpers.ObjectParameter.Personnel
{
    public class PersonnelLeaveCreditsProcedures
    {
        private static PersonnelLeaveCreditsProcedures _instance;
        public static PersonnelLeaveCreditsProcedures Instance
        {
            get
            {
                if (_instance == null) _instance = new PersonnelLeaveCreditsProcedures();
                return _instance;
            }
        }

        public string Get = "hr.GetPersonnelLeaveCredits";
        public string CreateOrUpdate = "hr.CreateorUpdatePersonnelLeaveCredits";
        public string Delete = "hr.DeletePersonnelLeaveCredits";
        public string GetRemainingCredits = "hr.GetRemainingLeaveCredits";
    }
}
