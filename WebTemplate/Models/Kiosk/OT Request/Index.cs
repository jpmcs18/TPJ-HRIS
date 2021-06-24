using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.Kiosk.OT_Request
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();

        public bool IsExpired { get; set; }
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public OTRequest OTRequest { get; set; } = new OTRequest();
        public List<OTRequest> OTRequests { get; set; } = new List<OTRequest>();
    }
}