using ProcessLayer.Entities.CnB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Payroll
{
    public class PayrollDetailsManagement
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PayrollPeriod PayrollPeriod { get; set; }
        public ProcessLayer.Entities.CnB.Payroll Payroll { get; set; }
        public List<PayrollDetails> PayrollDetails { get; set; }
        public List<PayrollDeductions> PayrollDeductions { get; set; }
        public List<LoanDeductions> LoanDeductions { get; set; }
    }
}