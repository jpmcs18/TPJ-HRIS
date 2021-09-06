namespace ProcessLayer.Entities
{
    public class LeaveDefaultCredits
    {
        public int ID { get; set; }
        public byte LeaveTypeID { get; set; }
        public float MinYearsInService { get; set; }
        public float? MaxYearsInService { get; set; }
        public float Credits { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
