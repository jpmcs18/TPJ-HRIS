using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers
{
    public static class GlobalHelper
    {

        public static int SubtractDate(DateTime? end, DateTime? start)
        {
            return (int)((end - start)?.TotalMinutes ?? 0);
        }

        public static DateTime? GetPrevSchedDate(List<PersonnelSchedule> scheds, DateTime date, List<NonWorkingDays> NonWorkingDays, int? locationId)
        {

            date = date.AddDays(-1);
            
            if((NonWorkingDays.Where(x => (((x.Yearly ?? false) && x.Day?.Month == date.Month && x.Day?.Day == date.Day) || x.Day == date) && (x.LocationID == locationId || (x.IsGlobal ?? false)))?.Any() ?? false) || !HasSchedule(scheds, date))
            {
                return GetPrevSchedDate(scheds, date);
            }
            
            return date;
        }
        public static DateTime? GetPrevSchedDate(List<PersonnelSchedule> scheds, DateTime date)
        {
            date = date.AddDays(-1);
            if (!scheds.Where(x => x.EffectivityDate <= date).Any())
                return null;

            if (HasSchedule(scheds, date))
                return date;
            else
                return GetPrevSchedDate(scheds, date);
        }
        public static ScheduleType GetSchedule(List<PersonnelSchedule> schedules, DateTime date)
        {
            var schedule = schedules.Where(x => x.EffectivityDate <= date).OrderByDescending(x => x.EffectivityDate).FirstOrDefault();
            return GetSchedule(schedule, date);
        }
        public static ScheduleType GetSchedule(PersonnelSchedule schedule, DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return schedule?._SundaySchedule;
                case DayOfWeek.Monday:
                    return schedule?._MondaySchedule;
                case DayOfWeek.Tuesday:
                    return schedule?._TuesdaySchedule;
                case DayOfWeek.Wednesday:
                    return schedule?._WednesdaySchedule;
                case DayOfWeek.Thursday:
                    return schedule?._ThursdaySchedule;
                case DayOfWeek.Friday:
                    return schedule?._FridaySchedule;
                case DayOfWeek.Saturday:
                    return schedule?._SaturdaySchedule;
                default:
                    return null;
            }
        }
        public static bool HasSchedule(List<PersonnelSchedule> schedules, DateTime date)
        {
            var schedule = schedules.Where(x => x.EffectivityDate <= date).OrderByDescending(x => x.EffectivityDate).FirstOrDefault();
            return HasSchedule(schedule, date);
        }
        public static bool HasSchedule(PersonnelSchedule schedule, DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return (schedule?._SundaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Monday:
                    return (schedule?._MondaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Tuesday:
                    return (schedule?._TuesdaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Wednesday:
                    return (schedule?._WednesdaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Thursday:
                    return (schedule?._ThursdaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Friday:
                    return (schedule?._FridaySchedule?.ID ?? 0) > 0;
                case DayOfWeek.Saturday:
                    return (schedule?._SaturdaySchedule?.ID ?? 0) > 0;
                default:
                    return false;
            }
        }
        public static NonWorkingDays GetNonWorking(List<NonWorkingDays> nonWorkingDays, DateTime date)
        {
            return nonWorkingDays.Where(x => x.Day?.Date == date.Date).FirstOrDefault();
        }
    }
}
