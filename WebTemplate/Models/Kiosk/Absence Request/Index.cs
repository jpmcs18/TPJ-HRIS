using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Kiosk.Absence_Request
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();

        public bool IsExpired { get; set; }
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public AbsenceRequest AbsenceRequest { get; set; } = new AbsenceRequest();
        public List<AbsenceRequest> AbsenceRequests { get; set; } = new List<AbsenceRequest>();
    }
}