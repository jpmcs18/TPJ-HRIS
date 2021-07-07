using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using System;

namespace ProcessLayer.Entities.CnB
{
    public class PayrollDetails
    {
        public long ID { get; set; }
        public DateTime LoggedDate { get; set; }
        public int TotalRegularMinutes { get; set; }
        public int TotalLeaveMinutes { get; set; }
        public Location Location { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsSunday { get; set; }
        public bool IsNonTaxable { get; set; }
        public bool IsAbsent { get { return (!(Location?.RequiredTimeLog ?? false) && !IsHoliday && !IsSunday && TotalRegularMinutes == 0); } }
        public bool IsHazard { get { return (Location?.ID ?? 0) > 0; } }
        public string HazardLocation { get { return Location?.Description ?? ""; } }
        public bool IsHighRisk { get; set; }
        public decimal HighRiskRate { get; set; }
        public decimal HighRiskPayRate { get; set; }
        public decimal HighRiskAllowanceRate { get; set; }
        public int RegularOTMinutes { get; set; }
        public int SundayOTMinutes { get; set; }
        public int HolidayRegularOTMinutes { get; set; }
        public int HolidayExcessOTMinutes { get; set; }
        public int NightDifferentialOTMinutes1 { get; set; }
        public int NightDifferentialOTMinutes2 { get; set; }
        public bool IsPresent { get; set; }

        //Display
        public decimal RegularHours { get { return (TotalRegularMinutes / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal RegularDay { get { return (TotalRegularMinutes / (decimal)PayrollParameters.CNBInstance.TotalMinutesPerDay).ToDecimalPlaces(3); } }
        public decimal LeaveHours { get { return (TotalLeaveMinutes / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal RegularOTHours { get { return (RegularOTMinutes / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal SundayOTHours { get { return (SundayOTMinutes / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal HolidayOTDays { get { return (HolidayRegularOTMinutes / (decimal)PayrollParameters.CNBInstance.HolidayTotalMinutes).ToDecimalPlaces(3); } }
        public decimal HolidayExcessOTHours { get { return (HolidayExcessOTMinutes / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal NightDifferentialOTHours1 { get { return (NightDifferentialOTMinutes1 / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }
        public decimal NightDifferentialOTHours2 { get { return (NightDifferentialOTMinutes2 / PayrollParameters.CNBInstance.Minutes).ToDecimalPlaces(2); } }

        public bool Modified { get; set; } = false;
    }
}
