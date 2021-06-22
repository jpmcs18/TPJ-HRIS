using ProcessLayer.Entities.CnB;
using System.Collections.Generic;

namespace WebTemplate.Models.Payroll
{
    public class PayrollManagement
    {
        public PayrollPeriod PayrollBase { get; set; }
        public List<ProcessLayer.Entities.CnB.Payroll> Payrolls { get; set; }
    }
}