using System.Collections.Generic;
using System.Linq;

namespace ProcessLayer.Entities.CnB
{
    public class CrewPayroll
    {
        public long ID { get; set; }
        public long CrewPayrollPeriodID { get; set; }
        public long PersonnelID { get; set; }
        public decimal HolidayPay { get; set; }
        public decimal  NoofDays { get; set; }
        public decimal BasicPay { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public bool Recomputed { get; set; }
        public int VesselID { get; set; }
        public Vessel Vessel { get; set; }
        public bool Modified { get; set; } = false;

        public Personnel Personnel { get; set; }
        public string FullName { get { return Personnel?.LastName + ", " + Personnel?.FirstName; } }

        public List<CrewPayrollDetails> CrewPayrollDetails { get; set; } = new List<CrewPayrollDetails>();
        public List<CrewPayrollDeductions> CrewPayrollDeductions { get; set; } = new List<CrewPayrollDeductions>();
        public List<CrewLoanDeductions> CrewLoanDeductions { get; set; } = new List<CrewLoanDeductions>();

        public decimal PagibigFund { get { return CrewPayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibig")).Sum(x => x.Amount) ?? 0; } }
        public decimal SSSLoan { get { return CrewLoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("sssloan")).Sum(x => x.Amount); } }
        public decimal PagibigCalamityLoan { get { return CrewLoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibigcalamity")).Sum(x => x.Amount); } }
        public decimal PagibigLoan { get { return CrewLoanDeductions.Where(x => x.PersonnelLoan._Loan.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("pagibigloan")).Sum(x => x.Amount); } }
        public decimal SSS { get { return CrewPayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("sss")).Sum(x => x.Amount) ?? 0; } }
        public decimal ProvidentFund { get { return CrewPayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("provident")).Sum(x => x.Amount) ?? 0; } }
        public decimal PhilHealth { get { return CrewPayrollDeductions.Where(x => x.Deduction.Description.Replace("-", "").Replace(" ", "").ToLower().Contains("philhealth")).Sum(x => x.Amount) ?? 0; } }

    }
}

