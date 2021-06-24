using ProcessLayer.Entities.Kiosk;
using System;
using System.Collections.Generic;

namespace WebTemplate.Models.RequestsApproval.Requests_Approver
{
    public class Index : BaseModel
    {
        public string Personnel { get; set; }
        public long ID { get; set; }
        public int DepartmentID { get; set; }
    }
}