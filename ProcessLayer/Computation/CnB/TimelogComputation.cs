using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Entities.CnBs;
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
    public class TimelogComputation
    {
        private static TimelogComputation _instance;
        public static TimelogComputation Instance { get { if (_instance == null) _instance = new TimelogComputation(); return _instance; } }
        private List<NonWorkingDays> NonWorkingDays { get; set; }
        private List<LateDeduction> LateDeductions { get; set; }
        public PayrollType Monthly { get; set; }
        public TimelogComputation()
        {
            LateDeductions = LookupProcess.GetLateDeductions();
        }

        public List<PersonnelTimesheet> GenerateTimesheet(DateTime start, DateTime end, long? personnelID, int? departmentID)
        {
            List<PersonnelTimesheet> timesheets = new List<PersonnelTimesheet>();
            var emp = PersonnelProcess.GetListByDepartment(start, personnelID, departmentID);
            timesheets.AddRange(PersonnelProcess.GetListByDepartment(start, personnelID, departmentID)?.Select(x => new PersonnelTimesheet { Personnel = x }));
            if (timesheets?.Any() ?? false)
            {
                NonWorkingDays = NonWorkingDaysProcess.Instance.GetNonWorkingDays(start, end);

                foreach (var timesheet in timesheets)
                {
                    if (!(timesheet.Personnel._Schedules?.Any() ?? false)) throw new Exception("Cannot compute timesheet for " + timesheet.Personnel.FullName + ". No Schedule found.");
                    if (!(timesheet.Personnel._AssignedLocation?.Any() ?? false)) throw new Exception("Cannot compute timesheet for " + timesheet.Personnel.FullName + ". No Assigned Location found.");
                }
                foreach (var timesheet in timesheets)
                {
                    Compute(timesheet, start, end);
                }
                return timesheets;
            }
            else
                throw new Exception("No personnel found");
        }
        private void Compute(PersonnelTimesheet timesheet, DateTime periodStart, DateTime periodEnd)
        {
            List<TimeLog> timelogs = TimeLogProcess.Get(timesheet.Personnel.ID, periodStart.AddDays(-5), periodEnd);
            List<OTRequest> approvedotrequests = OTRequestProcess.Instance.GetApprovedOT(timesheet.Personnel.ID, periodStart, periodEnd);
            List<LeaveRequest> approvedleaverequests = LeaveRequestProcess.Instance.GetApprovedLeave(timesheet.Personnel.ID, null, periodStart, periodEnd);
            List<OuterPortRequest> approvedouterportrequests = OuterPortRequestProcess.Instance.GetApprovedOuterPort(timesheet.Personnel.ID, null, periodStart, periodEnd);
            List<HighRiskRequest> approvedhighriskrequests = HighRiskRequestProcess.Instance.GetApproved(timesheet.Personnel.ID, periodStart, periodEnd);

            DateTime start = periodStart.Date;
            DateTime end = periodEnd.Date;
            while (start <= end)
            {
                //Get Schedule
                #region schedule per day
                ScheduleType sched = GlobalHelper.GetSchedule(timesheet.Personnel._Schedules, start) ?? new ScheduleType();
                DateTime starttime = start;
                DateTime breaktime = start;
                DateTime breaktimeend = start;
                DateTime endtime = start;
                DateTime startnight1 = start + PayrollParameters.CNBInstance.NightDiffStartTime1;
                DateTime endnight1 = start + PayrollParameters.CNBInstance.NightDiffEndTime1;
                DateTime startnight2 = start + PayrollParameters.CNBInstance.NightDiffStartTime2;
                DateTime endnight2 = start + PayrollParameters.CNBInstance.NightDiffEndTime2;

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
                DateTime defbt = start + PayrollParameters.CNBInstance.DefaultBreaktime;
                DateTime defbtend = defbt.AddHours((double)PayrollParameters.CNBInstance.DefaultBreaktimeHour);
                var loc = PersonnelAssignedLocationProcess.GetCurrent(timesheet.Personnel.ID, start)?._Location ?? new Location();
                NonWorkingDays holiday = NonWorkingDays.Where(x => (((x.Yearly ?? false) && x.Day?.Month == start.Month && x.Day?.Day == start.Day) || x.Day == start) && (x.LocationID == loc.ID || (x.IsGlobal ?? false))).FirstOrDefault();

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
                #endregion

                LeaveRequest leave = approvedleaverequests.Where(x => starttime.Date >= x.StartDateTime?.Date && starttime.Date <= x.EndDateTime?.Date && x.ApprovedLeaveCredits.HasValue && x.ApprovedLeaveCredits > 0).FirstOrDefault();

                ComputedTimelog details = new ComputedTimelog
                {
                    Date = start,
                    Login = LoginDate,
                    Logout = LogoutDate,
                    Assigned = loc?.Description,
                    Schedule = GlobalHelper.GetSchedule(timesheet.Personnel._Schedules, start)
                };

                bool needTimeLog = false;
                //get outer port request on specific date
                OuterPortRequest outerPort = approvedouterportrequests.Where(x => starttime.Date >= x.StartDate && starttime.Date <= x.EndDate).FirstOrDefault();

                HighRiskRequest highRisk = approvedhighriskrequests.Where(x => starttime.Date == x.RequestDate.Date).FirstOrDefault();
                DateTime? prevDate = null;

                if ((outerPort?.ID ?? 0) > 0)
                {
                    details.isHazard = true;
                    details.HazRate = outerPort._Location?.HazardRate ?? 0;
                    details.Assigned = outerPort._Location?.Prefix;
                    needTimeLog = outerPort._Location?.RequiredTimeLog ?? false;
                    holiday = NonWorkingDays.Where(x => (((x.Yearly ?? false) && x.Day?.Month == start.Month && x.Day?.Day == start.Day) || x.Day == start) && (x.LocationID == outerPort._Location?.ID || (x.IsGlobal ?? false))).FirstOrDefault();
                }

                if ((holiday?.ID ?? 0) > 0)
                {
                    prevDate = GlobalHelper.GetPrevSchedDate(timesheet.Personnel._Schedules, starttime, NonWorkingDays, outerPort?._Location?.ID ?? loc?.ID);
                }
                if ((highRisk?.ID ?? 0) > 0)
                {
                    details.isHighRisk = true;
                    details.HighRiskRate = PayrollParameters.CNBInstance.HighRiskRate;
                }
                if(!needTimeLog && details.isHazard)
                {
                    details.NoofDays = 1;
                }
                else if (sched.AtHome ?? false)
                {
                    details.NoofDays = 1;
                }
                else if (((holiday?.ID ?? 0) > 0) || (sched?.ID ?? 0) == 0 || start.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (((holiday?.ID ?? 0) > 0) && (sched?.ID ?? 0) != 0)
                    {
                        details.HolidayDesc = holiday.Description;
                    }
                    if (LoginDate.HasValue && LogoutDate.HasValue)
                    {
                        int mins = 0;
                        OTRequest ot = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date).FirstOrDefault();
                        if ((ot?.ID ?? 0) > 0 || timesheet.Personnel.AutoOT)
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
                                mins = GlobalHelper.SubtractDate(LogoutDate, LoginDate) - (int)PayrollParameters.CNBInstance.DefaultBreaktimeMinutes;
                            }

                            if (start.DayOfWeek == DayOfWeek.Sunday && (sched?.ID ?? 0) == 0)
                            {
                                int regminutes = mins - PayrollParameters.CNBInstance.SundayTotalMinutes;
                                if (regminutes > 0
                                    && timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(-1).Date).Any()
                                    && timelogs.Where(x => x.LoginDate?.Date == starttime.AddDays(1).Date).Any())
                                {
                                    details.NoofDays = 1;
                                    details.SunOTHours = regminutes / PayrollParameters.CNBInstance.Minutes;
                                }
                                else
                                    details.NoofDays = mins / (decimal)PayrollParameters.CNBInstance.TotalMinutesPerDay;

                            }
                            else if (((holiday?.ID ?? 0) > 0))
                            {
                                int regminutes = mins - PayrollParameters.CNBInstance.HolidayTotalMinutes;
                                if (regminutes > 0
                                    && timelogs.Where(x => x.LoginDate?.Date == prevDate?.Date).Any())
                                {
                                    details.Holiday = 1;
                                    details.HolExcHours = regminutes / PayrollParameters.CNBInstance.Minutes;
                                }
                                else
                                    details.Holiday = mins / (decimal)PayrollParameters.CNBInstance.TotalMinutesPerDay; ;
                            }

                            #region Night Diff
                            if (starttime <= startnight1 && endtime >= endnight1)
                            {
                                DateTime? login = LoginDate < startnight1 ? startnight1 : LoginDate;
                                DateTime? logout = LogoutDate > endnight1 ? endnight1 : LogoutDate;

                                details.NightDiffEarlyHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;

                            }
                            if (starttime <= startnight2 && endtime >= endnight2)
                            {
                                DateTime? login = LoginDate < startnight2 ? startnight2 : LoginDate;
                                DateTime? logout = LogoutDate > endnight2 ? endnight2 : LogoutDate;

                                details.NightDiffLateHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;
                            }
                            #endregion
                        }
                    }
                    else if (((holiday?.ID ?? 0) > 0) && (sched?.ID ?? 0) != 0 && timelogs.Where(x => x.LoginDate?.Date == prevDate?.Date).Any())
                    {
                        details.NoofDays = 1;
                    }
                }
                else
                {
                    if (!sched.TimeIn.HasValue && !sched.TimeOut.HasValue) //Flexi
                    {
                        if (LoginDate.HasValue && LogoutDate.HasValue)
                        {
                            decimal totalminutes = GlobalHelper.SubtractDate(LogoutDate, LoginDate);
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
                                    totalminutes = GlobalHelper.SubtractDate(LogoutDate, LoginDate) - (PayrollParameters.CNBInstance.Minutes * (sched.BreakTimeHour ?? 0));
                                }
                            }
                            else if (sched.BreakTimeHour.HasValue)
                            {
                                totalminutes = totalminutes > PayrollParameters.CNBInstance.DefaultHalfdayMinutes ?
                                        (totalminutes < PayrollParameters.CNBInstance.DefaultHalfdayMinutesWithBreaktime ?
                                            PayrollParameters.CNBInstance.DefaultHalfdayMinutes
                                            : (totalminutes - PayrollParameters.CNBInstance.DefaultHalfdayMinutes))
                                        : totalminutes;
                            }
                            DateTime? timeotmuststart = LoginDate?.AddMinutes((sched.BreakTimeHour.HasValue ? PayrollParameters.CNBInstance.TotalMinutesPerDayWithBreak : PayrollParameters.CNBInstance.TotalMinutesPerDay));
                            if (LogoutDate > timeotmuststart && (leave?.ID ?? 0) == 0)
                            {
                                totalminutes = PayrollParameters.CNBInstance.TotalMinutesPerDay;
                                //compute ot
                                OTRequest ot = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date).FirstOrDefault();
                                if ((ot?.ID ?? 0) > 0 || timesheet.Personnel.AutoOT)
                                {
                                    DateTime? startot = timeotmuststart;
                                    DateTime? endot = LogoutDate;
                                    if (end > startot)
                                        details.RegOTHours = GlobalHelper.SubtractDate(endot, startot) / PayrollParameters.CNBInstance.Minutes;
                                }
                            }

                            #region Night Diff
                            if (starttime <= startnight1 && endtime >= endnight1)
                            {
                                DateTime? login = LoginDate < startnight1 ? startnight1 : LoginDate;
                                DateTime? logout = LogoutDate > endnight1 ? endnight1 : LogoutDate;

                                details.NightDiffEarlyHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;

                            }
                            if (starttime <= startnight2 && endtime >= endnight2)
                            {
                                DateTime? login = LoginDate < startnight2 ? startnight2 : LoginDate;
                                DateTime? logout = LogoutDate > endnight2 ? endnight2 : LogoutDate;

                                details.NightDiffLateHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;
                            }
                            #endregion
                        }
                    }
                    else //Regular Hours
                    {
                        if (LoginDate.HasValue && LogoutDate.HasValue)
                        {
                            int totalRegularMinutes = GlobalHelper.SubtractDate(endtime, starttime) - GlobalHelper.SubtractDate(breaktimeend, breaktime);
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
                                        late = (lateded.DeductedHours ?? 0) * (int)PayrollParameters.CNBInstance.Minutes;
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

                            if ((leave?.ID ?? 0) == 0)
                            {
                                //compute ot
                                OTRequest ot = approvedotrequests.Where(x => x.RequestDate.Date == starttime.Date).FirstOrDefault();
                                if ((ot?.ID ?? 0) > 0 || timesheet.Personnel.AutoOT)
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
                                            details.RegOTHours = totalotminutesremain / PayrollParameters.CNBInstance.Minutes;
                                        }
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
                                    {
                                        undertime = GlobalHelper.SubtractDate(endtime, LogoutDate) - (sched.BreakTimeHour ?? 0);
                                    }
                                    else if (breaktime.AddHours(sched.BreakTimeHour ?? 0) < LogoutDate)
                                        undertime = GlobalHelper.SubtractDate(endtime, LogoutDate);
                                    else
                                        undertime = GlobalHelper.SubtractDate(endtime, breaktime.AddHours(sched.BreakTimeHour ?? 0));
                                }
                                else
                                    undertime = GlobalHelper.SubtractDate(endtime, LogoutDate);
                                totalRegularMinutes -= undertime;

                            }
                            
                            totalRegularMinutes -= late;

                            details.NoofDays = totalRegularMinutes / (decimal)PayrollParameters.CNBInstance.TotalMinutesPerDay;

                            #region Night Diff
                            if (starttime <= startnight1 && endtime >= endnight1)
                            {
                                DateTime? login = LoginDate < startnight1 ? startnight1 : LoginDate;
                                DateTime? logout = LogoutDate > endnight1 ? endnight1 : LogoutDate;

                                details.NightDiffEarlyHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;
                            }
                            
                            if (starttime <= startnight2 && endtime >= endnight2)
                            {
                                DateTime? login = LoginDate < startnight2 ? startnight2 : LoginDate;
                                DateTime? logout = LogoutDate > endnight2 ? endnight2 : LogoutDate;

                                details.NightDiffLateHours = GlobalHelper.SubtractDate(logout, login) / PayrollParameters.CNBInstance.Minutes;
                            }
                            #endregion
                        }
                    }
                }
                timesheet.ComputedTimelogs.Add(details);
                start = start.AddDays(1);
            } 
        }
    }
}
