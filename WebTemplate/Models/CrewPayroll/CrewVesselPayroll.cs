using ProcessLayer.Entities.CnB;
using System.Collections.Generic;

namespace WebTemplate.Models.CrewPayroll
{
    public class CrewVesselList
    {
        public CrewPayrollPeriod PayrollBase { get; set; }
        public List<CrewVessel> Vessel { get; set; }
    }

    public class CrewPayrollList
    {
        public CrewPayrollPeriod PayrollBase { get; set; }
        public ProcessLayer.Entities.Vessel Vessel { get; set; }
        public List<ProcessLayer.Entities.CnB.CrewPayroll> CrewPayrolls { get; set; }
    }
}