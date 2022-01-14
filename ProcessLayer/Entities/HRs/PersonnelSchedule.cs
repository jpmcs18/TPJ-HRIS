using System;

namespace ProcessLayer.Entities
{
    public class PersonnelSchedule : PersonnelBase
    {
        public int? SundayScheduleID { get; set; }
        public int? MondayScheduleID { get; set; }
        public int? TuesdayScheduleID { get; set; }
        public int? WednesdayScheduleID { get; set; }
        public int? ThursdayScheduleID { get; set; }
        public int? FridayScheduleID { get; set; }
        public int? SaturdayScheduleID { get; set; }
        public DateTime? EffectivityDate { get; set; }

        public ScheduleType _SundaySchedule { get; set; }

        public ScheduleType _MondaySchedule { get; set; }

        public ScheduleType _TuesdaySchedule { get; set; }

        public ScheduleType _WednesdaySchedule { get; set; }

        public ScheduleType _ThursdaySchedule { get; set; }

        public ScheduleType _FridaySchedule { get; set; }

        public ScheduleType _SaturdaySchedule { get; set; }
    }
}
