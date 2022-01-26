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
using System.Linq;

namespace ProcessLayer.Processes.CnB
{
    public sealed class CrewPayrollProcess
    {
        public static readonly CrewPayrollProcess Instance = new CrewPayrollProcess();
        private CrewPayrollProcess() { }
        public bool BaseOnly { get; set; } = false;
        public bool VesselOnly { get; set; } = false;
        internal CrewPayrollPeriod BaseConverter(DataRow dr)
        {
            var pbase = new CrewPayrollPeriod
            {
                ID = dr["ID"].ToLong(),
                PayPeriod = dr["Pay Period"].ToString(),
                StartDate = dr["Start Date"].ToDateTime(),
                EndDate = dr["End Date"].ToDateTime(),
                AdjustedStartDate = dr["Adjusted Start Date"].ToDateTime(),
                AdjustedEndDate = dr["Adjusted End Date"].ToDateTime(),
                Type = dr["Type"].ToPayrollSheet(),
                PayrollStatus = LookupProcess.GetPayrollStatus(dr["Status ID"].ToInt()),
                PreparedBy = LookupProcess.GetUser(dr["Prepared By"].ToNullableInt()),
                PreparedOn = dr["Prepared On"].ToNullableDateTime(),
                CheckedBy = LookupProcess.GetUser(dr["Checked By"].ToNullableInt()),
                CheckedOn = dr["Checked On"].ToNullableDateTime(),
                ApprovedBy = LookupProcess.GetUser(dr["Approved By"].ToNullableInt()),
                ApprovedOn = dr["Approved On"].ToNullableDateTime()
            };

            if (!BaseOnly)
                pbase.CrewVessel = GetCrewVessel(pbase.ID);

            return pbase;
        }
        internal CrewPayroll CrewPayrollConverter(DataRow dr)
        {
            var p = new CrewPayroll
            {
                ID = dr["ID"].ToLong(),
                CrewPayrollPeriodID = dr["Crew Payroll Period ID"].ToLong(),
                Personnel = PersonnelProcess.Get(dr["Personnel ID"].ToLong(), true),
                NoOfDays = dr["No of Days"].ToNullableDecimal() ?? 0,
                Tax = dr["Tax"].ToNullableDecimal() ?? 0,
                BasicPay = dr["Basic Pay"].ToDecimal(),
                GrossPay = dr["Gross Pay"].ToDecimal(),
                NetPay = dr["Net Pay"].ToDecimal(),
                HolidayPay = dr["Holiday Pay"].ToDecimal(),
                PersonnelID = dr["Personnel ID"].ToLong(),
                TotalDeductions = dr["Total Deductions"].ToDecimal(),
                VesselID = dr["Vessel ID"].ToInt(),

            };
            p.CrewPayrollDetails = GetCrewPayrollDetails(p.ID);
            p.CrewPayrollDeductions = GetCrewPayrollDeductions(p.ID);
            p.CrewLoanDeductions = GetCrewLoanDeductions(p.ID);
            p.Vessel = VesselProcess.Instance.Get(p.VesselID);
            return p;
        }

        internal CrewVessel CrewVesselConverter(DataRow dr)
        {
            CrewVessel crewVessel = new CrewVessel()
            {
                PayPeriodID = dr["PayPeriodID"].ToLong(),
                Vessel = VesselProcess.Instance.Converter(dr)
            };
            if (!VesselOnly)
            {
                crewVessel.CrewPayrolls = GetCrewPayrolls(crewVessel.PayPeriodID, crewVessel.Vessel.ID);
            }
            return crewVessel;
        }

        internal CrewLoanDeductions CrewLoanDeductionConverter(DataRow dr)
        {
            CrewLoanDeductions loanDeductions = new CrewLoanDeductions
            {
                ID = dr["ID"].ToLong(),
                Amount = dr["Amount"].ToNullableDecimal() ?? 0,
                PersonnelLoan = PersonnelLoanProcess.Instance.Get(dr["Personnel Loan ID"].ToLong())
            };

            return loanDeductions;
        }

