using ProcessLayer.Entities;
using ProcessLayer.Entities.CnB;
using ProcessLayer.Entities.HR;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Helpers.ObjectParameter.Payroll;
using ProcessLayer.Processes;
using ProcessLayer.Processes.CnB;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Computation.CnB
{
    public sealed class CrewPayrollComputation
    {
        public static readonly CrewPayrollComputation Instance = new CrewPayrollComputation();
        private CrewPayrollComputation()
        {
        }
        private List<NonWorkingDays> NonWorkingDays { get; set; }
        private List<Vessel> Vessel { get; set; }
        private int Cutoff { get; set; }
        public PayrollType Monthly { get; set; }
        public CrewPayrollPeriod GeneratePayroll(CrewPayrollPeriod payrollPeriod)
        {
            Cutoff = (payrollPeriod.EndDate - payrollPeriod.StartDate).TotalDays > 25 ? 0 : (payrollPeriod.EndDate.Day <= 15 ? 1 : 2);
            Vessel = VesselProcess.Instance.GetList();

            if(Cutoff == 2 || Cutoff == 0)
                NonWorkingDays = NonWorkingDaysProcess.Instance.GetNonWorkingDays(payrollPeriod.StartDate, payrollPeriod.AdjustedEndDate)?.Where(x => (x.NonWorkingType == 1 || x.NonWorkingType == 2) && (x.IsGlobal ?? false))?.ToList();
            
            for (int i = 0; i < Vessel.Count; i++)
            {
                List<Personnel> crews = PersonnelProcess.GetVesselCrewForPayroll(Vessel[i].ID, payrollPeriod.StartDate, payrollPeriod.EndDate);
                if (crews?.Any() ?? false)
                {
                    CrewVessel crewVessel = new CrewVessel()
                    {
                        Vessel = Vessel[i]
                    };

                    for (int j = 0; j < crews.Count; j++)
                    {
                        CrewPayroll crewPayroll = new CrewPayroll
                        {
                            PersonnelID = crews[j].ID,
                            Personnel = crews[j],
                            VesselID = Vessel[i].ID,
                            Vessel = Vessel[i]
                        };
                        List<CrewMovement> crewMovememnts = VesselMovementProcess.GetCrewList(Vessel[i].ID, crews[j].ID, payrollPeriod.StartDate, payrollPeriod.EndDate);
                        Compute(crewPayroll, crewMovememnts, payrollPeriod.Type, payrollPeriod.StartDate, payrollPeriod.EndDate, payrollPeriod.AdjustedStartDate, payrollPeriod.AdjustedEndDate);

                        crewVessel.CrewPayrolls.Add(crewPayroll);
                    }
                    payrollPeriod.CrewVessel.Add(crewVessel);
                }
            }
            if (payrollPeriod.CrewVessel?.Any() ?? false)
                throw new Exception("No Data Found");
            return payrollPeriod;
        }
        public CrewPayroll Recompute(CrewPayroll payroll, CrewPayrollPeriod payrollPeriod)
        {
            List<CrewMovement> crewMovememnts = VesselMovementProcess.GetCrewList(payroll.Vessel.ID, payroll.PersonnelID, payrollPeriod.StartDate, payrollPeriod.EndDate);
            Compute(payroll, crewMovememnts, payrollPeriod.Type, payrollPeriod.StartDate, payrollPeriod.EndDate, payrollPeriod.AdjustedStartDate, payrollPeriod.AdjustedEndDate);
            return payroll;
        }
        private void Compute(CrewPayroll payroll, List<CrewMovement> crewMovememnts, PayrollSheet type, DateTime periodStart, DateTime periodEnd, DateTime assumeStart, DateTime assumeEnd)
        {
            List<PersonnelLoan> deductibleLoans = PersonnelLoanProcess.Instance.GetPayrollBGovernmentLoanDeductions(payroll.Personnel.ID, periodStart, periodEnd);
            DateTime prevAssumeEnd = periodStart.AddDays(-1);
            int cutoffAssumeStart = (Cutoff == 2 ? CrewPayrollParameters.Instance.CrewCutOff1 : CrewPayrollParameters.Instance.CrewCutOff2) + 1;
            DateTime prevAssumeStart = new DateTime(prevAssumeEnd.Year, prevAssumeEnd.Month, cutoffAssumeStart);
            List<CrewPayrollDetails> previousAdjustedPayroll = CrewPayrollProcess.Instance.GetPreviousAdjustedPayroll(prevAssumeStart, prevAssumeEnd, payroll.PersonnelID);
            List<CrewMovement> previousAdjustedCrewMovement = CrewMovementProcess.GetCrewActualMovement(payroll.PersonnelID, null, prevAssumeStart, prevAssumeEnd);
            if (previousAdjustedPayroll?.Any() ?? false)
            {
                for (int i = 0; i < previousAdjustedPayroll.Count; i++)
                {
                    var cm = previousAdjustedCrewMovement?.Where(x => x.OnboardDate?.Date <= previousAdjustedPayroll[i].LoggedDate && (x.OffboardDate ?? DateTime.Now).Date >= previousAdjustedPayroll[i].LoggedDate).OrderByDescending(x => x.OnboardDate).FirstOrDefault();
                    if ((cm?.PersonnelID ?? 0) == 0) continue;
                    var dailyrate = cm.DailyRate ?? cm.SNDailyRate ?? 0;
                    if (dailyrate == previousAdjustedPayroll[i].DailyRate) continue;

                    var holiday = NonWorkingDays?.Where(x => ((x.Yearly ?? false) && x.Day?.Month == previousAdjustedPayroll[i].LoggedDate.Month && x.Day?.Day == previousAdjustedPayroll[i].LoggedDate.Day) || x.Day == previousAdjustedPayroll[i].LoggedDate).FirstOrDefault();

                    CrewPayrollDetails details = new CrewPayrollDetails();

                    if (payroll.CrewPayrollDetails?.Where(x => x.LoggedDate == previousAdjustedPayroll[i].LoggedDate)?.Any() ?? false)
                    {
                        details = payroll.CrewPayrollDetails?.Where(x => x.LoggedDate == previousAdjustedPayroll[i].LoggedDate).First();
                        details.IsHoliday = default;
                    }
                    details.Position = cm.PositionID != null ? cm._Position : cm._SNPosition;
                    details.PostiionID = details.Position?.ID ?? 0;
                    details.Vessel = cm.PositionID != null ? cm._Vessel : cm._SNVessel;
                    details.VesselID = details.Vessel?.ID ?? 0;
                    details.IsSunday = Cutoff == 1 ? previousAdjustedPayroll[i].LoggedDate.DayOfWeek == DayOfWeek.Sunday : false;
                    details.LoggedDate = previousAdjustedPayroll[i].LoggedDate;
                    details.IsHoliday = Cutoff == 1 ? (holiday?.ID ?? 0) > 0 : false;
                    details.IsCorrected = true;
                    details.DailyRate = dailyrate;
                    payroll.CrewPayrollDetails.Add(details);
                }
            }
            if (payroll.ID > 0)
            {
                payroll.Tax = 0;
                payroll.NetPay = 0;
                payroll.BasicPay = 0;
                payroll.GrossPay = 0;
            }

            if(Cutoff == 2)
            {
                DateTime d = new DateTime(periodStart.Year, periodStart.Month, 1);
                List<CrewMovement> actualMovement = CrewMovementProcess.GetCrewActualMovement(payroll.PersonnelID, null, d, periodStart.AddDays(-1));
                DateTime? startDate = actualMovement.Select(x => x.OnboardDate).OrderBy(x => x).FirstOrDefault();
                DateTime endDate = periodStart.AddDays(-1);

                if (startDate != null)
                {
                    GenerateDetails(payroll, startDate.Value, assumeStart.Date, endDate, actualMovement, true);
                }
            }

            GenerateDetails(payroll, periodStart.Date, assumeStart.Date, assumeEnd.Date, crewMovememnts, true);

            payroll.BasicPay = payroll.CrewPayrollDetails.Where(x => (x.ID > 0 && x.Modified) || x.ID == 0).Sum(t => t.DailyRate * ((t.IsAdditionalsOnly ? 0 : 1) + (t.IsSunday ? CrewPayrollParameters.Instance.CrewSundayRate : CrewPayrollParameters.Instance.CrewHolidayRate))).ToDecimalPlaces(2);

            payroll.GrossPay = payroll.BasicPay.ToDecimalPlaces(2);
            if (Cutoff == 2)
            {
                ComputeDeductions(payroll, periodStart, Cutoff);
                ComputeLoans(payroll, deductibleLoans);

                payroll.Tax += PayrollProcess.Instance.GetTax(payroll.Personnel?.ID ?? 0, type, payroll.GrossPay, Cutoff, periodStart) ?? 0;
                payroll.TotalDeductions += payroll.Tax.ToDecimalPlaces(2);
            }
            payroll.NetPay += payroll.GrossPay.ToDecimalPlaces(2) - payroll.TotalDeductions.ToDecimalPlaces(2);

        }
        private void GenerateDetails(CrewPayroll payroll, DateTime startDate, DateTime assumeStart, DateTime endDate, List<CrewMovement> actualMovement, bool isAdditionalOnly = false)
        {
            while (startDate <= endDate)
            {
                var cm = actualMovement?.Where(x => x.OnboardDate?.Date <= startDate.Date && (x.OffboardDate ?? DateTime.Now).Date >= startDate.Date).OrderByDescending(x => x.OnboardDate).FirstOrDefault();
                if ((cm?.PersonnelID ?? 0) == 0)
                {
                    startDate = startDate.AddDays(1); 
                    continue; 
                }

                var holiday = NonWorkingDays?.Where(x => ((x.Yearly ?? false) && x.Day?.Month == startDate.Month && x.Day?.Day == startDate.Day) || x.Day == startDate).FirstOrDefault();
                if ((holiday?.ID ?? 0) == 0 && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1); 
                    continue; 
                }


                CrewPayrollDetails details = new CrewPayrollDetails();


                if (payroll.CrewPayrollDetails?.Where(x => x.LoggedDate == startDate)?.Any() ?? false)
                {
                    details = payroll.CrewPayrollDetails.Where(x => x.LoggedDate == startDate).First();
                    details.IsHoliday = default;
                }
                details.Position = cm.PositionID != null ? cm._Position : cm._SNPosition;
                details.PostiionID = details.Position?.ID ?? 0;
                details.Vessel = cm.PositionID != null ? cm._Vessel : cm._SNVessel;
                details.VesselID = details.Vessel?.ID ?? 0;
                details.IsSunday = Cutoff == 2 ? startDate.DayOfWeek == DayOfWeek.Sunday : false;
                details.LoggedDate = startDate;
                details.IsHoliday = Cutoff == 2 ? (holiday?.ID ?? 0) > 0 : false;
                details.IsAdditionalsOnly = isAdditionalOnly;
                details.DailyRate = cm.DailyRate ?? cm.SNDailyRate ?? 0;
                details.IsAdjusted = startDate >= assumeStart;
                payroll.CrewPayrollDetails.Add(details);

                startDate = startDate.AddDays(1);
            }
        }
        private void ComputeLoans(CrewPayroll payroll, List<PersonnelLoan> deductibleLoans)
        {
            if (deductibleLoans?.Any() ?? false)
            {
                deductibleLoans.ForEach(d =>
                {
                    if (d._Loan.GovernmentLoan ?? false)
                    {
                        payroll.TotalDeductions += (d.Amount ?? 0).ToDecimalPlaces(2);
                        payroll.NetPay -= (d.Amount ?? 0).ToDecimalPlaces(2);
                        if (payroll.CrewLoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).Any())
                        {
                            var ded = payroll.CrewLoanDeductions.Where(x => x.PersonnelLoan?.ID == d?.ID && x.ID > 0).First();
                            ded.Modified = true;
                            ded.Amount = (d.Amount ?? 0);
                        }
                        else
                            payroll.CrewLoanDeductions.Add(new CrewLoanDeductions { Amount = (d.Amount ?? 0), PersonnelLoan = d });
                    }
                });
            }
        }
        private void ComputeDeductions(CrewPayroll payroll, DateTime periodStart, int cutoff)
        {
            CrewPayrollDeductions HDMF = (CrewPayrollDeductions)PayrollProcess.Instance.GetHDMF(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            CrewPayrollDeductions PhilHealth = (CrewPayrollDeductions)PayrollProcess.Instance.GetPhilHealth(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            CrewPayrollDeductions SSS = (CrewPayrollDeductions)PayrollProcess.Instance.GetSSS(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);
            CrewPayrollDeductions ProvFund = (CrewPayrollDeductions)PayrollProcess.Instance.GetProvidentFund(payroll.Personnel?.ID, payroll.GrossPay, cutoff, periodStart);

            if ((PhilHealth?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (PhilHealth.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == PhilHealth.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == PhilHealth.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = PhilHealth.Amount ?? 0;
                }
                else
                    payroll.CrewPayrollDeductions.Add(PhilHealth);
            }

            if ((HDMF?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (HDMF.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == HDMF.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == HDMF.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = HDMF.Amount;
                }
                else
                    payroll.CrewPayrollDeductions.Add(HDMF);
            }

            if ((SSS?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (SSS.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == SSS.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == SSS.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = SSS.Amount;
                }
                else
                    payroll.CrewPayrollDeductions.Add(SSS);
            }

            if ((ProvFund?.Amount ?? 0) > 0)
            {
                payroll.TotalDeductions += (ProvFund.Amount ?? 0).ToDecimalPlaces(2);
                if (payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == ProvFund.Deduction?.ID && x.ID > 0).Any())
                {
                    var ded = payroll.CrewPayrollDeductions.Where(x => x.Deduction?.ID == ProvFund.Deduction?.ID && x.ID > 0).First();
                    ded.Modified = true;
                    ded.Amount = ProvFund.Amount;
                }
                else
                    payroll.CrewPayrollDeductions.Add(ProvFund);
            }
        }
    }
}