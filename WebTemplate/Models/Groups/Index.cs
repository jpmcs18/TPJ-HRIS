using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Groups
{
    public class Index : BaseModel
    {
        public List<ProcessLayer.Entities.PersonnelGroup> PersonnelGroups { get; set; } = new List<ProcessLayer.Entities.PersonnelGroup>();
    }
}