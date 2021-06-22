using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.RequestsApproval.Outer_Port_Request
{
    public class Index : BaseModel
    {
        public string Personnel { get; set; }
        public DateTime? LoginDateTime { get; set; }
        public DateTime? LogoutDateTime { get; set; }

        public bool IsExpired { get; set; } = true;
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public List<OuterPortRequest> OuterPortRequests { get; set; } = new List<OuterPortRequest>();
    }
}