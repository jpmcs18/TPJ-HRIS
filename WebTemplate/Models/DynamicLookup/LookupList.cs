using Newtonsoft.Json;
using ProcessLayer.Entities;
using ProcessLayer.Processes;
using ProcessLayer.Processes.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTemplate.Models.DynamicLookup
{
    public enum Lookups
    {
        Position,
        Location,
        Deduction,
        Payroll_Parameters,
        Non_Working_Days,
        Schedule,
        Tax_Table,
        Late_Deduction,
        Leave_Type,
        PhilHealth,
        SSS,
        HDMF,
        Department_Position,
        Non_Taxable_Days,
        Vessel,
        Civil_Status,
        Code_of_Discipline,
        Contact_No_Type,
        Educational_Level,
        Employment_Status,
        Employment_Type,
        Memo_Status,
        Memo_Type,
        Nationality,
        Relationship,
        Religion
    }
    public enum LookupDataType
    {
        rate,
        amount,
        number,
        text,
        boolean,
        model,
        date,
        time,
        year
    }
    public class LookupSetting
    {
        public string Name { get; }
        public bool ReadOnly { get; }
        public string ReadOnlyCheck { get { return ReadOnly ? "disabled" : ""; } }
        public string ReadOnlyText { get { return ReadOnly ? "readonly" : ""; } }
        public LookupDataType Type { get; }
        public string Display { get; }
        public LookupSetting ClassSetting { get; }
        public dynamic LookupSource { get; }
        public string LookupSourceData { get; }
        public bool HasDefault { get; }
        public LookupSetting(string name = "Description",
                             string display = null,
                             LookupDataType type = LookupDataType.text,
                             LookupSetting classSetting = null,
                             dynamic lookupSource = null,
                             string lookupSourceData = null,
                             bool readOnly = false, 
                             bool hasDefault = false)
        {
            Name = name;
            Type = type;
            ClassSetting = classSetting;
            Display = display ?? name;
            LookupSource = lookupSource;
            ReadOnly = readOnly;
            LookupSourceData = lookupSourceData;
            HasDefault = hasDefault;
        }

    }
    public class LookupActions
    {
        public bool Add { get; }
        public bool Edit { get; }
        public bool Delete { get; }
        public bool Pagination { get; }
        public bool Search { get; }

        public LookupActions(bool add = true, bool edit = true, bool delete = true, bool pagination = true, bool search = true)
        {
            Add = add;
            Edit = edit;
            Delete = delete;
            Pagination = pagination;
            Search = search;
        }
    }
    public static class LookupHelperExtension
    {
        public static void Add(this List<KeyValuePair<Lookups, string>> lookupLists, Lookups lookup, string value)
        {
            lookupLists.Add(new KeyValuePair<Lookups, string>(lookup, value));
        }
        public static void Add(this List<KeyValuePair<Lookups, List<LookupSetting>>> lookupListItems, Lookups lookup, params LookupSetting[] lookupSettings)
        {
            lookupListItems.Add(new KeyValuePair<Lookups, List<LookupSetting>>(lookup, lookupSettings.ToList()));
        }
        public static void Add(this List<KeyValuePair<Lookups, LookupActions>> lookupActions, Lookups lookup, LookupActions lookupAction)
        {
            lookupActions.Add(new KeyValuePair<Lookups, LookupActions>(lookup, lookupAction));
        }
    }
    public static class LookupHelper
    {
        public static readonly List<KeyValuePair<Lookups, string>> LookupLists = Enum.GetValues(typeof(Lookups)).Cast<Lookups>().Select(x => new KeyValuePair<Lookups, string>(x, x.ToString().Replace("_", " "))).ToList();

        public static readonly List<LookupSetting> DefaultLookupListItems = new List<LookupSetting> { new LookupSetting() };

        public static readonly List<KeyValuePair<Lookups, List<LookupSetting>>> LookupListItems = new List<KeyValuePair<Lookups, List<LookupSetting>>>()
        {
            { Lookups.Position
                , new LookupSetting()
                , new LookupSetting("Abbreviation")
                , new LookupSetting("PersonnelType", "Personnel Type", LookupDataType.model, new LookupSetting(),  PersonnelTypeProcess.Instance, "PersonnelTypeID")
                , new LookupSetting("AllowApprove", "Allow Approve", LookupDataType.boolean) },
            { Lookups.Location
                , new LookupSetting("Prefix")
                , new LookupSetting()
                , new LookupSetting("HazardRate", "Hazard Rate", LookupDataType.number)
                , new LookupSetting("RequiredTimeLog", "Required Time Log", LookupDataType.boolean)
                , new LookupSetting("OfficeLocation", "Office Location", LookupDataType.boolean)
                , new LookupSetting("WarehouseLocation", "Warehouse Location", LookupDataType.boolean) },
            { Lookups.Late_Deduction
                , new LookupSetting("TimeIn", "Time In", LookupDataType.time) 
                , new LookupSetting("DeductedHours", "Deducted Hours", LookupDataType.number) },
            { Lookups.Deduction
                , new LookupSetting(readOnly: true)
                , new LookupSetting("GovernmentDeduction", "Government Deduction", LookupDataType.boolean, readOnly: true)
                , new LookupSetting("Deduct", "Cutoff", LookupDataType.model, new LookupSetting(),  WhenToDeductProcess.Instance, "WhenToDeduct") },
            { Lookups.Leave_Type
                , new LookupSetting()
                , new LookupSetting("BulkUse", "Bulk Use", LookupDataType.boolean)
                , new LookupSetting("HasDocumentNeeded", "Has Document Needed", LookupDataType.boolean)},
            { Lookups.Payroll_Parameters
                , new LookupSetting("DisplayName", "Name", readOnly: true)
                , new LookupSetting("Value") },
            { Lookups.Non_Working_Days
                , new LookupSetting()
                , new LookupSetting("Day", "Date", type: LookupDataType.date)
                , new LookupSetting("StartTime", "Start Time", LookupDataType.time)
                , new LookupSetting("EndTime", "End Time", LookupDataType.time)
                , new LookupSetting("Type", null, LookupDataType.model, new LookupSetting(), NonWorkingTypeProcess.Instance, "NonWorkingType")
                , new LookupSetting("Yearly", type: LookupDataType.boolean)
                , new LookupSetting("IsGlobal", "Global", LookupDataType.boolean)
                , new LookupSetting("Location", null, LookupDataType.model, new LookupSetting(), LocationProcess.Instance, "LocationID", hasDefault: true) },
             { Lookups.Schedule
                , new LookupSetting()
                , new LookupSetting("TimeIn", "Time In", LookupDataType.time)
                , new LookupSetting("TimeOut", "Time Out", LookupDataType.time)
                , new LookupSetting("BreakTime", "Breaktime", LookupDataType.time)
                , new LookupSetting("BreakTimeHour", "Breaktime Hours", LookupDataType.number)
                , new LookupSetting("TotalWorkingHours", "Total Working Hours", LookupDataType.number)
                , new LookupSetting("AtHome", "At Home", LookupDataType.boolean)
                , new LookupSetting("MustBePresentOnly", "Present Only", LookupDataType.boolean) },
             { Lookups.Tax_Table
                , new LookupSetting("TaxSchedule", "Tax Schedule", LookupDataType.model, new LookupSetting(), TaxScheduleProcess.Instance, "TaxScheduleID")
                , new LookupSetting("MinimumIncome", "Minimum Income", LookupDataType.amount)
                , new LookupSetting("MaximumIncome", "Maximum Income", LookupDataType.amount)
                , new LookupSetting("FixedTax", "Fixed Tax", LookupDataType.amount)
                , new LookupSetting("ExcessOver", "Excess Over", LookupDataType.amount)
                , new LookupSetting("AdditionalTax", "Tax Rate", LookupDataType.rate)
                , new LookupSetting("EffectiveStartDate", "Effective Start Date", LookupDataType.date)
                , new LookupSetting("EffectiveEndDate", "Effective End Date", LookupDataType.date) },
             { Lookups.PhilHealth
                , new LookupSetting("MinSalary", "Minimum Salary", LookupDataType.amount)
                , new LookupSetting("MaxSalary", "Maximum Salary", LookupDataType.amount)
                , new LookupSetting("Share", "Share", LookupDataType.amount)
                , new LookupSetting("Rate", "Rate", LookupDataType.rate)
                , new LookupSetting("DateStart", "Date Start", LookupDataType.date)
                , new LookupSetting("DateEnd", "Date End", LookupDataType.date) },
             { Lookups.HDMF
                , new LookupSetting("MinSalary", "Minimum Salary", LookupDataType.amount)
                , new LookupSetting("MaxSalary", "Maximum Salary", LookupDataType.amount)
                , new LookupSetting("EmployeeShare", "Employee Share", LookupDataType.amount)
                , new LookupSetting("EmployeePercentage", "Employee Percentage", LookupDataType.rate)
                , new LookupSetting("EmployerShare", "Employer Share", LookupDataType.amount)
                , new LookupSetting("EmployerPercentage", "Employer Percentage", LookupDataType.rate)
                , new LookupSetting("DateStart", "Date Start", LookupDataType.date)
                , new LookupSetting("DateEnd", "Date End", LookupDataType.date) },
             { Lookups.SSS
                , new LookupSetting("MinSalary", "Minimum Salary", LookupDataType.amount)
                , new LookupSetting("MaxSalary", "Maximum Salary", LookupDataType.amount)
                , new LookupSetting("EmployeeShare", "Employee Share", LookupDataType.amount)
                , new LookupSetting("EmployerShare", "Employer Share", LookupDataType.amount)
                , new LookupSetting("EC", "EC", LookupDataType.amount)
                , new LookupSetting("ProvPS", "Provident Fund Employee Share", LookupDataType.amount)
                , new LookupSetting("ProvES", "Provident Fund Employer Share", LookupDataType.amount)
                , new LookupSetting("DateStart", "Date Start", LookupDataType.date)
                , new LookupSetting("DateEnd", "Date End", LookupDataType.date) },
            { Lookups.Department_Position
                , new LookupSetting("Department", "Department", LookupDataType.model, new LookupSetting(), DepartmentProcess.Instance, "DepartmentID")
                , new LookupSetting("Position", "Position", LookupDataType.model, new LookupSetting(), PositionProcess.Instance, "PositionID") },
            { Lookups.Vessel
                , new LookupSetting("Code")
                , new LookupSetting()
                , new LookupSetting("GrossTon", "Gross Ton", LookupDataType.number)
                , new LookupSetting("NetTon", "Net Ton", LookupDataType.number)
                , new LookupSetting("HP", "HP", LookupDataType.number) },
            { Lookups.Non_Taxable_Days
                , new LookupSetting()
                , new LookupSetting("StartDate", "Start Date", LookupDataType.date)
                , new LookupSetting("EndDate", "End Date", LookupDataType.date)
                , new LookupSetting("IsGlobal", "Global", LookupDataType.boolean)
                , new LookupSetting("Location", null, LookupDataType.model, new LookupSetting(), LocationProcess.Instance, "LocationID", hasDefault: true) }
        };

        //Include if has any custom action
        public static readonly List<KeyValuePair<Lookups, LookupActions>> LookupActions = new List<KeyValuePair<Lookups, LookupActions>>()
        {
            { Lookups.Payroll_Parameters, new LookupActions(add: false, delete: false, pagination: false) },
            { Lookups.Late_Deduction, new LookupActions(search: false, add:false, delete: false, pagination: false) },
            { Lookups.PhilHealth, new LookupActions(search: false) },
            { Lookups.HDMF, new LookupActions(search: false) },
            { Lookups.SSS, new LookupActions(search: false) },
            { Lookups.Deduction, new LookupActions(add: false, delete: false) },
        };

        public static readonly List<Lookups> DynamicLookups = new List<Lookups>
        {
            Lookups.Civil_Status,
            Lookups.Code_of_Discipline,
            Lookups.Contact_No_Type,
            Lookups.Educational_Level,
            Lookups.Employment_Status,
            Lookups.Employment_Type,
            Lookups.Memo_Status,
            Lookups.Memo_Type,
            Lookups.Nationality,
            Lookups.Relationship,
            Lookups.Religion,
            Lookups.Relationship,
            Lookups.Relationship,
        };

        public static dynamic GetData(dynamic data, LookupSetting setting)
        {
            try
            {
                var propertyInfo = data.GetType().GetProperty(setting.Name);
                var value = propertyInfo.GetValue(data, null);
                return setting.ClassSetting == null || value == null ? value : GetData(value, setting.ClassSetting);
            }
            catch
            {
                return null;
            }
        }
    }
}