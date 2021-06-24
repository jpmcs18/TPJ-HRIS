using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.RequestsApproval.Leave_Request
{
    public class Index : BaseModel
    {
        public string Personnel { get; set; }
        public byte? LeaveTypeID { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }
        public List<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();

        public LeaveType _LeaveType = new LeaveType();

        public bool IsExpired { get; set; } = true;
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public List<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}