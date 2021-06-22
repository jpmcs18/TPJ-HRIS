using ProcessLayer.Entities;
using System.Collections.Generic;

namespace WebTemplate.Models.Maintenance.Lookup.KioskApprover
{
    public class Index : BaseModel
    {
        public int? DepartmentID { get; set; }

        public string Name { get; set; }
        public string Personnel { get; set; }
        public string Approver { get; set; }

        public List<ProcessLayer.Entities.Personnel> Personnels { get; set; } = new List<ProcessLayer.Entities.Personnel>();
        public List<KioskApprovers> KioskApprovers { get; set; } = new List<KioskApprovers>();
        public KioskApprovers KioskApprover { get; set; } = new KioskApprovers();
    }
}