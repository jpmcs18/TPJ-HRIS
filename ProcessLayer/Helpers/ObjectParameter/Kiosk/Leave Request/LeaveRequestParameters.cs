using System;

namespace ProcessLayer.Helpers.ObjectParameter.Kiosk.Leave_Request
{
    public class LeaveRequestParameters : KioskParametersBase
    {
        public const string LeaveUsed = "@LeaveUsed";
        public const string LeaveTypeID = "@LeaveTypeID";
        public const string StartDate = "@StartDate";
        public const string EndDate = "@EndDate";
        public const string BulkUse = "@BulkUse";
        public const string RequestedDate = "@RequestedDate";
        public const string Hospital = "@Hospital";
        public const string Location = "@Location";
        public const string PeriodStart = "@PeriodStart";
        public const string PeriodEnd = "@PeriodEnd";
        public const string NoofDays = "@NoofDays";
        public const string ComputedLeaveCredits = "@ComputedLeaveCredits";
    }
}
