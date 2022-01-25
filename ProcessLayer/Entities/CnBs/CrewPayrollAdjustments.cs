using System;

namespace ProcessLayer.Entities.CnB
{
    public class CrewPayrollAdjustments
    {
        public long ID { get; set; }
        public long PrecCrewPayrollDetailsID { get; set; }
        public DateTime LoggedDate { get; set; }
        public int DeductRegularMinutes { get; set; }
        public bool IsDeduction { get; set; }
    }
}

