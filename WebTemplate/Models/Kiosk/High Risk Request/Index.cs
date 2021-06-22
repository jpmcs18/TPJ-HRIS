using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Kiosk.High_Risk_Request
{
    public class Index : BaseModel
    {
        public string Key { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public ProcessLayer.Entities.Personnel Personnel { get; set; } = new ProcessLayer.Entities.Personnel();

        public bool IsExpired { get; set; }
        public bool IsPending { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsCancelled { get; set; }
        public bool All { get; set; }

        public HighRiskRequest HighRiskRequest { get; set; } = new HighRiskRequest();
        public List<HighRiskRequest> HighRiskRequests { get; set; } = new List<HighRiskRequest>();
    }
}