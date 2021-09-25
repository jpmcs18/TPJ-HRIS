using ProcessLayer.Helpers.ObjectParameter.Payroll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities.HR
{
    public class PersonnelLoan : PersonnelBase
    {
        public byte? LoanID { get; set; }
        public decimal? Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal? Amortization { get; set; }
        public float? PaymentTerms { get; set; }
        public bool? PayrollDeductible { get; set; }
        public byte? WhenToDeduct { get; set; }
        public DateTime? DateStart { get; set; }
        public string Remarks { get; set; }
        public long? PayrollID { get; set; }
        public decimal RemainingAmount { get { return (Amount ?? 0) - PaidAmount; } }
        public Loan _Loan { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime? DateEnd
        {
            get
            {
                DateTime? end = null;
                if (RemainingAmount == 0)
                {
                    end = new DateTime(LastModified?.Year??0, LastModified?.Month??0, 15);
                    if (end < LastModified)
                    {
                        end?.AddMonths(1).AddDays(-15);
                    }

                    return end;
                }
                
                int toPay = (int)(RemainingAmount / Amortization);

                end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15); 
                
                if(WhenToDeduct == 2)
                {
                    end?.AddMonths(1).AddDays(-15);
                }

                if (end < DateTime.Now)
                {
                    end?.AddDays(1).AddMonths(1).AddDays(-1);

                    if (WhenToDeduct != 1)
                    {
                        end?.AddDays(-15);
                    }
                }

                for(int i = 1; i < toPay; i++)
                {
                    if(WhenToDeduct == 3)
                    {
                        if(end?.Day == 15)
                        {
                            end?.AddDays(1).AddMonths(1).AddDays(-1).AddDays(-15);
                        }
                        else
                        {
                            end?.AddDays(15);
                        }

                    }
                    else
                    {
                        end?.AddDays(1).AddMonths(1).AddDays(-1);
                    }

                }

                return end;
            }
        }
    }
}
