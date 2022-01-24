using ProcessLayer.Entities.CnB;
using System.Collections.Generic;

namespace WebTemplate.Models.CrewPayroll
{
    public class PayrollManagement
    {
        public CrewPayrollPeriod PayrollBase { get; set; }
        public List<CrewVessel> Vessel { get; set; }
    }
}