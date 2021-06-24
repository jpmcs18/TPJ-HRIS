using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Groups
{
    public class Personnels
    {
        public List<ProcessLayer.Entities.Personnel> Personnel { get; set; } = new List<ProcessLayer.Entities.Personnel>();
    }
}