using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.RequestsApproval.OT_Request
{
    public class Index : BaseModel
    {
        public string Personnel { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }
        public int? OtType { get; set; }

        public bool IsExpired { get; set; } = true;
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public OTRequest OTRequest { get; set; } = new OTRequest();
        public List<OTRequest> OTRequests { get; set; } = new List<OTRequest>();
    }
}