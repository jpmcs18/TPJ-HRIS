using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using System;

namespace ProcessLayer.Entities.CnBs
{
    public class ComputedTimelog
    {
        public DateTime Date { get; set; }
        public DateTime? Login { get; set; }
        public DateTime? Logout { get; set; }
        public ScheduleType Schedule { get; set; }
        public string Assigned { get; set; }
        public decimal NoofDays { get; set; }
        public decimal Holiday { get; set; }
        public decimal HazRate { get; set; }
        public bool isHazard { get; set; } = false;
        public decimal AddHazardRate { get; set; }
        public bool isHighRisk { get; set; }
        public decimal HighRiskRate { get; set; }
        public string HolidayDesc { get; set; }
        public string LeaveDesc { get; set; }


        public decimal RegOTHours { get; set; }
        public decimal SunOTHours { get; set; }
        public decimal NightDiffEarlyHours { get; set; }
        public decimal NightDiffLateHours { get; set; }
        public decimal HolExcHours { get; set; }
        public decimal HazardHours { get { return isHazard ? NoofDays : 0; } }
        public decimal HighRiskHours { get { return isHighRisk ? (NoofDays > 0 ? 1 : 0) : 0; } }
    }
}
