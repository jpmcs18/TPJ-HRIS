using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.RequestsApproval.High_Risk
{
    public class Index : BaseModel
    {
        public string Personnel { get; set; }
        public int? LeaveTypeID { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }

        public bool IsExpired { get; set; } = true;
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public List<HighRiskRequest> HighRiskRequests { get; set; } = new List<HighRiskRequest>();
    }
}