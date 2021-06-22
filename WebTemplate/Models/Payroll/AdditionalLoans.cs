using ProcessLayer.Entities.CnB;
using ProcessLayer.Entities.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.Payroll
{
    public class AdditionalLoans
    {
        public List<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();
        public List<AdditionalLoanForApproval> AdditionalLoanForApproval { get; set; } = new List<AdditionalLoanForApproval>();
    }
}