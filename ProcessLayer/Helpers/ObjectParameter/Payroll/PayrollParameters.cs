using ProcessLayer.Entities;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers.ObjectParameter.Payroll
{
    public sealed class PayrollParameters
    {
        public static readonly Lazy<PayrollParameters> CNBInstance = new Lazy<PayrollParameters>(() => new PayrollParameters(ParametersTag.Payroll));
        private IEnumerable<Parameters> Parameters { get; set; }
        private PayrollParameters(string key)
        {
            Parameters = ParametersProcess.Instance.Value.GetParameters(key);

            TotalMinutesPerDay = GetParameters(nameof(TotalMinutesPerDay)).ToShort();
            TotalMinutesPerDayWithBreak = GetParameters(nameof(TotalMinutesPerDayWithBreak)).ToShort();
            SundayTotalMinutes = GetParameters(nameof(SundayTotalMinutes)).ToShort();
            HolidayTotalMinutes = GetParameters(nameof(HolidayTotalMinutes)).ToShort();
            DefaultBreaktime = GetParameters(nameof(DefaultBreaktime)).ToTimeSpan();
            DefaultBreaktimeHour = GetParameters(nameof(DefaultBreaktimeHour)).ToShort();
            DefaultHalfdayMinutes = GetParameters(nameof(DefaultHalfdayMinutes)).ToShort();
            RegularOTRate = GetParameters(nameof(RegularOTRate)).ToDecimal();
            HighRiskRate = GetParameters(nameof(HighRiskRate)).ToDecimal();
            ExtensionRate = GetParameters(nameof(ExtensionRate)).ToDecimal();
            SundayOTRate = GetParameters(nameof(SundayOTRate)).ToDecimal();
            HolidayOTRate = GetParameters(nameof(HolidayOTRate)).ToDecimal();
            NightDiffRate1 = GetParameters(nameof(NightDiffRate1)).ToDecimal();
            NightDiffRate2 = GetParameters(nameof(NightDiffRate2)).ToDecimal();
            DefaultBreaktime = GetParameters(nameof(DefaultBreaktime)).ToTimeSpan();
            NightDiffStartTime1 = GetParameters(nameof(NightDiffStartTime1)).ToTimeSpan();
            NightDiffEndTime1 = GetParameters(nameof(NightDiffEndTime1)).ToTimeSpan();
            NightDiffStartTime2 = GetParameters(nameof(NightDiffStartTime2)).ToTimeSpan();
            NightDiffEndTime2 = GetParameters(nameof(NightDiffEndTime2)).ToTimeSpan();
            FirstCutoffStart = GetParameters(nameof(FirstCutoffStart)).ToInt();
            FirstCutoffEnd = GetParameters(nameof(FirstCutoffEnd)).ToInt();
            SecondCutoffStart = GetParameters(nameof(SecondCutoffStart)).ToInt();
            SecondCutoffEnd = GetParameters(nameof(SecondCutoffEnd)).ToInt();
            ExtendedMonths = GetParameters(nameof(ExtendedMonths)).ToInt();
        }
        public PayrollParameters() { }
        private object GetParameters(string desc)
        {
            return Parameters?.Where(x => x.Description == desc).Select(x => x.Value).FirstOrDefault();
        }
        public decimal Minutes { get { return 60; } }
        public decimal DailyHours { get { return TotalMinutesPerDay / Minutes; } }
        public short TotalMinutesPerDay { get; set; }
        public short TotalMinutesPerDayWithBreak { get; set; }
        public short SundayTotalMinutes { get; set; }
        public short HolidayTotalMinutes { get; set; }
        public short DefaultBreaktimeHour { get; set; }
        public int DefaultBreaktimeMinutes { get { return DefaultBreaktimeHour * (short)Minutes; } }
        public short DefaultHalfdayMinutes { get; set; }
        public int DefaultHalfdayMinutesWithBreaktime { get { return DefaultHalfdayMinutes + (DefaultBreaktimeHour * (short)Minutes); } }
        public decimal HighRiskRate { get; set; }
        public decimal ExtensionRate { get; set; }
        public decimal RegularOTRate { get; set; }
        public decimal SundayOTRate { get; set; }
        public decimal HolidayOTRate { get; set; }
        public decimal NightDiffRate1 { get; set; }
        public decimal NightDiffRate2 { get; set; }
        public TimeSpan DefaultBreaktime { get; set; }
        public TimeSpan NightDiffStartTime1 { get; set; }
        public TimeSpan NightDiffEndTime1 { get; set; }
        public TimeSpan NightDiffStartTime2 { get; set; }
        public TimeSpan NightDiffEndTime2 { get; set; }
        public int FirstCutoffStart { get; set; }
        public int FirstCutoffEnd { get; set; }
        public int SecondCutoffStart { get; set; }
        public int SecondCutoffEnd { get; set; }
        public int ExtendedMonths { get; set; }
    }
}
