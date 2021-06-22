using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.Kiosk.Time_Edit_Request
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public DateTime? LoginDateTime { get; set; }
        public DateTime? LogoutDateTime { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();

        public bool IsExpired { get; set; }
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public TimeEditRequest TimeEditRequest { get; set; } = new TimeEditRequest();
        public List<TimeEditRequest> TimeEditRequests { get; set; } = new List<TimeEditRequest>();
    }
}