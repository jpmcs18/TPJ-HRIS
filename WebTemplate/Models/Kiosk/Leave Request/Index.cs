using ProcessLayer.Entities;
using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.Kiosk.Leave_Request
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }
        public int? LeaveTypeID { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();
        public List<LeaveType> LeaveTypes { get; set; } = new List<LeaveType>();

        public bool IsExpired { get; set; }
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public LeaveRequest LeaveRequest { get; set; } = new LeaveRequest();
        public List<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }
}