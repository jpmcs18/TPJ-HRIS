using ProcessLayer.Entities;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Helpers.ObjectParameter.Payroll
{
    public sealed class CrewPayrollParameters
    {
        public static readonly CrewPayrollParameters Instance = new Lazy<CrewPayrollParameters>(() => new CrewPayrollParameters()).Value;
        private IEnumerable<Parameters> Parameters { get; set; }
        private CrewPayrollParameters()
        {
            Parameters = ParametersProcess.Instance.GetParameters(ParametersTag.CrewPayroll);

            CrewCutOff1 = GetParameters(nameof(CrewCutOff1)).ToInt();
            CrewCutOff2 = GetParameters(nameof(CrewCutOff2)).ToInt();
            CrewSundayRate = GetParameters(nameof(CrewSundayRate)).ToDecimal();
            CrewHolidayRate = GetParameters(nameof(CrewHolidayRate)).ToDecimal();
        }
        private object GetParameters(string desc)
        {
            return Parameters?.Where(x => x.Description == desc).Select(x => x.Value).FirstOrDefault();
        }
        public int CrewCutOff1 { get; set; }
        public int CrewCutOff2 { get; set; }
        public decimal CrewHolidayRate { get; set; }
        public decimal CrewRegularRate { get { return (decimal)0.5; } }
        public decimal CrewSundayRate { get; set; }
    }
}
