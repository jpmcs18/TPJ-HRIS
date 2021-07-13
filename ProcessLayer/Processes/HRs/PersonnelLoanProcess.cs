using DBUtilities;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.HR
{
    public class PersonnelLoanProcess
    {
        private static PersonnelLoanProcess _instance;
        public static PersonnelLoanProcess Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PersonnelLoanProcess();
                return _instance;
            }
        }
        internal bool IsPersonnelLoanOnly = false;

        public PersonnelLoan Converter(DataRow dr)
        {
            var pl = new PersonnelLoan {
                ID = dr["ID"].ToLong(),
                PersonnelID = dr["Personnel ID"].ToNullableLong(),
                LoanID = dr["Loan ID"].ToNullableByte(),
                Amount = dr["Amount"].ToNullableDecimal(),
                PaidAmount = dr["Paid Amount"].ToNullableDecimal(),
                Amortization = dr["Amortization"].ToNullableDecimal(),
                PaymentTerms = dr["Payment Terms"].ToNullableInt(),
                PayrollDeductible = dr["Payroll Deductible"].ToNullableBoolean(),
                WhenToDeduct = dr["When to Deduct"].ToNullableByte(),
                Remarks = dr["Remarks"].ToString(),
            };
                try {
                pl.PayrollID = dr["Payroll ID"].ToNullableLong();
            }

            catch { }

            if (!IsPersonnelLoanOnly)
            {
                pl._Loan = LookupProcess.GetLoan(pl.LoanID);
            }

            return pl;
        }

        public PersonnelLoan Get(long id)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@ID", id }
                };

                using (var ds = db.ExecuteReader("hr.GetPersonnelLoan", parameters))
                {
                    return ds.Get(Converter);
                }
            }
        }

        public List<PersonnelLoan> GetList(long personnelID)
        {
            var pl = new List<PersonnelLoan>();
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@PersonnelID", personnelID }
                };

                using (var ds = db.ExecuteReader("hr.GetPersonnelLoan", parameters))
                {
                    pl = ds.GetList(Converter);
                }
            }
            return pl;
        }

        public List<PersonnelLoan> GetDeductibleAmount(long payrollID, long personnelID, DateTime date, int? id = null, bool? isGovernmentLoan = null)
        {
            var pl = new List<PersonnelLoan>();
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Date", date },
                    { "@ID", id },
                    { "@IsGovernmentLoan", isGovernmentLoan },
                    { "@PayrollID", payrollID }
                };

                using (var ds = db.ExecuteReader("hr.GetLoanAmountToBeDeduct", parameters))
                {
                    pl = ds.GetList(Converter);
                }
            }
            return pl;
        }

        public List<PersonnelLoan> GetPayrollBGovernmentLoanDeductions(long personnelID, DateTime start, DateTime end)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Start", start },
                    { "@End", end }
                };

                using (var ds = db.ExecuteReader("cnb.GetPayrollBGovernmentLoanDeductions", parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }

        public void RevertAmount(DBTools dB, long id, long loanDeductionId, int userId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", id },
                { "@LoanDeductionID", loanDeductionId},
                { "@LogBy", userId }
            };

            dB.ExecuteNonQuery("hr.RevertPersonnelLoanAmount", parameters);
        }

        public void UpdateAmount(DBTools dB, long id, decimal amount, int userId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", id },
                { "@Amount", amount},
                { "@LogBy", userId }
            };

            dB.ExecuteNonQuery("hr.UpdatePersonnelLoanAmount", parameters);
        }

        public PersonnelLoan CreateOrUpdate(PersonnelLoan personnelLoan, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@PersonnelID", personnelLoan.PersonnelID },
                    { "@LoanID", personnelLoan.LoanID },
                    { "@Amount", personnelLoan.Amount },
                    { "@PaidAmount", personnelLoan.PaidAmount },
                    { "@Amortization", personnelLoan.Amortization},
                    { "@PaymentTerms", personnelLoan.PaymentTerms },
                    { "@PayrollDeductible", personnelLoan.PayrollDeductible },
                    { "@WhenToDeduct", personnelLoan.WhenToDeduct },
                    { "@Remarks", personnelLoan.Remarks },
                    { "@LogBy", userid }
                };

                var outparameters = new List<OutParameters> {
                    { "@ID", SqlDbType.BigInt, personnelLoan.ID }
                };

                db.ExecuteNonQuery("hr.CreateOrUpdatePersonnelLoan", ref outparameters, parameters);
                personnelLoan.ID = outparameters.Get("@ID").ToLong();
                personnelLoan._Loan = LookupProcess.GetLoan(personnelLoan.LoanID);
            }

            return personnelLoan;
        }

        public void Delete(PersonnelLoan personnelLoan, int userid)
        {

            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@ID", personnelLoan.ID },
                    { "@LogBy", userid }
                };
                
                db.ExecuteNonQuery("hr.DeletePersonnelLoan", parameters);
            }
        }

        public void MergeAdditionalToExistingLoan(long PersonnelLoanID, long AdditionalLoanID, decimal Amount, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@PersonnelLoanID", PersonnelLoanID },
                    { "@AdditionalLoanID", AdditionalLoanID },
                    { "@Amount", Amount },
                    { "@LogBy", userid }
                };

                db.ExecuteNonQuery("hr.MergeAdditionalToExistingPersonnelLoan", parameters);
            }
        }

        public void InsertAdditionalLoanToPersonnelLoan(long additionalLoanID, PersonnelLoan personnelLoan, int userid)
        {
            using (var db = new DBTools())
            {
                var parameters = new Dictionary<string, object> {
                    { "@AdditionalLoanID", additionalLoanID },
                    { "@PersonnelID", personnelLoan.PersonnelID },
                    { "@LoanID", personnelLoan.LoanID },
                    { "@Amount", personnelLoan.Amount },
                    { "@Amortization", personnelLoan.Amortization },
                    { "@PaymentTerms", personnelLoan.PaymentTerms },
                    { "@PayrollDeductible", personnelLoan.PayrollDeductible },
                    { "@WhenToDeduct", personnelLoan.WhenToDeduct },
                    { "@Remarks", personnelLoan.Remarks },
                    { "@LogBy", userid }
                };

                db.ExecuteNonQuery("hr.InsertAdditionalToPersonnelLoan", parameters);
            }
        }
    }
}