        internal CrewPayrollDeductions CrewPayrollDeductionConverter(DataRow dr)
        {
            return new CrewPayrollDeductions
            {
                ID = dr["ID"].ToLong(),
                Deduction = LookupProcess.GetDeduction(dr["Deduction ID"].ToNullableInt()),
                Amount = dr["Amount"].ToNullableDecimal() ?? 0,
                PS = dr["PS"].ToNullableDecimal() ?? 0,
                ES = dr["ES"].ToNullableDecimal() ?? 0,
                EC = dr["EC"].ToNullableDecimal() ?? 0
            };
        }

        internal CrewPayrollDetails CrewPayrollDetailConverter(DataRow dr)
        {
            return new CrewPayrollDetails
            {
                ID = dr["ID"].ToLong(),
                LoggedDate = dr["Logged Date"].ToDateTime(),
                TotalRegularMinutes = dr["Total Regular Minutes"].ToNullableShort() ?? 0,
                IsHoliday = dr["Is Holiday"].ToBoolean(),
                IsSunday = dr["Is Sunday"].ToBoolean(),
                IsNonTaxable = dr["Is Non Taxable"].ToNullableBoolean() ?? false,
                DailyRate = dr["Daily Rate"].ToDecimal(),
                CrewPayrollID = dr["Crew Payroll ID"].ToLong(),
                IsAdjusted = dr["Is Adjusted"].ToBoolean(),
                IsAdditionalsOnly = dr["Is Additionals Only"].ToNullableBoolean() ?? false,
                IsCorrected = dr["Is Corrected"].ToNullableBoolean() ?? false,
                PostiionID = dr["Position ID"].ToInt(),
                Position = PositionProcess.Instance.Get(dr["Position ID"].ToLong()),
                VesselID = dr["Vessel ID"].ToInt(),
                Vessel = VesselProcess.Instance.Get(dr["Vessel ID"].ToLong())
            };
        }

        public List<CrewVessel> GetCrewVessel(long payperiodId, bool isVesselOnly = false)
        {
            using (var db = new DBTools())
            {
                return GetCrewVessel(db, payperiodId, isVesselOnly);

            }
        }
        public CrewVessel GetCrewVessel(long payperiodId, int vesselId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewVessel", new Dictionary<string, object> { { "@PayPeriodID", payperiodId }, { "@VesselID", vesselId } }))
                {
                    return ds.Get(CrewVesselConverter);
                }
            }
        }
        public List<CrewVessel> GetCrewVessel(DBTools db, long payperiodId, bool isVesselOnly = false)
        {
            using (var ds = db.ExecuteReader("cnb.GetCrewVessel", new Dictionary<string, object> { { "@PayPeriodID", payperiodId } }))
            {
                VesselOnly = isVesselOnly;
                List<CrewVessel> vessels = ds.GetList(CrewVesselConverter);
                VesselOnly = isVesselOnly;
                return vessels;
            }
        }
        
