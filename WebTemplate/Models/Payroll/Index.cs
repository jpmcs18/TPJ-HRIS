using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Payroll
{
    public class Index : BaseModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<ProcessLayer.Entities.CnB.PayrollPeriod> Payrolls { get; set; }
    }
}