using ProcessLayer.Helpers.Enumerable;
using System;
using System.Collections.Generic;

namespace ProcessLayer.Entities.CnB
{
    public class CrewPayrollPeriod
    {
        public long ID { get; set; }
        public string PayPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StatusID { get; set; }
        public PayrollSheet Type { get; set; }
        public DateTime AdjustedStartDate { get; set; }
        public DateTime AdjustedEndDate { get; set; }
        public Lookup PayrollStatus { get; set; }

        public DateTime? PreparedOn { get; set; }
        public string PreparedBy { get; set; }

        public DateTime? CheckedOn { get; set; }
        public string CheckedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }

        public List<CrewVessel> CrewVessel { get; set; } = new List<CrewVessel>();
    }
}

