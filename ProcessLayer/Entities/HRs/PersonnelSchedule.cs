using System;

namespace ProcessLayer.Entities
{
    public class PersonnelSchedule : PersonnelBase
    {
        public byte? SundayScheduleID { get; set; }
        public byte? MondayScheduleID { get; set; }
        public byte? TuesdayScheduleID { get; set; }
        public byte? WednesdayScheduleID { get; set; }
        public byte? ThursdayScheduleID { get; set; }
        public byte? FridayScheduleID { get; set; }
        public byte? SaturdayScheduleID { get; set; }
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