        public List<CrewPayrollDetails> GetPreviousAdjustedPayroll(DateTime start, DateTime end, long personnelId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPreviousAdjustedPayroll", new Dictionary<string, object> { { "@Start", start }, { "@End", end }, { "@PersonnelID", personnelId } }))
                {
                    return ds.GetList(CrewPayrollDetailConverter);
                }
            }
        }

        public List<CrewPayroll> GetCrewPayrolls(long payperiodId, int vesselId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewPayroll", new Dictionary<string, object> { { "@PayPeriodID", payperiodId }, { "@VesselID", vesselId } }))
                {
                    return ds.GetList(CrewPayrollConverter);
                }
            }

        }
        public List<CrewPayrollPeriod> GetCrewPayrollBases(DateTime? startDate, DateTime? endDate, PayrollSheet payrollSheet, int page, int gridCount, out int PageCount)
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
            var pbase = new List<CrewPayrollPeriod>();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.FilterCrewPayrollBase", ref outparameters, parameters))
                {
                    BaseOnly = true;
                    pbase = ds.GetList(BaseConverter);
                    BaseOnly = false;
                    PageCount = outparameters.Get("@PageCount").ToInt();
                }
            }
            return pbase;
        }
        public CrewPayrollPeriod GetCrewPayrollBase(long? id, bool baseOnly = false)
        {
            var parameters = new Dictionary<string, object> {
                { "@ID", id}
            };

            var pbase = new CrewPayrollPeriod();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewPayrollBase", new Dictionary<string, object> { { "@ID", id} }))
                {
                    BaseOnly = baseOnly;
                    pbase = ds.Get(BaseConverter);
                    BaseOnly = false;
                }
            }
            return pbase;
        }
        public CrewPayrollPeriod GetCrewPayrollBase(DBTools db, long? id)
        {
            using (var ds = db.ExecuteReader("cnb.GetCrewPayrollBase", new Dictionary<string, object> { { "@ID", id } }))
            {
                BaseOnly = true;
                var pbase = ds.Get(BaseConverter);
                BaseOnly = false;
                return pbase;
            }
        }
        public List<CrewLoanDeductions> GetCrewLoanDeductions(long payrollID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewLoanDeductions", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    return ds.GetList(CrewLoanDeductionConverter);
                }
            }
        }
        public List<CrewPayrollDeductions> GetCrewPayrollDeductions(long payrollID)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewPayrollDeductions", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    return ds.GetList(CrewPayrollDeductionConverter);
                }
            }
        }
        public List<CrewPayrollDetails> GetCrewPayrollDetails(long payrollID)
        {
            var paydet = new List<CrewPayrollDetails>();
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetCrewPayrollDetails", new Dictionary<string, object> { { "@PayrollID", payrollID } }))
                {
                    paydet = ds.GetList(CrewPayrollDetailConverter);
                }
            }
            return paydet;
        }
        public List<CrewPayroll> GetCrewPayrollList(DBTools db, long payrollperiodid, long? id = null, long? personnelId = null)
        {
            using (var ds = db.ExecuteReader("cnb.GetCrewPayroll", new Dictionary<string, object> { 
                { "@PayrollPeriodID", payrollperiodid },
                { "@ID", id },
                { "@PersonnelID", personnelId } }))
            {
                return ds.GetList(CrewPayrollConverter);
            }
        }
        public CrewPayroll GetCrewPayroll(long id)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("cnb.GetPayroll", new Dictionary<string, object> { { "@ID", id } }))
                {
                    return ds.Get(CrewPayrollConverter);
                }
            }
        }
        public bool ValidateCrewPayrollGeneration(string payperiod, PayrollSheet type)
        {
            using (var db = new DBTools())
            {
                var res = db.ExecuteScalar("cnb.ValidateCrewPayroll", new Dictionary<string, object> { { "@PayPeriod", payperiod }, { "@Type", type } });
                if (res != null)
                    return res.ToInt() == 0;
            }
            throw new Exception("Unable to generate payroll");
        }
        public CrewPayrollPeriod CreateOrUpdate(CrewPayrollPeriod payrollBase, int userid)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    SaveCrewPayrollBase(payrollBase, userid, db);

                    SaveCrewPayrollList(payrollBase.CrewVessel.SelectMany(x => x.CrewPayrolls).ToList(), payrollBase.ID, payrollBase.Type, userid, db);


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
        private void SaveCrewPayrollBase(CrewPayrollPeriod payrollBase, int userid, DBTools db)
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

            db.ExecuteNonQuery("cnb.CreateOrUpdateCrewPayrollPeriod", ref outparameters, parameters);
            payrollBase.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveCrewPayrollList(List<CrewPayroll> payrolls, long baseId, PayrollSheet type, int userid, DBTools db)
        {
            foreach (var payroll in payrolls)
            {
                if (payroll.ID > 0 && !payroll.Modified)
                {
                    DeleteCrewPayroll(db, payroll.ID, userid);
                    DeleteCrewPayrollDetails(db, userid, payrollId: payroll.ID);
                }
                else
                    SaveIndividualCrewPayroll(db, baseId, type, userid, payroll);
            }
        }
        private void DeleteCrewPayrollDetails(DBTools db, int userid, long? payrollId = null, long? detailsId = null)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", detailsId },
                { "@PayrollID", payrollId },
                { "@LogBy", userid }
            };

            db.ExecuteNonQuery("cnb.DeleteCrewPayrollDetails", parameters);
        }
        private void DeleteCrewPayroll(DBTools db, long id, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", id },
                {  "@LogBy", userid }
            };

            db.ExecuteNonQuery("cnb.DeleteCrewPayroll", parameters);
        }
        private void SaveIndividualCrewPayroll(DBTools db, long baseId, PayrollSheet type, int userid, CrewPayroll payroll)
        {
            SaveCrewPayroll(db, baseId, userid, payroll);

            foreach (var details in payroll.CrewPayrollDetails)
            {
                if (details.ID > 0 && !details.Modified)
                    DeleteCrewPayrollDetails(db, userid, detailsId: details.ID);
                else
                    SaveCrewPayrollDetails(userid, db, payroll.ID, details);
            }

            foreach (var deduction in payroll.CrewPayrollDeductions)
            {
                if (deduction.ID > 0 && !deduction.Modified)
                    DeleteCrewPayrollDeduction(db, userid, deduction.ID);
                else
                    SaveCrewPayrollDeductions(userid, db, payroll.ID, deduction);
            }

            foreach (var loan in payroll.CrewLoanDeductions)
            {
                if (loan.ID > 0 && !loan.Modified)
                {
                    DeleteCrewLoanDeduction(db, loan.ID, userid);
                }
                else
                {
                    SaveCrewLoanDeductions(userid, db, payroll.ID, loan);
                    if (type == PayrollSheet.B) PersonnelLoanProcess.Instance.UpdateAmount(db, loan.PersonnelLoan?.ID ?? 0, loan.Amount, userid);
                }
            }
        }
        private void DeleteCrewLoanDeduction(DBTools db, long loanDeductionId, int userid)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", loanDeductionId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.DeleteCrewLoanDeduction", parameters);
        }
        private void DeleteCrewPayrollDeduction(DBTools db, int userid, long payrollDeductionId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID", payrollDeductionId },
                { "@LogBy", userid}
            };

            db.ExecuteNonQuery("cnb.DeleteCrewPayrollDeductions", parameters);
        }
        private void SaveCrewPayroll(DBTools db, long baseId, int userid, CrewPayroll payroll)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CrewPayrollPeriodID", baseId },
                { "@PersonnelID", payroll.Personnel.ID },
                { "@HolidayPay", payroll.HolidayPay },
                { "@NoofDays", payroll.NoOfDays },
                { "@BasicPay", payroll.BasicPay },
                { "@Tax", payroll.Tax },
                { "@TotalDeductions", payroll.TotalDeductions },
                { "@GrossPay", payroll.GrossPay },
                { "@NetPay", payroll.NetPay },
                { "@VesselID", payroll.VesselID },
                { "@LogBy", userid }
            };

            List<OutParameters> outparameters = new List<OutParameters> { { "@ID", SqlDbType.BigInt, payroll.ID } };

            db.ExecuteNonQuery("cnb.CreateOrUpdateCrewPayroll", ref outparameters, parameters);

            payroll.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveCrewLoanDeductions(int userid, DBTools db, long payrollId, CrewLoanDeductions loan)
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
            db.ExecuteNonQuery("cnb.CreateOrUpdateCrewLoanDeductions", ref outparameters, parameters);
            loan.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveCrewPayrollDeductions(int userid, DBTools db, long payrollId, CrewPayrollDeductions deduction)
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
            db.ExecuteNonQuery("cnb.CreateOrUpdateCrewPayrollDeductions", ref outparameters, parameters);
            deduction.ID = outparameters.Get("@ID").ToLong();
        }
        private void SaveCrewPayrollDetails(int userid, DBTools db, long payrollId, CrewPayrollDetails details)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@CrewPayrollID", payrollId },
                { "@LoggedDate", details.LoggedDate },
                { "@DailyRate", details.TotalRegularMinutes },
                { "@IsHoliday", details.IsHoliday },
                { "@IsSunday", details.IsSunday },
                { "@IsAdjusted", details.IsAdjusted },
                { "@IsNonTaxable", details.IsNonTaxable },
                { "@PositionID", details.PostiionID },
                { "@VesselID", details.VesselID },
                { "@IsAdditionalsOnly", details.IsAdditionalsOnly },
                { "@IsCorrected", details.IsCorrected},
                { "@LogBy", userid }
            };

            List<OutParameters> outparameters = new List<OutParameters>
            {
                { "@ID", SqlDbType.BigInt, details.ID }
            };
            db.ExecuteNonQuery("cnb.CreateOrUpdateCrewPayrollDetails", ref outparameters, parameters);
            details.ID = outparameters.Get("@ID").ToLong();
        }
        public CrewPayrollPeriod GetCrewVesselPayroll(long payPeriodId)
        {
            using (var db = new DBTools())
            {
                CrewPayrollPeriod payrollBase = GetCrewPayrollBase(db, payPeriodId);
                return payrollBase;
            }
        }
        public bool HasCrewPayrollAhead(PayrollSheet type, DateTime date)
        {
            using (var db = new DBTools())
            {
                return (db.ExecuteScalar("cnb.HasCrewPayrollAhead", new Dictionary<string, object> {
                        { "@PayrollType", type },
                        { "@StartDate", date },
                     }).ToNullableInt() ?? 0) > 0;
            }
        }
        public CrewPayrollPeriod GenerateCrewPayroll(int month, int year, int cutoff, PayrollSheet payrollSheet, int userid)
        {
            CrewPayrollPeriod payrollBase = GetCrewPayrollDate(month, year, cutoff, payrollSheet);
            if (HasCrewPayrollAhead(payrollSheet, payrollBase.StartDate))
                throw new Exception("Unable to generate payroll");

            if (payrollBase.EndDate > DateTime.Now)
                throw new Exception("Unable to generate future payroll");


            payrollBase.PayPeriod = payrollBase.StartDate.ToString("MMddyyyy") + payrollBase.AdjustedEndDate.ToString("MMddyyyy");
            payrollBase.Type = payrollSheet;
            payrollBase.PayrollStatus = new Lookup { ID = 1, Description = "Pending" };

            if (ValidateCrewPayrollGeneration(payrollBase.PayPeriod, payrollBase.Type))
            {
                return CreateOrUpdate(CrewPayrollComputation.Instance.GeneratePayroll(payrollBase), userid);
            }
            else
            {
                throw new Exception("Payroll already generated for this cut-off");
            }
        }
        public CrewPayrollPeriod GetCrewPayrollDate(int month, int year, int cutoff, PayrollSheet payrollSheet)
        {
            CrewPayrollPeriod payrollBase = new CrewPayrollPeriod();

            var start = 1;
            var end = 1;
            var assumeStart = 0;
            var assumeEnd = 0;

            if (payrollSheet == PayrollSheet.A)
            {
                start = 1;
                end = CrewPayrollParameters.Instance.CrewCutOff2;
                assumeStart = CrewPayrollParameters.Instance.CrewCutOff2 + 1;
                assumeEnd = new DateTime(month, year, 1).AddMonths(1).AddDays(-1).Day;
            }
            else
            {
                switch (cutoff)
                {
                    case 1:
                        start = 1;
                        end = CrewPayrollParameters.Instance.CrewCutOff1;
                        assumeStart = CrewPayrollParameters.Instance.CrewCutOff1 + 1;
                        assumeEnd = 15;
                        break;
                    case 2:
                        start = 16;
                        end = CrewPayrollParameters.Instance.CrewCutOff2;
                        assumeStart = CrewPayrollParameters.Instance.CrewCutOff2 + 1;
                        assumeEnd = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).Day;
                        break;
                }
            }
            payrollBase.StartDate = new DateTime(year, month, start);
            payrollBase.EndDate = new DateTime(year, month, end);
            payrollBase.AdjustedStartDate = new DateTime(year, month, assumeStart);
            payrollBase.AdjustedEndDate = new DateTime(year, month, assumeEnd);

            return payrollBase;
        }
        public CrewPayroll RecomputeCrewPayroll(long payrollId, int userid)
        {
            var payroll = GetCrewPayroll(payrollId);
            var payrollBase = GetCrewPayrollBase(payroll.CrewPayrollPeriodID, true);

            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    foreach (var loan in payroll.CrewLoanDeductions)
                    {
                        if (payrollBase.Type == PayrollSheet.B)
                            PersonnelLoanProcess.Instance.RevertAmount(db, loan.PersonnelLoan?.ID ?? 0, loan.ID, userid);
                    }
                    db.CommitTransaction();
                }
                catch (Exception ex) { db.RollBackTransaction(); throw ex; }

            }

            CrewPayrollComputation.Instance.Recompute(payroll, payrollBase);

            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    SaveIndividualCrewPayroll(db, payrollBase.ID, payrollBase.Type, userid, payroll);
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
