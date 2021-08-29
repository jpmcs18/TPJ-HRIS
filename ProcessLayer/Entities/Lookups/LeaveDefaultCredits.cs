namespace ProcessLayer.Entities
{
    public class LeaveDefaultCredits
    {
        public int ID { get; set; }
        public byte LeaveTypeID { get; set; }
        public byte MinYearsInService { get; set; }
        public byte? MaxYearsInService { get; set; }
        public float Credits { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
