using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.PersonnelApproval
{
    public class Index : BaseModel
    {
        public List<ProcessLayer.Entities.Personnel> Personnel { get; set; }
    }
}