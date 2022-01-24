using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.CrewPayroll
{
    public class Index : BaseModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ProcessLayer.Entities.CnB.CrewPayrollPeriod> Payrolls { get; set; }
    }
}