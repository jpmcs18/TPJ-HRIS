using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Kiosk.Outer_Port_Request
{
    public class Index : BaseModel
    {

        public string Key { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndingDateTime { get; set; }

        public int? LocationID { get; set; }

        public ProcessLayer.Entities.Lookup Location { get; set; }
        public ProcessLayer.Entities.Personnel Approver { get; set; }

        public bool IsCancelled { get; set; }
        public bool All { get; set; }


        public List<OuterPortRequest> OuterPortRequests { get; set; } = new List<OuterPortRequest>();
        public OuterPortRequest OuterPortRequest { get; set; } = new OuterPortRequest();
    }
}