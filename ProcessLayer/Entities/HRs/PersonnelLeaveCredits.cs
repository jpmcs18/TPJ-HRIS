using System;
using System.ComponentModel;

namespace ProcessLayer.Entities.HR
{
    public class PersonnelLeaveCredit : PersonnelBase
    {
        [DisplayName("Leave Type ID")]
        public byte? LeaveTypeID { get; set; }
        [DisplayName("Leave Credits")]
        public float? LeaveCredits { get; set; }
        public short? YearValid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public LeaveType _LeaveType { get; set; }
    }
}
