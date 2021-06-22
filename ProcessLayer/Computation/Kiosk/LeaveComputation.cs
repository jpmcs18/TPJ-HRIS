using ProcessLayer.Entities;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Computation.Kiosk
{
    public class LeaveComputation
    {
        private LeaveComputation _instance;

        public LeaveComputation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LeaveComputation();

                return _instance;
            }
        }

        public int ComputeTotalLeaveToUse(DateTime startdate, DateTime enddate, Personnel personnel)
        {
            int total = 0;
            var nonworking = NonWorkingDaysProcess.Instance.GetNonWorkingDays(startdate, enddate);

            if((enddate - startdate).TotalHours <= 9)
            {
                var sched = GetSchedule(personnel._Schedules.Where(x => x.EffectivityDate <= startdate).OrderByDescending(x => x.EffectivityDate).FirstOrDefault(), startdate);
                var login = startdate.Date + sched.TimeIn;

            }

            while(startdate < enddate)
            {

            }

            return total;
        }
        private ScheduleType GetSchedule(PersonnelSchedule schedule, DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return schedule._SundaySchedule;
                case DayOfWeek.Monday:
                    return schedule._MondaySchedule;
                case DayOfWeek.Tuesday:
                    return schedule._TuesdaySchedule;
                case DayOfWeek.Wednesday:
                    return schedule._WednesdaySchedule;
                case DayOfWeek.Thursday:
                    return schedule._ThursdaySchedule;
                case DayOfWeek.Friday:
                    return schedule._FridaySchedule;
                case DayOfWeek.Saturday:
                    return schedule._SaturdaySchedule;
                default:
                    return null;
            }
        }
    }
}
