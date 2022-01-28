using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.CompensationApproval
{
    public class Index : BaseModel
    {
        //public bool? Status { get; set; } = false;
        public List<PersonnelCompensation> PersonnelCompensations { get; set; } = new List<PersonnelCompensation>();
    }
}