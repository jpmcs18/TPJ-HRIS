using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Entities.HR;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ProcessLayer.Processes;
using ProcessLayer.Processes.CnB;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.Kiosk;
using ProcessLayer.Processes.Kiosks;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Computation.CnB
{
    public sealed class PayrollComputation
    {
        public static readonly Lazy<PayrollComputation> Instance = new Lazy<PayrollComputation>(() => new PayrollComputation());
        private PayrollComputation()
        {
            LateDeductions = LateDeductionProcess.Instance.Value.GetList();
        }
        private IEnumerable<NonWorkingDays> NonWorkingDays { get; set; }
        private IEnumerable<NonTaxableDay> NonTaxableDays { get; set; }
        private IEnumerable<LateDeduction> LateDeductions { get; set; }
        public PayrollType Monthly { get; set; }
        public PayrollPeriod GeneratePayroll(PayrollPeriod payrollPeriod)
        {
            List<Personnel> personnels = PersonnelProcess.GetForPayroll(payrollPeriod.StartDate, payrollPeriod.EndDate, payrollPeriod.Type);

            if (!(personnels?.Any() ?? false))
                throw new Exception("No personnel found");

            if (personnels?.Any() ?? false)
            {
                NonWorkingDays = NonWorkingDaysProcess.Instance.Value.GetNonWorkingDays(payrollPeriod.StartDate, payrollPeriod.EndDate);
                NonTaxableDays = NonTaxableDayProcess.Instance.Value.GetList(payrollPeriod.StartDate, payrollPeriod.EndDate);
                payrollPeriod.Payrolls = new List<Payroll>();

                personnels.ForEach(personnel =>
                {
                    payrollPeriod.Payrolls.Add(new Payroll
                    {
                        Personnel = personnel
                    });
                });

                if (payrollPeriod.Payrolls.Any())
                {
                    foreach (Payroll payroll in payrollPeriod.Payrolls)
                    {
                        List<PersonnelCompensation> compensation = PersonnelCompensationProcess.Instance.Value.GetByPersonnelID(payroll.Personnel.ID);
                        if (!(payroll.Personnel._AssignedLocation?.Any() ?? false)) throw new Exception("Cannot compute payroll for " + payroll.Personnel.FullName + ". No Assigned Location found.");

                        if (string.IsNullOrEmpty(payroll.Personnel._PayrollType?.Description)) throw new Exception("Cannot compute payroll for " + payroll.Personnel.FullName + ". No Payroll Type found.");

                        PersonnelCompensation comp = compensation.Where(x => x._Compensation.SupplementarySalary ?? false).FirstOrDefault();
                        decimal rate = (comp?.Amount ?? 0).ToDecimalPlaces(3);

                        if (rate == 0) throw new Exception("Cannot compute payroll for " + payroll.Personnel.FullName + ". No Rate found.");
                        decimal noofdays = payroll.Personnel._PayrollType?.NoofDays ?? 0;

                        if (!(payroll.Personnel._Schedules?.Any() ?? false)) throw new Exception("Cannot compute payroll for " + payroll.Personnel.FullName + ". No Schedule found.");
                    }

                    foreach (Payroll payroll in payrollPeriod.Payrolls)
                    {
                        Compute(payroll, payrollPeriod.Type, payrollPeriod.StartDate, payrollPeriod.EndDate);
                    }
                }
            }

            return payrollPeriod;
        }
        public Payroll Recompute(Payroll payroll, PayrollPeriod payrollPeriod)
        {
            NonWorkingDays = NonWorkingDaysProcess.Instance.Value.GetNonWorkingDays(payrollPeriod.StartDate, payrollPeriod.EndDate);
            NonTaxableDays = NonTaxableDayProcess.Instance.Value.GetList(payrollPeriod.StartDate, payrollPeriod.EndDate);

            Compute(payroll, payrollPeriod.Type, payrollPeriod.StartDate, payrollPeriod.EndDate);

            return payroll;
        }

        private void Compute(Payroll payroll, PayrollSheet type, DateTime periodStart, DateTime periodEnd)
        {
            List<PersonnelCompensation> compensation = PersonnelCompensationProcess.Instance.Value.GetByPersonnelID(payroll.Personnel.ID);
            PersonnelCompensation comp = compensation.Where(x => x._Compensation.SupplementarySalary ?? false).FirstOrDefault();
            decimal rate = (comp?.Amount ?? 0).ToDecimalPlaces(3);
            decimal noofdays = payroll.Personnel._PayrollType?.NoofDays ?? 0;

            rate = GetRate(payroll, periodStart, periodEnd, comp, rate, noofdays);

            byte payrollType = payroll.Personnel.PayrollTypeID ?? 0;

            byte cutoff = (byte)((periodEnd - periodStart).TotalDays > 25 ? 0 : (periodEnd.Day <= 15 ? 1 : 2));

            List<PersonnelLoan> deductibleLoans = new List<PersonnelLoan>();
            if(type == PayrollSheet.B)
                deductibleLoans = PersonnelLoanProcess.Instance.Value.GetDeductibleAmount(payroll.ID, payroll.Personnel.ID, periodStart, periodEnd);
            else
                deductibleLoans = PersonnelLoanProcess.Instance.Value.GetPayrollBGovernmentLoanDeductions(payroll.Personnel.ID, periodStart, periodEnd);

            if(payroll.ID > 0)
            {
                payroll.Tax = 0;
                payroll.NetPay = 0;
                payroll.TotalDeductions = 0;
                payroll.BasicPay = 0;
                payroll.GrossPay = 0;
                payroll.OutstandingVale = 0;
            }
            
            List<PersonnelDeduction> deductions = PersonnelDeductionProcess.GetByPersonnelID(payroll.Personnel.ID);

            List<TimeLog> timelogs = TimeLogProcess.Get(payroll.Personnel.ID, periodStart.AddDays(-5), periodEnd);
            List<OTRequest> approvedotrequests = OTRequestProcess.Instance.Value.GetApprovedOT(payroll.Personnel.ID, periodStart, periodEnd);
            List<LeaveRequest> approvedleaverequests = LeaveRequestProcess.Instance.Value.GetLeaveForPayroll(payroll.Personnel.ID, periodStart, periodEnd);
            List<ComputedLeaveCredits> computedLeaveCredits = new List<ComputedLeaveCredits>();
            foreach(LeaveRequest l in approvedleaverequests)
            {
                if(l._ComputedLeaveCredits?.Any() ?? false)
                {
                    l.ComputedLeaveCredits -= l._ComputedLeaveCredits.Sum(x => x.LeaveCreditUsed);
                    computedLeaveCredits.AddRange(l._ComputedLeaveCredits);
                }
            }
            List<OuterPortRequest> approvedouterportrequests = OuterPortRequestProcess.Instance.Value.GetApprovedOuterPort(payroll.Personnel.ID, null, periodStart, periodEnd);
            List<HighRiskRequest> approvedhighriskrequests = HighRiskRequestProcess.Instance.Value.GetApproved(payroll.Personnel.ID, periodStart, periodEnd);

            payroll.Department = PersonnelDepartmentProcess.GetList(payroll.Personnel.ID).Where(x => x.StartDate <= periodStart).OrderByDescending(x => x.StartDate).FirstOrDefault()?._Department?.Description;
            payroll.Position = PersonnelPositionProcess.GetList(payroll.Personnel.ID).Where(x => x.StartDate <= periodStart).OrderByDescending(x => x.StartDate).FirstOrDefault()?._Position?.Description;
            payroll.Allowance = (compensation.Where(x => (x._Compensation?.Taxable ?? false) == false && (x._Compensation?.SupplementarySalary ?? false) == false).Sum(x => x.Amount) ?? 0).ToDecimalPlaces(3);
            payroll.RegularOTAllowance = ((payroll.Allowance / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.RegularOTRate).ToDecimalPlaces(3);
            payroll.SundayOTAllowance = ((payroll.Allowance / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.SundayOTRate).ToDecimalPlaces(3);
            payroll.HolidayRegularOTAllowance = payroll.Allowance.ToDecimalPlaces(3);
            payroll.HolidayExcessOTAllowance = ((payroll.Allowance / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.HolidayOTRate).ToDecimalPlaces(3);
            payroll.RegularOTRate = ((payroll.DailyRate / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.RegularOTRate).ToDecimalPlaces(3);
            payroll.SundayOTRate = ((payroll.DailyRate / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.SundayOTRate).ToDecimalPlaces(3);
            payroll.HolidayRegularOTRate = payroll.DailyRate.ToDecimalPlaces(3);
            payroll.HolidayExcessOTRate = ((payroll.DailyRate / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.HolidayOTRate).ToDecimalPlaces(3);
            payroll.NightDifferentialRate1 = ((payroll.DailyRate / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.NightDiffRate1).ToDecimalPlaces(3);
            payroll.NightDifferentialRate2 = ((payroll.DailyRate / PayrollParameters.CNBInstance.Value.DailyHours) * PayrollParameters.CNBInstance.Value.NightDiffRate2).ToDecimalPlaces(3);
            payroll.AdditionalAllowanceRate = (payroll.Allowance * payroll.Personnel.AdditionalHazardRate).ToDecimalPlaces(3);
            payroll.AdditionalPayRate = (payroll.DailyRate * payroll.Personnel.AdditionalHazardRate).ToDecimalPlaces(3);
            payroll.HighRiskPayRate = (payroll.DailyRate * PayrollParameters.CNBInstance.Value.HighRiskRate).ToDecimalPlaces(3);
            payroll.HighRiskAllowanceRate = (payroll.Allowance * PayrollParameters.CNBInstance.Value.HighRiskRate).ToDecimalPlaces(3);
            payroll.ExtensionRate = (payroll.DailyRate * PayrollParameters.CNBInstance.Value.ExtensionRate).ToDecimalPlaces(3);
            payroll.ExtensionAllowanceRate = (payroll.Allowance * PayrollParameters.CNBInstance.Value.ExtensionRate).ToDecimalPlaces(3);
            if (!(payroll.Personnel.FixedSalary ?? false))
            {
                DateTime start = periodStart.Date;
                DateTime end = periodEnd.Date;
                while (start <= end)
                {
                    //Get Schedule
                    #region schedule per day
                    ScheduleType sched = GlobalHelper.GetSchedule(payroll.Personnel._Schedules, start) ?? new ScheduleType();
                    DateTime starttime = start;
                    DateTime breaktime = start;
                    DateTime breaktimeend = start;
                    DateTime endtime = start;
                    DateTime startnight1 = start + PayrollParameters.CNBInstance.Value.NightDiffStartTime1;
                    DateTime endnight1 = start + PayrollParameters.CNBInstance.Value.NightDiffEndTime1;
                    DateTime startnight2 = start + PayrollParameters.CNBInstance.Value.NightDiffStartTime2;
                    DateTime endnight2 = start + PayrollParameters.CNBInstance.Value.NightDiffEndTime2;

                    if (endnight1 < startnight1)
                        endnight1 = endnight1.AddDays(1);

                    if (endnight2 < startnight1)
                        endnight2 = endnight2.AddDays(1);

                    if (sched.BreakTime.HasValue) breaktime = start + sched.BreakTime.Value;

                    if (sched.BreakTimeHour.HasValue) breaktimeend = breaktime.AddHours(sched.BreakTimeHour.Value);

                    if (sched.TimeIn.HasValue) starttime += sched.TimeIn.Value;

                    if (sched.TimeOut.HasValue)
                    {
                        endtime += sched.TimeOut.Value;
                        if (endtime < starttime)
                            endtime = endtime.AddDays(1);
                    }
                    #endregion
                    DateTime defbt = start + PayrollParameters.CNBInstance.Value.DefaultBreaktime;
                    DateTime defbtend = defbt.AddHours(PayrollParameters.CNBInstance.Value.DefaultBreaktimeHour);
                    var loc = PersonnelAssignedLocationProcess.GetCurrent(payroll.Personnel.ID, start)?._Location ?? new Location();

                    PayrollDetails details = new PayrollDetails
                    {
                        IsSunday = starttime.DayOfWeek == DayOfWeek.Sunday,
                        LoggedDate = start,
                        TotalRegularMinutes = 0,
                    };

                    if (payroll.PayrollDetails?.Where(x => x.LoggedDate == start)?.Any() ?? false)
                    {
                        details = payroll.PayrollDetails.Where(x => x.LoggedDate == start).First();
                        details.Location = default;
                        details.TotalRegularMinutes = default;
                        details.TotalLeaveMinutes = default;
                        details.IsHoliday = default;
                        details.IsNonTaxable = default;
                        details.IsHighRisk = default;
                        details.RegularOTMinutes = default;
                        details.SundayOTMinutes = default;
                        details.HolidayRegularOTMinutes = default;
                        details.HolidayExcessOTMinutes = default;
                        details.NightDifferentialOTMinutes1 = default;
                        details.NightDifferentialOTMinutes2 = default;
                        details.IsPresent = default;
                        details.Modified = true;
                    }

                    //Get Time Log
                    #region timelog per day
                    DateTime? LoginDate = timelogs.Where(x => (x.LoginDate >= starttime && x.LogoutDate <= endtime)
                                        || (starttime >= x.LoginDate && endtime <= x.LogoutDate)
                                        || (starttime <= x.LoginDate && endtime <= x.LogoutDate && endtime >= x.LoginDate)
                                        || (x.LoginDate <= starttime && x.LogoutDate <= endtime && x.LogoutDate >= endtime)
                                        || (x.LoginDate?.Date == start.Date)).OrderBy(x => x.LoginDate).Select(x => x.LoginDate).FirstOrDefault();

                    DateTime? LogoutDate = timelogs.Where(x => (x.LoginDate >= starttime && x.LogoutDate <= endtime)
                                        || (starttime >= x.LoginDate && endtime <= x.LogoutDate)
                                        || (starttime <= x.LoginDate && endtime <= x.LogoutDate && endtime >= x.LoginDate)
                                        || (x.LoginDate <= starttime && x.LogoutDate <= endtime && x.LogoutDate >= endtime)
                                        || (x.LoginDate?.Date == start.Date)).OrderByDescending(x => x.LogoutDate).Select(x => x.LogoutDate).FirstOrDefault();

                    LogoutDate = LogoutDate ?? LoginDate;

                    LoginDate = LoginDate?.AddSeconds(-(LoginDate?.Second ?? 0));
                    LogoutDate = LogoutDate?.AddSeconds(-(LogoutDate?.Second ?? 0));
                    #endregion
                    LeaveRequest leave = approvedleaverequests.Where(x => starttime.Date >= x.RequestedDate && x.ApprovedLeaveCredits.HasValue && x.ApprovedLeaveCredits > 0).FirstOrDefault();
                    float leaveCredits = (leave?.ApprovedLeaveCredits ?? 0) - (leave?.ComputedLeaveCredits ?? 0);
                    float leaveCreditsUsed = leaveCredits > 1 ? 1 : leaveCredits;
                    
                    OTRequest earlyOT = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date && x.OTType == OTType.Early).FirstOrDefault();
                    OTRequest afterWorkOT = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date && x.OTType == OTType.After).FirstOrDefault();
                    OTRequest wholeDayOT = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date && x.OTType == OTType.Whole).FirstOrDefault();

                    if ((leave?.ID ?? 0) == 0)
                    {
                        if ((earlyOT?.ID ?? 0) > 0 && !(earlyOT?.IsOffice ?? false))
                            LoginDate = earlyOT.StartDateTime;
                        if ((afterWorkOT?.ID ?? 0) > 0 && !(afterWorkOT?.IsOffice ?? false))
                            LogoutDate = afterWorkOT.EndDateTime;
                        if ((wholeDayOT?.ID ?? 0) > 0 && !(wholeDayOT?.IsOffice ?? false))
                        {
                            LoginDate = wholeDayOT.StartDateTime;
                            LogoutDate = wholeDayOT.EndDateTime;
                        }
                    }

                    //get outer port request on specific date
                    bool needTimeLog = false;
                    OuterPortRequest outerPort = approvedouterportrequests.Where(x => starttime.Date >= x.StartDate && (starttime.Date <= x.EndDate || !x.EndDate.HasValue)).FirstOrDefault();
                    HighRiskRequest highRisk = approvedhighriskrequests.Where(x => starttime.Date == x.RequestDate.Date).FirstOrDefault();

                    NonTaxableDay nonTaxableDay = NonTaxableDays.Where(x => x.StartDate <= start && (x.EndDate >= start && x.EndDate == null) && (x.LocationID == loc.ID || (x.IsGlobal ?? false))).FirstOrDefault();
                    NonWorkingDays holiday = NonWorkingDays.Where(x => (((x.Yearly ?? false) && x.Day?.Month == start.Month && x.Day?.Day == start.Day) || x.Day == start) && (x.LocationID == loc.ID || (x.IsGlobal ?? false))).FirstOrDefault();
                    DateTime? prevDate = null;

                    if ((outerPort?.ID ?? 0) > 0)
                    {
                        details.Location = outerPort._Location;
                        needTimeLog = outerPort._Location?.RequiredTimeLog ?? false;
                        if(outerPort.IsHighRisk ?? false)
                        {
                            if (outerPort.HasQuarantine ?? false)
                            {
                                if(outerPort.QuarantineDateEnd != null && outerPort.QuarantineDateEnd < start) //same day or after?????
                                {
                                    details.IsHighRisk = true;
                                }
                            }
                            else
                            {
                                details.IsHighRisk = true;
                            }
                        }

                        if(outerPort._Location.WithAdditionalForExtension ?? false && outerPort.StartDate?.AddMonths(PayrollParameters.CNBInstance.Value.ExtendedMonths) <= start)
                        {
                            details.IsExtended = true;
                        }

                        holiday = NonWorkingDays.Where(x => (((x.Yearly ?? false) && x.Day?.Month == start.Month && x.Day?.Day == start.Day) || x.Day == start) && (x.LocationID == outerPort._Location?.ID || (x.IsGlobal ?? false))).FirstOrDefault();
                    }
                    if ((holiday?.ID ?? 0) > 0)
                    {
                        prevDate = GlobalHelper.GetPrevSchedDate(payroll.Personnel._Schedules, starttime, NonWorkingDays.ToList(), outerPort?._Location?.ID ?? loc?.ID);
                        details.IsHoliday = true;
                    }
                    if ((highRisk?.ID ?? 0) > 0)
                    {
                        details.IsHighRisk = true;
                    }
                    if ((nonTaxableDay?.ID ?? 0) > 0)
                    {
                        details.IsNonTaxable = true;
                    }
                    if(((holiday?.ID ?? 0) == 0 || ((holiday?.ID ?? 0) > 0 && (holiday?.NonWorkingType ?? 0) < 3)) && (sched?.ID ?? 0) > 0 && leaveCredits > 0)
                    {
                        if ((leave?.ID ?? 0) > 0)
                        {
                            details.TotalLeaveMinutes = (int)(leaveCreditsUsed * sched.TotalWorkingHours * 60);
                            leave.ComputedLeaveCredits = (leave.ComputedLeaveCredits ?? 0) + leaveCreditsUsed;
                            ComputedLeaveCredits c = computedLeaveCredits.Where(x => x.ComputedDate.Date == start.Date && x.LeaveRequestID == leave.ID).FirstOrDefault();
                            if ((c?.ID ?? 0) > 0)
                            {
                                c.Modified = true;
                                c.LeaveCreditUsed = leaveCreditsUsed;
                            }
                            else
                            {
                                computedLeaveCredits.Add(new ComputedLeaveCredits
                                {
                                    ComputedDate = start,
                                    LeaveCreditUsed = leaveCreditsUsed,
                                    LeaveRequestID = leave.ID,
                                });
                            }
                        }
                    }
                    if(leaveCreditsUsed == 1)
                    {
                        details.TotalRegularMinutes = details.TotalLeaveMinutes;
                        if (details.ID == 0)
                            payroll.PayrollDetails.Add(details);
                    }
                    else if (((holiday?.ID ?? 0) > 0) || (sched?.ID ?? 0) == 0 || start.DayOfWeek == DayOfWeek.Sunday)
                    {
                        SundayOrHolidayComputation(payroll, outerPort?._Location?.WithHolidayAndSunday ?? false, needTimeLog, approvedouterportrequests, timelogs, start, sched, starttime, endtime, startnight1, endnight1, startnight2, endnight2, defbt, defbtend, details, LoginDate, LogoutDate, (wholeDayOT ?? earlyOT) ?? afterWorkOT, ((holiday?.ID ?? 0) > 0), prevDate);
                    }
                    else if (!needTimeLog && details.IsHazard)
                    {
                        details.TotalRegularMinutes = PayrollParameters.CNBInstance.Value.TotalMinutesPerDay;
                        if (details.ID == 0)
                            payroll.PayrollDetails.Add(details);
                    }
                    else if (sched.AtHome ?? false)
                    {
                        WorkFromHomeComputation(payroll, sched, details, (wholeDayOT ?? earlyOT) ?? afterWorkOT, LoginDate, LogoutDate);
                    }
                    else
                    {
                        if (sched.MustBePresentOnly ?? false)
                        {
                            MustBePresentOnly(payroll, sched, details);
                        }
                        else
                        {
                            if (!sched.TimeIn.HasValue && !sched.TimeOut.HasValue) //Flexi
                            {
                                FlexiComputation(payroll, afterWorkOT, end, sched, starttime, breaktime, breaktimeend, endtime, startnight1, endnight1, startnight2, endnight2, details, LoginDate, LogoutDate, leaveCreditsUsed);
                            }
                            else //Regular Hours
                            {
                                RegularComputation(payroll, start, sched, starttime, breaktime, breaktimeend, endtime, startnight1, endnight1, startnight2, endnight2, details, LoginDate, LogoutDate, leaveCreditsUsed, earlyOT, afterWorkOT);
                            }
                        }
                    }

                    start = start.AddDays(1);
                }
            }

            payroll.BasicPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).GroupBy(x => x.Location?.ID ?? 0).Sum(t => t.Sum(x => (payroll.DailyRate * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2)).ToDecimalPlaces(2);
            payroll.NOofDays = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Sum(x => x.RegularDay);

            if (type == PayrollSheet.B)
            {
                payroll.RegularOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.RegularOTAllowance * x.RegularOTHours).ToDecimalPlaces(2);
                payroll.SundayOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.SundayOTAllowance * x.SundayOTHours).ToDecimalPlaces(2);
                payroll.HolidayOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTAllowance * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2);
                payroll.HolidayOTExcessAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.HolidayExcessOTAllowance * x.HolidayExcessOTHours).ToDecimalPlaces(2);
                payroll.TotalOTAllowance = (payroll.RegularOTAllowancePay + payroll.SundayOTAllowancePay + payroll.HolidayOTAllowancePay + payroll.HolidayOTExcessAllowancePay).ToDecimalPlaces(2);
                payroll.TotalAllowance = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).GroupBy(x => x.Location?.ID ?? 0).Sum(t => t.Sum(x => (payroll.Allowance * (x.IsHazard ? ((x.Location?.HazardRate ?? 0) + 1) : 1)).ToDecimalPlaces(3) * x.RegularDay).ToDecimalPlaces(2)).ToDecimalPlaces(2);
                payroll.TotalAdditionalPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsPresent).Sum(x => payroll.AdditionalPayRate * x.RegularDay).ToDecimalPlaces(2);
                payroll.TotalAdditionalAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsPresent).Sum(x => payroll.AdditionalAllowanceRate * x.RegularDay).ToDecimalPlaces(2);
                payroll.TotalHighRiskPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsHighRisk).Sum(x => payroll.HighRiskPayRate).ToDecimalPlaces(2);
                payroll.TotalHighRiskAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsHighRisk).Sum(x => payroll.HighRiskAllowanceRate).ToDecimalPlaces(2);
                payroll.TotalExtensionPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsExtended).Sum(x => payroll.ExtensionRate).ToDecimalPlaces(2);
                payroll.TotalExtensionAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsExtended).Sum(x => payroll.ExtensionAllowanceRate).ToDecimalPlaces(2);
                payroll.TotalAdditionalPay += payroll.TotalHighRiskPay + payroll.TotalExtensionPay;
                payroll.TotalAdditionalAllowancePay += payroll.TotalHighRiskAllowancePay + payroll.TotalExtensionAllowancePay;
                var _RegularOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.RegularOTRate * x.RegularOTHours).ToDecimalPlaces(2);
                var _SundayOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.SundayOTRate * x.SundayOTHours).ToDecimalPlaces(2);
                var _HolidayOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTRate * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2);
                var _HolidayExcessOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.HolidayExcessOTRate * x.HolidayExcessOTHours).ToDecimalPlaces(2);
                var _RegularOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.RegularOTAllowance * x.RegularOTHours).ToDecimalPlaces(2);
                var _SundayOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.SundayOTAllowance * x.SundayOTHours).ToDecimalPlaces(2);
                var _HolidayOTAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTAllowance * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2);
                var _HolidayOTExcessAllowancePay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => x.IsNonTaxable).Sum(x => payroll.HolidayExcessOTAllowance * x.HolidayExcessOTHours).ToDecimalPlaces(2);
                payroll.TotalAdditionalOvertimePay = _RegularOTPay + _SundayOTPay + _HolidayOTPay + _HolidayExcessOTPay;
                payroll.TotalAdditionalOvertimeAllowancePay = _RegularOTAllowancePay + _SundayOTAllowancePay + _HolidayOTAllowancePay + _HolidayOTExcessAllowancePay;
            }

            payroll.RegularOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.RegularOTRate * x.RegularOTHours).ToDecimalPlaces(2);
            payroll.SundayOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.SundayOTRate * x.SundayOTHours).ToDecimalPlaces(2);
            payroll.HolidayOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.HolidayRegularOTRate * ((x.Location?.HazardRate ?? 0) + 1) * x.HolidayOTDays).ToDecimalPlaces(2);
            payroll.HolidayExcessOTPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Where(x => !x.IsNonTaxable).Sum(x => payroll.HolidayExcessOTRate * x.HolidayExcessOTHours).ToDecimalPlaces(2);
            payroll.NightDifferentialPay = payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Sum(x => payroll.NightDifferentialRate1 * x.NightDifferentialOTHours1) + payroll.PayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Sum(x => payroll.NightDifferentialRate2 * x.NightDifferentialOTHours2).ToDecimalPlaces(2);
            payroll.TotalOTPay = (payroll.RegularOTPay + payroll.SundayOTPay + payroll.HolidayOTPay + payroll.HolidayExcessOTPay + payroll.NightDifferentialPay).ToDecimalPlaces(2);
            payroll.GrossPay = (payroll.BasicPay + payroll.TotalOTPay).ToDecimalPlaces(2);

            ComputeDeductions(payroll, periodStart, cutoff, deductions);

            payroll.Tax += PayrollProcess.Instance.Value.GetTax(payroll.Personnel?.ID ?? 0, type, payroll.GrossPay, cutoff, periodStart) ?? 0;
            payroll.TotalDeductions += payroll.Tax.ToDecimalPlaces(2);
            payroll.NetPay += payroll.GrossPay.ToDecimalPlaces(2) + payroll.SumOfAllAllowance.ToDecimalPlaces(2) - payroll.TotalDeductions.ToDecimalPlaces(2) + payroll.SumOfAllAdditionalPay.ToDecimalPlaces(2);

            ComputeLoans(payroll, type, deductibleLoans);
            if (type == PayrollSheet.B)
            {
                if (payroll.NetPay < 0)
                {
                    payroll.OutstandingVale = Math.Abs(payroll.NetPay.ToDecimalPlaces(2));
                    payroll.NetPay = 0;
                }
            }

            payroll.LeaveRequests = approvedleaverequests;
            payroll.ComputedLeaveCredits = computedLeaveCredits;
        }

        private static void WorkFromHomeComputation(Payroll payroll, ScheduleType sched, PayrollDetails details, OTRequest ot, DateTime? LoginDate, DateTime? LogoutDate)
        {
            details.TotalRegularMinutes = (sched.TotalWorkingHours ?? 0) * 60;

            if ((ot?.ID ?? 0) > 0 || payroll.Personnel.AutoOT)
            {
                DateTime? startot = LoginDate;
                DateTime? endot = LogoutDate;

                if (endot > startot)
                {
                    details.RegularOTMinutes = GlobalHelper.SubtractDate(endot, startot);
                }
            }

            if (details.ID == 0)
                payroll.PayrollDetails.Add(details);
        }

        private void RegularComputation(Payroll payroll, DateTime start, ScheduleType sched, DateTime starttime, DateTime breaktime, DateTime breaktimeend, DateTime endtime, DateTime startnight1, DateTime endnight1, DateTime startnight2, DateTime endnight2, PayrollDetails details, DateTime? LoginDate, DateTime? LogoutDate, float leave, OTRequest earlyOT, OTRequest afterWorkOT)
        {
            if (!LoginDate.HasValue && !LogoutDate.HasValue)
            {
                details.TotalRegularMinutes = details.TotalLeaveMinutes;
            }
            else
            {
                int totalRegularMinutes = GlobalHelper.SubtractDate(endtime, starttime) - GlobalHelper.SubtractDate(breaktimeend, breaktime);
                details.TotalRegularMinutes = totalRegularMinutes;
                int late = 0;
                int undertime = 0;
                //compute late
                if (starttime < LoginDate)
                {
                    late = GlobalHelper.SubtractDate(LoginDate, starttime);
                    LateDeduction lateded = LateDeductions.Where(x => (start + x.TimeIn) <= LoginDate).OrderByDescending(x => x.TimeIn).FirstOrDefault();
                    if ((lateded?.ID ?? 0) > 0)
                    {
                        if (lateded.DeductedHours.HasValue)
                            late = (lateded.DeductedHours ?? 0) * (int)PayrollParameters.CNBInstance.Value.Minutes;
                    }

                    if (sched.BreakTime.HasValue && sched.BreakTimeHour.HasValue)
                    {
                        if (breaktime < LoginDate &&
                            breaktime.AddHours(sched.BreakTimeHour ?? 0) > LoginDate &&
                            breaktime.AddHours(sched.BreakTimeHour ?? 0) < LogoutDate)
                            late -= GlobalHelper.SubtractDate(breaktime.AddHours(sched.BreakTimeHour ?? 0), LoginDate);
                        else if (breaktime.AddHours(sched.BreakTimeHour ?? 0) <= LoginDate)
                        {
                            late -= sched.BreakTimeHour ?? 0;
                        }
                    }
                }
                //compute undertime
                if (LogoutDate.HasValue && LogoutDate < endtime)
                {
                    if (sched.BreakTime.HasValue && sched.BreakTimeHour.HasValue)
                    {
                        if (breaktime.AddHours(sched.BreakTimeHour ?? 0) > LogoutDate && breaktime < LogoutDate)
                            undertime = GlobalHelper.SubtractDate(endtime, breaktime.AddHours(sched.BreakTimeHour ?? 0));
                        else if (breaktime > LogoutDate)
                            undertime = GlobalHelper.SubtractDateWithBreakTime(endtime, LogoutDate, sched.BreakTimeHour ?? 0);
                        else if (breaktime.AddHours(sched.BreakTimeHour ?? 0) < LogoutDate)
                            undertime = GlobalHelper.SubtractDate(endtime, LogoutDate);
                        else
                            undertime = GlobalHelper.SubtractDate(endtime, breaktime.AddHours(sched.BreakTimeHour ?? 0));
                    }
                    else
                        undertime = GlobalHelper.SubtractDate(endtime, LogoutDate);
                    details.TotalRegularMinutes -= undertime;
                }

                //compute ot after office
                if ((afterWorkOT?.ID ?? 0) > 0 || payroll.Personnel.AutoOT)
                {
                    DateTime? startot = endtime;
                    DateTime? endot = LogoutDate;

                    if (endot > startot)
                    {
                        int totolotminutes = GlobalHelper.SubtractDate(endot, startot);

                        int totalotminutesremain = totolotminutes - late;
                        if (totalotminutesremain <= 0)
                            late = Math.Abs(totalotminutesremain);
                        else
                        {
                            late = 0;
                            details.RegularOTMinutes = totalotminutesremain;
                        }
                    }
                }

                //compute early ot
                if ((earlyOT?.ID ?? 0) > 0)
                {
                    DateTime? startot = LoginDate;
                    DateTime? endot = starttime;

                    if (endot > startot)
                    {
                        details.RegularOTMinutes += GlobalHelper.SubtractDate(endot, startot);
                    }
                }
            
                details.TotalRegularMinutes -= late;
                details.IsPresent = true;


                NighDiffComputation(starttime, endtime, startnight1, endnight1, startnight2, endnight2, details, LoginDate, LogoutDate);

                details.TotalRegularMinutes = details.TotalRegularMinutes < 0 ? 0 : details.TotalRegularMinutes;
                details.TotalRegularMinutes += details.TotalLeaveMinutes;
                details.TotalRegularMinutes = details.TotalRegularMinutes > (int)(sched.TotalWorkingHours * 60) ? (int)(sched.TotalWorkingHours * 60) : details.TotalRegularMinutes;
            }
            if (details.ID == 0)
                payroll.PayrollDetails.Add(details);
        }

        private static void NighDiffComputation(DateTime starttime, DateTime endtime, DateTime startnight1, DateTime endnight1, DateTime startnight2, DateTime endnight2, PayrollDetails details, DateTime? LoginDate, DateTime? LogoutDate)
        {
            if (starttime <= startnight1 && endtime >= endnight1)
            {
                DateTime? login = LoginDate < startnight1 ? startnight1 : LoginDate;
                DateTime? logout = LogoutDate > endnight1 ? endnight1 : LogoutDate;

                details.NightDifferentialOTMinutes1 = GlobalHelper.SubtractDate(logout, login);
            }
            if (starttime <= startnight2 && endtime >= endnight2)
            {
                DateTime? login = LoginDate < startnight2 ? startnight2 : LoginDate;
                DateTime? logout = LogoutDate > endnight2 ? endnight2 : LogoutDate;

                details.NightDifferentialOTMinutes2 = GlobalHelper.SubtractDate(logout, login);
            }
        }

        private static void SundayOrHolidayComputation(Payroll payroll, bool withHolidayAndSundayHazard, bool needTimeLog, List<OuterPortRequest> approvedouterportrequests, List<TimeLog> timelogs, DateTime start, ScheduleType sched, DateTime starttime, DateTime endtime, DateTime startnight1, DateTime endnight1, DateTime startnight2, DateTime endnight2, DateTime defbt, DateTime defbtend, PayrollDetails details, DateTime? LoginDate, DateTime? LogoutDate, OTRequest ot, bool isholiday, DateTime? prevDate)
        {
            if (isholiday && (sched?.ID ?? 0) != 0 && (timelogs.Where(x => x.LoginDate?.Date == prevDate?.Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, prevDate) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, prevDate)))
            {
                details.TotalRegularMinutes = PayrollParameters.CNBInstance.Value.TotalMinutesPerDay;
                details.IsHoliday = true;
            }

            if (LoginDate.HasValue && LogoutDate.HasValue || withHolidayAndSundayHazard)
            {
                int mins = 0;
                if ((ot?.ID ?? 0) > 0 || payroll.Personnel.AutoOT || withHolidayAndSundayHazard)
                {
                    if (needTimeLog || !details.IsHazard)
                    {
                        if ((LoginDate > defbt && defbtend > LoginDate) && defbtend < LogoutDate)
                        {
                            mins = GlobalHelper.SubtractDate(LogoutDate, defbtend);
                        }
                        else if (LoginDate >= defbtend || LogoutDate <= defbt)
                        {
                            mins = GlobalHelper.SubtractDate(LogoutDate, LoginDate);
                        }
                        else if ((LogoutDate > defbt && defbtend > LogoutDate) && defbt > LoginDate)
                        {
                            mins = GlobalHelper.SubtractDate(defbt, LoginDate);
                        }
                        else if (LoginDate < defbt && LogoutDate > defbtend)
                        {
                            mins = GlobalHelper.SubtractDate(LogoutDate, LoginDate) - (int)PayrollParameters.CNBInstance.Value.DefaultBreaktimeMinutes;
                        }

                        if (start.DayOfWeek == DayOfWeek.Sunday && (sched?.ID ?? 0) == 0)
                        {
                            details.IsSunday = true;
                            int regminutes = mins - PayrollParameters.CNBInstance.Value.SundayTotalMinutes;
                            if (regminutes > 0
                                && (timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(-1).Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, starttime.AddDays(-1)) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, starttime.AddDays(-1)))
                                && (timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(1).Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, starttime.AddDays(1)) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, starttime.AddDays(1))))
                            {
                                details.IsPresent = true;
                                details.TotalRegularMinutes = PayrollParameters.CNBInstance.Value.TotalMinutesPerDay;
                                details.SundayOTMinutes = regminutes;
                            }
                            else
                                details.TotalRegularMinutes = mins;

                        }
                        else if (isholiday)
                        {
                            details.IsHoliday = true;
                            int regminutes = mins - PayrollParameters.CNBInstance.Value.HolidayTotalMinutes;
                            if (regminutes > 0
                                && (timelogs.Where(x => x.LoginDate?.Date == prevDate?.Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, prevDate) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, prevDate)))
                            {
                                details.IsPresent = true;
                                details.HolidayRegularOTMinutes = PayrollParameters.CNBInstance.Value.HolidayTotalMinutes;
                                details.HolidayExcessOTMinutes = regminutes;

                            }
                            else
                                details.HolidayRegularOTMinutes = mins;
                        }
                    }

                    else
                    {
                        if (start.DayOfWeek == DayOfWeek.Sunday)
                        {
                            if ((timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(-1).Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, starttime.AddDays(-1)) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, starttime.AddDays(-1)))
                                && (timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(1).Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, starttime.AddDays(1)) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, starttime.AddDays(1))))
                            {
                                details.IsPresent = true;
                                details.TotalRegularMinutes = PayrollParameters.CNBInstance.Value.TotalMinutesPerDay;
                            }

                        }
                        else if (isholiday)
                        {
                            if ((timelogs.Any(x => x.LoginDate?.Date == prevDate?.Date) || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, prevDate) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, prevDate)))
                            {
                                details.IsPresent = true;
                                details.HolidayRegularOTMinutes = PayrollParameters.CNBInstance.Value.HolidayTotalMinutes;
                            }
                        }
                    }
                    NighDiffComputation(starttime, endtime, startnight1, endnight1, startnight2, endnight2, details, LoginDate, LogoutDate);
                }
                details.TotalRegularMinutes = details.TotalRegularMinutes < 0 ? 0 : details.TotalRegularMinutes;

                if (details.ID == 0)
                    payroll.PayrollDetails.Add(details);
            }
            else if (isholiday && (sched?.ID ?? 0) != 0 && (timelogs.Where(x => x.LoginDate?.Date == prevDate?.Date).Any() || GlobalHelper.IsOuterPortWithoutTimelog(approvedouterportrequests, prevDate) || GlobalHelper.IsWFH(payroll.Personnel._Schedules, prevDate)))
            {
                if (details.ID == 0)
                    payroll.PayrollDetails.Add(details);
            }
        }

        private void ComputeLoans(Payroll payroll, PayrollSheet type, List<PersonnelLoan> deductibleLoans)
        {
            if (deductibleLoans?.Any() ?? false)
            {
                deductibleLoans.ForEach(d =>
                {
                    if (d._Loan.GovernmentLoan ?? false)
                    {
                        payroll.TotalDeductions += (d.Amount ?? 0).ToDecimalPlaces(2);
                        payroll.NetPay -= (d.Amount ?? 0).ToDecimalPlaces(2);
                        if (payroll.LoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).Any())
                        {
                            var ded = payroll.LoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).First();
                            ded.Modified = true;
                            ded.Amount = (d.Amount ?? 0);
                        }
                        else
                            payroll.LoanDeductions.Add(new LoanDeductions { Amount = (d.Amount ?? 0), PersonnelLoan = d });
                    }
                    else
                    {
                        if (type == PayrollSheet.B)
                        {
                            //if (payroll.NetPay >= d.Amortization)
                            //{
                                payroll.TotalDeductions += (d.Amount ?? 0).ToDecimalPlaces(2);
                                payroll.NetPay -= (d.Amount ?? 0).ToDecimalPlaces(2);

                                if (payroll.LoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).Any())
                                {
                                    var ded = payroll.LoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).First();
                                    ded.Modified = true;
                                    ded.Amount = (d.Amount ?? 0);
                                }
                                else
                                    payroll.LoanDeductions.Add(new LoanDeductions { Amount = (d.Amount ?? 0), PersonnelLoan = d });
                            //}
                        }
                    }
                });
            }
        }

        private void ComputeDeductions(Payroll payroll, DateTime periodStart, byte cutoff, List<PersonnelDeduction> deductions)
        {
            if (deductions?.Any() ?? false)
            {
                deductions.ForEach(d =>
                {
                    payroll.TotalDeductions += (d.Amount ?? 0).ToDecimalPlaces(2);
                    if (payroll.PayrollDeductions.Where(x => x.Deduction?.ID == d._Deduction?.ID && x.ID > 0).Any())
                    {
                        var ded = payroll.PayrollDeductions.Where(x => x.Deduction?.ID == d._Deduction?.ID && x.ID > 0).First();
                        ded.Modified = true;
                        ded.Amount = d.Amount;
                    }
                    else
                        payroll.PayrollDeductions.Add(new PayrollDeductions { Deduction = d._Deduction, Amount = d.Amount });
                });
            }
            PayrollDeductions HDMF = PayrollProcess.Instance.Value.GetHDMF(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            PayrollDeductions PhilHealth = PayrollProcess.Instance.Value.GetPhilHealth(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            PayrollDeductions SSS = PayrollProcess.Instance.Value.GetSSS(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            PayrollDeductions ProvFund = PayrollProcess.Instance.Value.GetProvidentFund(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);

            if ((PhilHealth?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (PhilHealth.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.PayrollDeductions.Where(x => x.Deduction?.ID == PhilHealth.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.PayrollDeductions.Where(x => x.Deduction?.ID == PhilHealth.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = PhilHealth.Amount;
                }
                else
                    payroll.PayrollDeductions.Add(PhilHealth);
            }

            if ((HDMF?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (HDMF.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.PayrollDeductions.Where(x => x.Deduction?.ID == HDMF.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.PayrollDeductions.Where(x => x.Deduction?.ID == HDMF.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = HDMF.Amount;
                }
                else
                    payroll.PayrollDeductions.Add(HDMF);
            }

            if ((SSS?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (SSS.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.PayrollDeductions.Where(x => x.Deduction?.ID == SSS.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.PayrollDeductions.Where(x => x.Deduction?.ID == SSS.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = SSS.Amount;
                }
                else
                    payroll.PayrollDeductions.Add(SSS);
            }

            if ((ProvFund?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (ProvFund.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.PayrollDeductions.Where(x => x.Deduction?.ID == ProvFund.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.PayrollDeductions.Where(x => x.Deduction?.ID == ProvFund.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = ProvFund.Amount;
                }
                else
                    payroll.PayrollDeductions.Add(ProvFund);
            }
        }


        private void FlexiComputation(Payroll payroll, OTRequest ot, DateTime end, ScheduleType sched, DateTime starttime, DateTime breaktime, DateTime breaktimeend, DateTime endtime, DateTime startnight1, DateTime endnight1, DateTime startnight2, DateTime endnight2, PayrollDetails details, DateTime? LoginDate, DateTime? LogoutDate, float leave)
        {
            if (!LoginDate.HasValue && !LogoutDate.HasValue)
            {
                details.TotalRegularMinutes = details.TotalLeaveMinutes;
            }
            else
            {
                int totalminutes = GlobalHelper.SubtractDate(LogoutDate, LoginDate);
                if (sched.BreakTime.HasValue && sched.BreakTimeHour.HasValue)
                {
                    if ((LoginDate > breaktime && breaktimeend > LoginDate) && breaktimeend < LogoutDate)
                    {
                        totalminutes = GlobalHelper.SubtractDate(LogoutDate, breaktimeend);
                    }
                    else if (LoginDate >= breaktimeend || LogoutDate <= breaktime)
                    {
                        totalminutes = GlobalHelper.SubtractDate(LogoutDate, LoginDate);
                    }
                    else if ((LogoutDate > breaktime && breaktimeend > LogoutDate) && breaktime > LoginDate)
                    {
                        totalminutes = GlobalHelper.SubtractDate(breaktime, LoginDate);
                    }
                    else if (LoginDate < breaktime && LogoutDate > breaktimeend)
                    {
                        totalminutes = GlobalHelper.SubtractDate(LogoutDate, LoginDate) - ((int)PayrollParameters.CNBInstance.Value.Minutes * (sched.BreakTimeHour ?? 0));
                    }
                }
                else if (sched.BreakTimeHour.HasValue)
                {
                    totalminutes = totalminutes > PayrollParameters.CNBInstance.Value.DefaultHalfdayMinutes ?
                            (totalminutes < PayrollParameters.CNBInstance.Value.DefaultHalfdayMinutesWithBreaktime ?
                                PayrollParameters.CNBInstance.Value.DefaultHalfdayMinutes
                                : (totalminutes - ((int)sched.BreakTimeHour * 60)))
                            : totalminutes;
                }
                DateTime? timeotmuststart = LoginDate?.AddMinutes((sched.BreakTimeHour.HasValue ? PayrollParameters.CNBInstance.Value.TotalMinutesPerDayWithBreak : PayrollParameters.CNBInstance.Value.TotalMinutesPerDay));
                if (LogoutDate > timeotmuststart)
                {
                    totalminutes = PayrollParameters.CNBInstance.Value.TotalMinutesPerDay;
                    //compute ot
                    if ((ot?.ID ?? 0) > 0 || payroll.Personnel.AutoOT)
                    {
                        DateTime? startot = timeotmuststart;
                        DateTime? endot = LogoutDate;
                        if (end > startot)
                            details.RegularOTMinutes = GlobalHelper.SubtractDate(endot, startot);
                    }
                }

                NighDiffComputation(starttime, endtime, startnight1, endnight1, startnight2, endnight2, details, LoginDate, LogoutDate);

                details.IsPresent = true;

                details.TotalRegularMinutes = totalminutes < 0 ? 0 : totalminutes;
                details.TotalRegularMinutes += details.TotalLeaveMinutes;
                details.TotalRegularMinutes = details.TotalRegularMinutes > (int)(sched.TotalWorkingHours * 60) ? (int)(sched.TotalWorkingHours * 60) : details.TotalRegularMinutes;
            }

            if (details.ID == 0)
                payroll.PayrollDetails.Add(details);
        }

        private void MustBePresentOnly(Payroll payroll, ScheduleType sched, PayrollDetails details)
        {
            if (!sched.TimeIn.HasValue && !sched.TimeOut.HasValue)
            {
                details.TotalRegularMinutes = (sched.TotalWorkingHours ?? 0) / 60;
                details.IsPresent = true;
            }

            if (details.ID == 0)
                payroll.PayrollDetails.Add(details);
        }

        private decimal GetRate(Payroll payroll, DateTime periodStart, DateTime periodEnd, PersonnelCompensation comp, decimal rate, decimal noofdays)
        {
            if (!(comp?._Compensation?.Description?.ToLower()?.Contains("daily") ?? false))
            {
                if (payroll.Personnel._PayrollType?.Description?.ToLower()?.Contains("semi") ?? false)
                    rate /= ((periodEnd - periodStart).TotalDays > 25 ? 1 : 2);
                payroll.DailyRate = (rate / noofdays).ToDecimalPlaces(3);
            }
            else
                payroll.DailyRate = rate.ToDecimalPlaces(3);
            return rate;
        }

    }
}
