using DBUtilities;
using ProcessLayer.Computation.CnB;
using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Entities.Kiosk;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.Kiosk;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProcessLayer.Processes.CnB
{
    public sealed class PayrollProcess
    {
        public static readonly Lazy<PayrollProcess> Instance = new Lazy<PayrollProcess>(() => new PayrollProcess());
        private PayrollProcess() { }
        public bool BaseOnly { get; set; } = false;
        internal PayrollPeriod BaseConverter(DataRow dr)
        {
            var pbase = new PayrollPeriod
            {
                ID = dr["ID"].ToLong(),
                PayPeriod = dr["Pay Period"].ToString(),
                StartDate = dr["Start Date"].ToDateTime(),
                EndDate = dr["End Date"].ToDateTime(),
                AdjustedStartDate = dr["Adjusted Start Date"].ToNullableDateTime(),
                AdjustedEndDate = dr["Adjusted End Date"].ToNullableDateTime(),
                Type = dr["Type"].ToPayrollSheet(),
                PayrollStatus = LookupProcess.GetPayrollStatus(dr["Status ID"].ToByte()),
                PreparedBy = LookupProcess.GetUser(dr["Prepared By"].ToNullableInt()),
                PreparedOn = dr["Prepared On"].ToNullableDateTime(),
                CheckedBy = LookupProcess.GetUser(dr["Checked By"].ToNullableInt()),
                CheckedOn = dr["Checked On"].ToNullableDateTime(),
                ApprovedBy = LookupProcess.GetUser(dr["Approved By"].ToNullableInt()),
                ApprovedOn = dr["Approved On"].ToNullableDateTime()
            };

            if(!BaseOnly)
               pbase.Payrolls = GetPayrollList(pbase.ID);

            return pbase;
        }
        internal Payroll PayrollConverter(DataRow dr)
        {
            var p = new Payroll
            {
                ID = dr["ID"].ToLong(),
                PayrollPeriodID = dr["Payroll Period ID"].ToLong(),
                Position = dr["Position"].ToString(),
                Department = dr["Department"].ToString(),
                Personnel = PersonnelProcess.Get(dr["Personnel ID"].ToLong(), true),
                NOofDays = dr["NO of Days"].ToNullableDecimal() ?? 0,
                Allowance = dr["Allowance"].ToNullableDecimal() ?? 0,
                DailyRate = dr["Daily Rate"].ToNullableDecimal() ?? 0,
                RegularOTPay = dr["Regular OT Pay"].ToNullableDecimal() ?? 0,
                SundayOTPay = dr["Sunday OT Pay"].ToNullableDecimal() ?? 0,
                HolidayExcessOTPay = dr["Holiday Excess OT Pay"].ToNullableDecimal() ?? 0,
                HolidayOTPay = dr["Holiday OT Pay"].ToNullableDecimal() ?? 0,
                NightDifferentialPay = dr["Night Differential Pay"].ToNullableDecimal() ?? 0,
                RegularOTRate = dr["Regular OT Rate"].ToDecimal(),
                SundayOTRate = dr["Sunday OT Rate"].ToDecimal(),
                HolidayRegularOTRate = dr["Holiday Regular OT Rate"].ToDecimal(),
                HolidayExcessOTRate = dr["Holiday Excess OT Rate"].ToDecimal(),
                RegularOTAllowance = dr["Regular OT Allowance"].ToDecimal(),
                SundayOTAllowance = dr["Sunday OT Allowance"].ToDecimal(),
                HolidayRegularOTAllowance = dr["Holiday Regular OT Allowance"].ToDecimal(),
                HolidayExcessOTAllowance = dr["Holiday Excess OT Allowance"].ToDecimal(),
                RegularOTAllowancePay = dr["Regular OT Allowance Pay"].ToDecimal(),
                SundayOTAllowancePay = dr["Sunday OT Allowance Pay"].ToDecimal(),
                HolidayOTAllowancePay = dr["Holiday OT Allowance Pay"].ToDecimal(),
                HolidayOTExcessAllowancePay = dr["Holiday OT Excess Allowance Pay"].ToDecimal(),
                TotalAllowance = dr["Total Allowance"].ToDecimal(),
                TotalOTPay = dr["Total OT Pay"].ToDecimal(),
                TotalOTAllowance = dr["Total OT Allowance"].ToDecimal(),
                AdditionalPayRate = dr["Additional Pay Rate"].ToNullableDecimal() ?? 0,
                AdditionalAllowanceRate = dr["Additional Allowance Rate"].ToNullableDecimal() ?? 0,
                TotalAdditionalPay = dr["Total Additional Pay"].ToNullableDecimal() ?? 0,
                TotalAdditionalAllowancePay = dr["Total Additional Allowance Pay"].ToNullableDecimal() ?? 0,
                TotalDeductions = dr["Total Deductions"].ToNullableDecimal() ?? 0,
                Tax = dr["Tax"].ToNullableDecimal() ?? 0,
                OutstandingVale = dr["Outstanding Vale"].ToNullableDecimal() ?? 0,
                TotalAdditionalOvertimePay = dr["Total Additional Overtime Pay"].ToNullableDecimal() ?? 0,
                TotalAdditionalOvertimeAllowancePay = dr["Total Additional Overtime Allowance Pay"].ToNullableDecimal() ?? 0,
                BasicPay = dr["Basic Pay"].ToDecimal(),
                GrossPay = dr["Gross Pay"].ToDecimal(),
                NetPay = dr["Net Pay"].ToDecimal(),
                HighRiskAllowanceRate = dr["High Risk Allowance Rate"].ToNullableDecimal() ?? 0,
                HighRiskPayRate = dr["High Risk Pay Rate"].ToNullableDecimal() ?? 0,
                TotalHighRiskAllowancePay = dr["Total High Risk Allowance Pay"].ToNullableDecimal() ?? 0,
                TotalHighRiskPay = dr["Total High Risk Pay"].ToNullableDecimal() ?? 0,
                ExtensionAllowanceRate = dr["Extension Allowance Rate"].ToNullableDecimal() ?? 0,
                ExtensionRate = dr["Extension Rate"].ToNullableDecimal() ?? 0,
                TotalExtensionPay = dr["Total Extension Pay"].ToNullableDecimal() ?? 0,
                TotalExtensionAllowancePay = dr["Total Extension Allowance Pay"].ToNullableDecimal() ?? 0
            };
            p.PayrollDetails = GetPayrollDetails(p.ID);
            p.PayrollDeductions = GetPayrollDeductions(p.ID);
            p.LoanDeductions = GetLoanDeductions(p.ID);
            return p;
        }
        internal LoanDeductions LoanDeductionConverter(DataRow dr)
        {
            LoanDeductions loanDeductions =  new LoanDeductions
            {
                ID = dr["ID"].ToLong(),
                Amount = dr["Amount"].ToNullableDecimal() ?? 0,
                PersonnelLoan = PersonnelLoanProcess.Instance.Value.Get(dr["Personnel Loan ID"].ToLong())
            };

            try
            {
                loanDeductions.CutoffStart = dr["Start Date"].ToDateTime();
                loanDeductions.CutoffEnd = dr["End Date"].ToDateTime();
            }
            catch { }

            return loanDeductions;
        }
        internal PayrollDeductions PayrollDeductionConverter(DataRow dr)
        {
            return new PayrollDeductions
            {
                ID = dr["ID"].ToLong(),
                Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                Amount = dr["Amount"].ToNullableDecimal(),
                PS = dr["PS"].ToNullableDecimal(),
                ES = dr["ES"].ToNullableDecimal(),
                EC = dr["EC"].ToNullableDecimal()
            };
        }
        internal PayrollDetails DetailConverter(DataRow dr)
        {
            return new PayrollDetails
            {
                ID = dr["ID"].ToLong(),
                LoggedDate = dr["Logged Date"].ToDateTime(),
                TotalRegularMinutes = dr["Total Regular Minutes"].ToNullableShort() ?? 0,
                TotalLeaveMinutes = dr["Total Leave Minutes"].ToNullableShort() ?? 0,
                Location = LocationProcess.Instance.Value.Get(dr["Location ID"].ToNullableByte()),
                IsHoliday = dr["Is Holiday"].ToBoolean(),
                IsSunday = dr["Is Sunday"].ToBoolean(),
                RegularOTMinutes = dr["Regular OT Minutes"].ToInt(),
                SundayOTMinutes = dr["Sunday OT Minutes"].ToInt(),
                IsHighRisk = dr["Is High Risk"].ToNullableBoolean() ?? false,
                HolidayRegularOTMinutes = dr["Holiday Regular OT Minutes"].ToInt(),
                HolidayExcessOTMinutes = dr["Holiday Excess OT Minutes"].ToInt(),
                IsPresent = dr["Is Present"].ToNullableBoolean() ?? false,
                IsNonTaxable = dr["Is Non Taxable"].ToNullableBoolean() ?? false,
                IsExtended = dr["Is Extended"].ToNullableBoolean() ?? false
            };
        }
        public List<PayrollPeriod> GetPayrollBases(DateTime? startDate, DateTime? endDate, PayrollSheet payrollSheet, int page, int gridCount, out int PageCount)
        {
            var parameters = new Dictionary<string, object> {
                { "@StartDate", startDate},
                { "@EndDate", endDate},
                { "@Type", payrollSheet},
                { "@PageNumber", page},
                { "@GridCount", gridCount},
            };
            var outparameters = new List<OutParameters> {
                { "@PageCount", SqlDbType.Int}
            };
            var pbase = new List<PayrollPeriod>();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.FilterPayrollBase", ref outparameters, parameters))
                {
                    BaseOnly = true;
                    pbase = ds.GetList(BaseConverter);
                    BaseOnly = false;
                    PageCount = outparameters.Get("@PageCount").ToInt();
                }
            }
            return pbase;
        }
        public PayrollPeriod GetPayrollBase(long? id, bool baseOnly = false)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id}
            };

            var pbase = new PayrollPeriod();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPayrollBase", parameters))
                {
                    BaseOnly = baseOnly;
                    pbase = ds.Get(BaseConverter);
                    BaseOnly = false;
                }
            }
            return pbase;
        }
        public PayrollPeriod GetPayrollBase(DBTools db, long? id)
        {
            using (var ds = db.ExecuteReader("cnb.GetPayrollBase", new Dictionary<string, object> { { "@ID", id } }))
            {
                BaseOnly = true;
                var pbase = ds.Get(BaseConverter);
                BaseOnly = false;
                return pbase;
            }
        }
        public PayrollDeductions GetPhilHealth(long? personnelID, decimal gross, byte cutoff, DateTime date)
        {
            var pd = new PayrollDeductions();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPhilHealth", new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Gross", gross},
                    { "@CutOff", cutoff },
                    { "@Date", date } }))
                {
                    pd = ds.Get(dr =>
                    {
                        return new PayrollDeductions
                        {
                            ID = dr["ID"].ToLong(),
                            Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                            Amount = dr["PS"].ToNullableDecimal(),
                            PS = dr["PS"].ToNullableDecimal(),
                            ES = dr["ES"].ToNullableDecimal()
                        };
                    });
                }
            }
            return pd;
        }
        public byte GetMonthlyTaxScheduleID()
        {
            using (var db = new DBTools())
            {
                return db.ExecuteScalar("lookup.GetMonthlyTaxScheduleID").ToNullableByte() ?? 0;
            }

        }
        public PayrollDeductions GetHDMF(long? personnelID, decimal gross, byte cutoff, DateTime date)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetHDMF", new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Gross", gross},
                    { "@CutOff", cutoff },
                    { "@Date", date } }))
                {
                    return ds.Get(dr =>
                    {
                        return new PayrollDeductions
                        {
                            ID = dr["ID"].ToLong(),
                            Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                            Amount = dr["PS"].ToNullableDecimal(),
                            PS = dr["PS"].ToNullableDecimal(),
                            ES = dr["ES"].ToNullableDecimal()
                        };
                    });
                }
            }
        }
        public PayrollDeductions GetSSS(long? personnelID, decimal gross, byte cutoff, DateTime date)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetSSS", new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Gross", gross},
                    { "@CutOff", cutoff },
                    { "@Date", date } }))
                {
                    return ds.Get(dr =>
                    {
                        return new PayrollDeductions
                        {
                            ID = dr["ID"].ToLong(),
                            Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                            Amount = dr["PS"].ToNullableDecimal(),
                            PS = dr["PS"].ToNullableDecimal(),
                            ES = dr["ES"].ToNullableDecimal(),
                            EC = dr["EC"].ToNullableDecimal()
                        };
                    });
                }
            }
        }
        public PayrollDeductions GetProvidentFund(long? personnelID, decimal gross, byte cutoff, DateTime date)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetProvidentFund", new Dictionary<string, object> {
                    { "@PersonnelID", personnelID },
                    { "@Gross", gross},
                    { "@CutOff", cutoff },
                    { "@Date", date } }))
                {
                    return ds.Get(dr =>
                    {
                        return new PayrollDeductions
                        {
                            ID = dr["ID"].ToLong(),
                            Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                            Amount = dr["PS"].ToNullableDecimal(),
                            PS = dr["PS"].ToNullableDecimal(),
                            ES = dr["ES"].ToNullableDecimal()
                        };
                    });
                }
            }
        }
        public List<LoanDeductions> GetLoanDeductions(long payrollID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetLoanDeductions", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    return ds.GetList(LoanDeductionConverter);
                }
            }
        }
        public List<LoanDeductions> GetPersonnelLoanDeductions(long personnelLoanId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetLoanDeductions", new Dictionary<string, object> { { "@PersonnelLoanID", personnelLoanId } }))
                {
                    return ds.GetList(LoanDeductionConverter);
                }
            }
        }
        public List<PayrollDeductions> GetPayrollDeductions(long payrollID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPayrollDeductions", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    return ds.GetList(PayrollDeductionConverter);
                }
            }
        }
        public List<PayrollDetails> GetPayrollDetails(long payrollID)
        {
            var paydet = new List<PayrollDetails>();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPayrollDetails", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    paydet = ds.GetList(DetailConverter);
                }
            }
            return paydet;
        }
        public List<Payroll> GetPayrollList(long payrollperiodid, long? id = null, long? personnelId = null)
        {
            var pay = new List<Payroll>();

            using (var db = new DBTools())
            {
                pay = GetPayrollList(db, payrollperiodid, id, personnelId);
            }
            return pay;
        }
        public List<Payroll> GetPayrollList(DBTools db, long payrollperiodid, long? id = null, long? personnelId = null)
        {
            using (var ds = db.ExecuteReader("cnb.GetPayroll", new Dictionary<string, object> { { "@PayrollPeriodID", payrollperiodid },
                { "@ID", id },
                { "@PersonnelID", personnelId } }))
            {
                return ds.GetList(PayrollConverter);
            }
        }
        public Payroll GetPayroll(long id)
        {
            var pay = new Payroll();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPayroll", new Dictionary<string, object> { { "@ID", id } }))
                {
                    pay = ds.Get(PayrollConverter);
                }
            }
            return pay;
        }
        public bool ValidatePayrollGeneration(string payperiod, PayrollSheet type)
        {

            using (var db = new DBTools())
            {
                var res = db.ExecuteScalar("cnb.ValidatePayroll", new Dictionary<string, object> { { "@PayPeriod", payperiod }, { "@Type", type } });
                if (res != null)
                    return res.ToInt() == 0;
            }
            throw new Exception("Unable to generate payroll");
        }
        public Payroll UpdatePayrollStatus(Payroll payroll, int userid)
        {
            var parameters = new Dictionary<string, object> {
                { "@PayrollID", payroll.ID },
                { "@LogBy", userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery("cnb.UpdatePayrollStatus", parameters);
                return GetPayroll(payroll.ID);
            }
        }
        public decimal? GetTax(long personnelId, PayrollSheet type, decimal? gross, byte cutoff, DateTime date)
        {

            var parameters = new Dictionary<string, object> {
                { "@PersonnelID", personnelId },
                { "@Gross", gross },
                { "@CutOff", cutoff },
                { "@Type", type },
                { "@Date", date }
            };

            using (var db = new DBTools())
            {
                return db.ExecuteScalar("cnb.GetTax", parameters).ToNullableDecimal();
            }
        }
        public PayrollPeriod CreateOrUpdate(PayrollPeriod payrollBase, int userid)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    SavePayrollBase(payrollBase, userid, db);

                    SavePayrollList(payrollBase.Payrolls, payrollBase.ID, payrollBase.Type, payrollBase.StartDate, payrollBase.EndDate, userid, db);


                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollBackTransaction();
                    throw ex;
                }
            }
            return payrollBase;
        }
        private void SavePayrollBase(PayrollPeriod payrollBase, int userid, DBTools db)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@PayPeriod", payrollBase.PayPeriod },
                { "@StartDate", payrollBase.StartDate },
                { "@EndDate", payrollBase.EndDate },
                { "@AdjustedStartDate", payrollBase.AdjustedStartDate },
                { "@AdjustedEndDate", payrollBase.AdjustedEndDate },
                { "@Type", payrollBase.Type },
                { "@StatusId", payrollBase.PayrollStatus?.ID },
                { "@LogBy", userid }
            };

            var outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt }
            };

            db.ExecuteNonQuery("cnb.CreateOrUpdatePayrollPeriod", ref outparameters, parameters);
            payrollBase.ID = outparameters.Get("@ID").ToLong();
        }
        private void SavePayrollList(List<Payroll> payrolls, long baseId, PayrollSheet type, DateTime cutoffStart, DateTime cutoffEnd, int userid, DBTools db)
        {
            foreach (var payroll in payrolls)
            {
                if (payroll.ID > 0 && !payroll.Modified)
                {
                    DeletePayroll(db, payroll.ID, userid);
                    DeletePayrollDetails(db, userid, payrollId: payroll.ID);
                }
                else
                    SaveIndividualPayroll(db, baseId, type, cutoffStart, cutoffEnd, userid, payroll);
            }
        }

        private void SaveComputedLeaveCredits(DBTools db, List<LeaveRequest> leaveRequests, List<ComputedLeaveCredits> computedLeaveCredits, int userId)
        {
            foreach(LeaveRequest leaveRequest in leaveRequests)
            {
                LeaveRequestProcess.Instance.Value.UpdateComputedLeaveCredits(db, leaveRequest, userId);
            }
            foreach (ComputedLeaveCredits computedLeaveCredit in computedLeaveCredits)
            {
                ComputedLeaveCreditsProcess.Instance.Value.CreateOrUpdate(db, computedLeaveCredit, userId);
            }
        }

        private void DeletePayrollDetails(DBTools db, int userid, long? payrollId = null, long? detailsId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", detailsId },
                { "@PayrollID", payrollId },
                { "@LogBy", userid }
            };

            db.ExecuteNonQuery("cnb.DeletePayrollDetails", parameters);
        }
        private void DeletePayroll(DBTools db, long id, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", id },
                {  "@LogBy", userid }
            };

            db.ExecuteNonQuery("cnb.DeletePayroll", parameters);
        }
        private void SaveIndividualPayroll(DBTools db, long baseId, PayrollSheet type, DateTime cutoffStart, DateTime cutoffEnd, int userid, Payroll payroll)
        {
            SavePayroll(db, baseId, userid, payroll);

            foreach (var details in payroll.PayrollDetails)
            {
                if(details.ID > 0 && !details.Modified)
                    DeletePayrollDetails(db, userid, detailsId: details.ID);
                else
                    SavePayrollDetails(userid, db, payroll.ID, details);
            }

            foreach (var deduction in payroll.PayrollDeductions)
            {
                if (deduction.ID > 0 && !deduction.Modified)
                    DeletePayrollDeduction(db, userid, deduction.ID);
                else
                    SavePayrollDeductions(userid, db, payroll.ID, deduction);
            }

            foreach (var loan in payroll.LoanDeductions)
            {
                if (loan.ID > 0 && !loan.Modified)
                {
                    DeleteLoanDeduction(db, loan.ID, userid);
                    DeleteLoanPaymentMethod(db, loan.ID, userid);
                }
                else
                {
                    SaveLoanDeductions(userid, db, payroll.ID, loan);
                    if(type == PayrollSheet.B) PersonnelLoanProcess.Instance.Value.UpdateAmount(db, loan.PersonnelLoan?.ID ?? 0, loan.Amount, userid);
                    SaveLoanPaymentMethod(userid, db, loan);
                }
            }

            if (payroll.OutstandingVale > 0)
                SaveOutstandingVale(cutoffStart, cutoffEnd, userid, db, payroll.Personnel.ID, payroll.OutstandingVale, payroll.ID);

            SaveComputedLeaveCredits(db, payroll.LeaveRequests, payroll.ComputedLeaveCredits, userid);
        }

        private void DeleteLoanPaymentMethod(DBTools db, long loanDeductionId, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@LoanDeductionID", loanDeductionId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.DeleteLoanPaymentMethod", parameters);
        }

        private void RevertAnyOutstandingVale(DBTools db, long payrollId, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@PayrollID", payrollId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.RevertAnyOutstandingVale", parameters);
        }

        private void DeleteLoanDeduction(DBTools db, long loanDeductionId, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", loanDeductionId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.DeleteLoanDeduction", parameters);
        }

        private void DeletePayrollDeduction(DBTools db, int userid, long payrollDeductionId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", payrollDeductionId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.DeletePayrollDeductions", parameters);
        }


        private void SavePayroll(DBTools db, long baseId, int userid, Payroll payroll)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@PayrollPeriodId", baseId },
                    { "@PersonnelID", payroll.Personnel.ID },
                    { "@Position", payroll.Position },
                    { "@Department", payroll.Department },
                    { "@TotalDeductions", payroll.TotalDeductions },
                    { "@Allowance", payroll.Allowance },
                    { "@Tax", payroll.Tax },
                    { "@RegularOTPay", payroll.RegularOTPay },
                    { "@SundayOTPay", payroll.SundayOTPay },
                    { "@HolidayOTPay", payroll.HolidayOTPay },
                    { "@NightDifferentialPay", payroll.NightDifferentialPay },
                    { "@NOofDays", payroll.NOofDays },
                    { "@DailyRate", payroll.DailyRate },
                    { "@OutstandingVale", payroll.OutstandingVale },
                    { "@TotalAllowance", payroll.TotalAllowance },
                    { "@RegularOTRate", payroll.RegularOTRate },
                    { "@RegularOTAllowance", payroll.RegularOTAllowance },
                    { "@SundayOTRate", payroll.SundayOTRate },
                    { "@SundayOTAllowance", payroll.SundayOTAllowance },
                    { "@HolidayRegularOTRate", payroll.HolidayRegularOTRate },
                    { "@HolidayRegularOTAllowance", payroll.HolidayRegularOTAllowance },
                    { "@HolidayExcessOTRate", payroll.HolidayExcessOTRate },
                    { "@HolidayExcessOTAllowance", payroll.HolidayExcessOTAllowance },
                    { "@HolidayExcessOTPay", payroll.HolidayExcessOTPay },
                    { "@RegularOTAllowancePay", payroll.RegularOTAllowancePay },
                    { "@SundayOTAllowancePay", payroll.SundayOTAllowancePay },
                    { "@HolidayOTAllowancePay", payroll.HolidayOTAllowancePay },
                    { "@HolidayOTExcessAllowancePay", payroll.HolidayOTExcessAllowancePay },
                    { "@AdditionalPayRate", payroll.AdditionalPayRate },
                    { "@AdditionalAllowanceRate", payroll.AdditionalAllowanceRate },
                    { "@TotalAdditionalPay", payroll.TotalAdditionalPay },
                    { "@TotalAdditionalAllowancePay", payroll.TotalAdditionalAllowancePay },
                    { "@TotalAdditionalOvertimePay", payroll.TotalAdditionalOvertimePay },
                    { "@TotalAdditionalOvertimeAllowancePay", payroll.TotalAdditionalOvertimeAllowancePay },
                    { "@HighRiskPayRate", payroll.HighRiskPayRate },
                    { "@HighRiskAllowanceRate", payroll.HighRiskAllowanceRate },
                    { "@TotalHighRiskPay", payroll.TotalHighRiskPay },
                    { "@TotalHighRiskAllowancePay", payroll.TotalHighRiskAllowancePay },
                    { "@ExtensionRate", payroll.ExtensionRate },
                    { "@ExtensionAllowanceRate", payroll.ExtensionAllowanceRate },
                    { "@TotalExtensionPay", payroll.TotalExtensionPay },
                    { "@TotalExtensionAllowancePay", payroll.TotalExtensionAllowancePay },
                    { "@BasicPay", payroll.BasicPay },
                    { "@TotalOTPay", payroll.TotalOTPay },
                    { "@TotalOTAllowance", payroll.TotalOTAllowance },
                    { "@GrossPay", payroll.GrossPay },
                    { "@NetPay", payroll.NetPay },
                    { "@LogBy", userid }
                };

            List<OutParameters> outparameters = new List<OutParameters>
                {
                    { "@ID", SqlDbType.BigInt, payroll.ID }
                };

            db.ExecuteNonQuery("cnb.CreateOrUpdatePayroll", ref outparameters, parameters);
            payroll.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveLoanDeductions(int userid, DBTools db, long payrollId, LoanDeductions loan)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@PayrollID", payrollId },
                { "@PersonnelLoanID", loan.PersonnelLoan?.ID },
                { "@Amount", loan.Amount },
                { "@LogBy", userid }
            };

            List<OutParameters> outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, loan.ID }
            };
            db.ExecuteNonQuery("cnb.CreateOrUpdateLoanDeductions", ref outparameters, parameters);
            loan.ID = outparameters.Get("@ID").ToLong();
        }
        private void SavePayrollDeductions(int userid, DBTools db, long payrollId, PayrollDeductions deduction)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@PayrollID", payrollId },
                { "@DeductionID", deduction.Deduction?.ID },
                { "@Amount", deduction.Amount },
                { "@PS", deduction.PS },
                { "@ES", deduction.ES },
                { "@EC", deduction.EC },
                { "@LogBy", userid }
            };

            List<OutParameters> outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, deduction.ID }
            };
            db.ExecuteNonQuery("cnb.CreateOrUpdatePayrollDeductions", ref outparameters, parameters);
            deduction.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveLoanPaymentMethod(int userid, DBTools db, LoanDeductions loan)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@PersonnelLoanID", loan.PersonnelLoan?.ID },
                    { "@LoanDeductionID", loan.ID },
                    { "@Amount", loan.Amount },
                    { "@LogBy", userid }
                };

            db.ExecuteNonQuery("cnb.CreateOrUpdateLoanPaymentMethod", parameters);
        }
        private void SaveOutstandingVale(DateTime cutoffStart, DateTime cutoffEnd, int userid, DBTools db, long personnelId, decimal outStandingVale, long payrollId)
        {
            DateTime datestart = new DateTime(cutoffEnd.Year, cutoffEnd.Month, 15);
            if (datestart < cutoffEnd)
                datestart.AddMonths(1).AddDays(-15);

            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@PersonnelID", personnelId },
                { "@PayrollID", payrollId },
                { "@Amount", outStandingVale },
                { "@CutOffStart", cutoffStart },
                { "@CutoffEnd", cutoffEnd },
                { "@DateStart", datestart },
                { "@LogBy", userid }
            };
            db.ExecuteNonQuery("cnb.ChargeRemainingToVale", parameters);
        }
        private void SavePayrollDetails(int userid, DBTools db, long payrollId, PayrollDetails details)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@PayrollID", payrollId },
                { "@LoggedDate", details.LoggedDate },
                { "@TotalRegularMinutes", details.TotalRegularMinutes },
                { "@TotalLeaveMinutes", details.TotalLeaveMinutes },
                { "@LocationID", details.Location?.ID },
                { "@IsHoliday", details.IsHoliday },
                { "@IsSunday", details.IsSunday },
                { "@RegularOTMinutes", details.RegularOTMinutes },
                { "@SundayOTMinutes", details.SundayOTMinutes },
                { "@HolidayRegularOTMinutes", details.HolidayRegularOTMinutes },
                { "@HolidayExcessOTMinutes", details.HolidayExcessOTMinutes },
                { "@IsHighRisk", details.IsHighRisk },
                { "@IsNonTaxable", details.IsNonTaxable },
                { "@IsPresent", details.IsPresent },
                { "@IsExtended", details.IsExtended },
                { "@LogBy", userid }
            };

            List<OutParameters> outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, details.ID }
            };
            db.ExecuteNonQuery("cnb.CreateOrUpdatePayrollDetails", ref outparameters, parameters);
            details.ID = outparameters.Get("@ID").ToLong();
        }
        public PayrollPeriod GeneratePayroll(PayrollPeriod payrollBase, PayrollSheet payrollSheet, int userid)
        {
            payrollBase.PayPeriod = payrollBase.StartDate.ToString("MMddyyyy") + payrollBase.EndDate.ToString("MMddyyyy");
            payrollBase.Type = payrollSheet;
            payrollBase.PayrollStatus = new Lookup { ID = 1, Description = "Pending" };

            if (ValidatePayrollGeneration(payrollBase.PayPeriod, payrollSheet))
            {
                return CreateOrUpdate(PayrollComputation.Instance.Value.GeneratePayroll(payrollBase), userid);
            }
            else
            {
                throw new Exception("Payroll already generated for this cut-off");
            }
        }
        public PayrollPeriod GetPersonnelPayroll(long personnelId, int month, int year, int cutoff)
        {

            PayrollPeriod payrollBase = GetPayrollDate(month, year, cutoff, PayrollSheet.B);

            payrollBase.PayPeriod = payrollBase.StartDate.ToString("MMddyyyy") + payrollBase.EndDate.ToString("MMddyyyy");

            using (var db = new DBTools())
            {

                using (var ds = db.ExecuteReader("cnb.GetPayrollPeriod", new Dictionary<string, object> { { "@PayPeriod", payrollBase.PayPeriod } }))
                {
                    BaseOnly = true;
                    payrollBase = ds.Get(BaseConverter);
                    BaseOnly = false;
                }

                payrollBase.Payrolls = GetPayrollList(db, payrollBase.ID, personnelId: personnelId);
            }

            return payrollBase;
        }
        public PayrollPeriod GetPersonnelPayroll(long personnelId, long payPeriodId)
        {
            using (var db = new DBTools())
            {
                PayrollPeriod payrollBase = GetPayrollBase(db, payPeriodId);
                payrollBase.Payrolls = GetPayrollList(db, payrollBase.ID, personnelId: personnelId);
                return payrollBase;
            }
        }
        public bool HasPayrollAhead(PayrollSheet type, DateTime date)
        {
            using (var db = new DBTools())
            {
                return (db.ExecuteScalar("cnb.HasPayrollAhead", new Dictionary<string, object> {
                        { "@PayrollType", type },
                        { "@StartDate", date },
                     }).ToNullableByte() ?? 0) > 0;
            }
        }
        public PayrollPeriod GeneratePayroll(int month, int year, int cutoff, PayrollSheet payrollSheet, int userid)
        {
            PayrollPeriod payrollBase = GetPayrollDate(month, year, cutoff, payrollSheet);
            if (HasPayrollAhead(payrollSheet, payrollBase.StartDate))
                throw new Exception("Unable to generate payroll");

            if (payrollBase.EndDate > DateTime.Now)
                throw new Exception("Unable to generate future payroll");


            payrollBase.PayPeriod = payrollBase.StartDate.ToString("MMddyyyy") + payrollBase.EndDate.ToString("MMddyyyy");
            payrollBase.Type = payrollSheet;
            payrollBase.PayrollStatus = new Lookup { ID = 1, Description = "Pending" };

            if (ValidatePayrollGeneration(payrollBase.PayPeriod, payrollBase.Type))
            {
                return CreateOrUpdate(PayrollComputation.Instance.Value.GeneratePayroll(payrollBase), userid);
            }
            else
            {
                throw new Exception("Payroll already generated for this cut-off");
            }
        }
        public PayrollPeriod GetPayrollDate(int month, int year, int cutoff, PayrollSheet payrollSheet)
        {
            PayrollPeriod payrollBase = new PayrollPeriod();

            var s = 1;
            var e = 1;

            if (payrollSheet == PayrollSheet.A)
            {
                s = PayrollParameters.CNBInstance.Value.FirstCutoffStart;
                e = PayrollParameters.CNBInstance.Value.SecondCutoffEnd;
            }
            else
            {
                switch (cutoff)
                {
                    case 1:
                        s = PayrollParameters.CNBInstance.Value.FirstCutoffStart;
                        e = PayrollParameters.CNBInstance.Value.FirstCutoffEnd;
                        break;
                    case 2:
                        s = PayrollParameters.CNBInstance.Value.SecondCutoffStart;
                        e = PayrollParameters.CNBInstance.Value.SecondCutoffEnd;
                        break;
                }
            }
            payrollBase.StartDate = new DateTime(year - (month == 1 ? 1 : 0), month - ((e < s && month > 1) ? 1 : 0), s);
            payrollBase.EndDate = new DateTime(year, month, e);

            return payrollBase;
        }
        public Payroll RecomputePayroll(long payrollId, int userid)
        {
            var payroll = GetPayroll(payrollId);
            var payrollBase = GetPayrollBase(payroll.PayrollPeriodID, true);

            using (var db = new DBTools())
            {
                db.StartTransaction();
                try {
                    foreach (var loan in payroll.LoanDeductions)
                    {
                        if (payrollBase.Type == PayrollSheet.B)
                            PersonnelLoanProcess.Instance.Value.RevertAmount(db, loan.PersonnelLoan?.ID ?? 0, loan.ID, userid);
                    }
                    RevertAnyOutstandingVale(db, payroll.ID, userid);
                    db.CommitTransaction();
                }
                catch(Exception ex) { db.RollBackTransaction(); throw ex; }

            }

            PayrollComputation.Instance.Value.Recompute(payroll, payrollBase);

            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    SaveIndividualPayroll(db, payrollBase.ID, payrollBase.Type, payrollBase.StartDate, payrollBase.EndDate, userid, payroll);
                    db.CommitTransaction();
                }
                catch (Exception ex)
                {
                    db.RollBackTransaction();
                    throw ex;
                }
            }

            return payroll;
        }
    }

}
