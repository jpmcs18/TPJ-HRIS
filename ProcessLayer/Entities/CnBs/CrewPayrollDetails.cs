using System;

namespace ProcessLayer.Entities.CnB
{
    public class CrewPayrollDetails
    {
        public long ID { get; set; }
        public long CrewPayrollID { get; set; }
        public DateTime LoggedDate { get; set; }
        public int TotalRegularMinutes { get; set; }
        public decimal DailyRate { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsSunday { get; set; }
        public bool IsAdjusted { get; set; }
        public bool IsNonTaxable { get; set; }
        public bool IsAdditionalsOnly { get; set; }
        public bool IsCorrected { get; set; }
        public bool Modified { get; set; } = false;
        public int PostiionID { get; set; }
        public Position Position { get; set; }
        public int VesselID { get; set; }
        public Vessel Vessel { get; set; }
    }
}

