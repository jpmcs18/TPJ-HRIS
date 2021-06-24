using ProcessLayer.Helpers.Enumerable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities.CnB
{
    public class PayrollPeriod
    {
        public long ID { get; set; }
        public string PayPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DisplayStartDate { 
            get { 
                var date = new DateTime(StartDate.Year, StartDate.Month, 1);
                return StartDate.Day == 1 ? StartDate : (StartDate.Day > 15 ? date.AddMonths(1) : date.AddDays(14)); 
            } 
        }
        public DateTime DisplayEndDate
        {
            get
            {
                var date = new DateTime(EndDate.Year, EndDate.Month, 1);
                return EndDate.Day == date.AddMonths(1).AddDays(-1).Day ? EndDate
                        : (EndDate.Day < 15 ? date.AddDays(14) : date.AddMonths(1).AddDays(-1));
            }
        }
        public DateTime? AdjustedStartDate { get; set; }
        public DateTime? AdjustedEndDate { get; set; }
        public Lookup PayrollStatus { get; set; }
        public PayrollSheet Type { get; set; }
        
        public DateTime? PreparedOn { get; set; }
        public string PreparedBy { get; set; }

        public DateTime? CheckedOn { get; set; }
        public string CheckedBy { get; set; }

        public DateTime? ApprovedOn { get; set; }
        public string ApprovedBy { get; set; }

        public List<Payroll> Payrolls { get; set; } = new List<Payroll>();

    }
}
