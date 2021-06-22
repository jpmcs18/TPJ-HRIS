namespace ProcessLayer.Helpers.ObjectParameter.Kiosk
{
    public class KioskParametersBase
    {
        public string ID = "@ID";
        public string File = "@File";
        public string PersonnelID = "@PersonnelID";
        public string Personnel = "@Personnel";
        public string Approver = "@Approver";
        public string Reasons = "@Reasons";
        public string CancellationRemarks = "@CancellationRemarks";

        public string IsApproved = "@IsApproved";
        public string IsCancelled = "@IsCancelled";
        public string IsPending = "@IsPending";
        public string IsExpired = "@IsExpired";
    }
}
