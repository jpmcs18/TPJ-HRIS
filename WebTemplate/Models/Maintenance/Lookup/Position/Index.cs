using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using e = ProcessLayer.Entities;

namespace WebTemplate.Models.Maintenance.Lookup.Position
{
    public class Index : BaseModel
    {
        public List<e.Position> Positions { get; set; }
    }
}