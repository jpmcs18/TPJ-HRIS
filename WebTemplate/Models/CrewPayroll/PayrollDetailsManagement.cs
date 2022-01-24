using ProcessLayer.Entities.CnB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.CrewPayroll
{
    public class PayrollDetailsManagement
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CrewPayrollPeriod PayrollPeriod { get; set; }
        public ProcessLayer.Entities.CnB.CrewPayroll Payroll { get; set; }
        public List<CrewPayrollDetails> PayrollDetails { get; set; }
        public List<CrewPayrollDeductions> PayrollDeductions { get; set; }
        public List<CrewLoanDeductions> LoanDeductions { get; set; }
    }
}