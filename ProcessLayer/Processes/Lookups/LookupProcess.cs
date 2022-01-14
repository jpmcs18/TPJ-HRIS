using DBUtilities;
using ProcessLayer.Entities;
using ProcessLayer.Helpers;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.LookUp;
using ProcessLayer.Helpers.ObjectParameter.Vessel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace ProcessLayer.Processes
{
    public class LookupProcess
    {
        #region old codes
        internal static Lookup Converter(DataRow dr)
        {
            return new Lookup
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString()
            };  
        }

        internal static Vessel VesselConverter(DataRow dr)
        {
            return new Vessel()
            {
                ID = dr[VesselFields.ID].ToShort(),
                Code = dr[VesselFields.Code].ToString(),
                Description = dr[VesselFields.Description].ToString(),
                GrossTon = dr[VesselFields.GrossTon].ToNullableDecimal(),
                NetTon = dr[VesselFields.NetTon].ToNullableDecimal(),
                HP = dr[VesselFields.HP].ToNullableDecimal(),
                Display = dr[VesselFields.Code].ToString() + " - " + dr[VesselFields.Description].ToString()
            };
        }

        internal static LicenseType LicenseTypeConverter(DataRow dr)
        {
            var l = new LicenseType
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                Perpetual = dr["Perpetual"].ToNullableBoolean() ?? false
            };
            if (l.Perpetual)
            {
                l.Description = l.Description + " (Perpetual)";
            }
            return l;
        }

        internal static PayrollType PayrollTypeConverter(DataRow dr)
        {
            return new PayrollType
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                NoofDays = dr["No of Days"].ToNullableInt()
            };
        }

        internal static LateDeduction LateDeductionConverter(DataRow dr)
        {
            return new LateDeduction
            {
                ID = dr["ID"].ToInt(),
                TimeIn = dr["Time In"].ToNullableTimeSpan(),
                DeductedHours = dr["Deducted Hours"].ToNullableInt()
            };
        }

        internal static EmploymentType EmploymentTypeConverter(DataRow dr)
        {
            return new EmploymentType
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                WithGovtDeduction = dr["With Govt Deduction"].ToNullableBoolean() ?? false
            };
        }
        public static List<LicenseType> GetLicenseType(bool HasDefault = false)
        {
            var licenseTypes = new List<LicenseType>();

            var Parameters = new Dictionary<string, object> {
                { LookupParameters.Table, Table.LicenseType }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    licenseTypes = ds.GetList(LicenseTypeConverter);
                }
            }

            if (HasDefault)
            {
                licenseTypes.Insert(0, new LicenseType { ID = 0, Description = "N/A" });
            }

            return licenseTypes;
        }

        public static LicenseType GetLicenseType(int? id)
        {
            var licenseType = new LicenseType();
            if (id.HasValue && id > 0)
            {
                var Parameters = new Dictionary<string, object> {
                    { LookupParameters.Table, Table.LicenseType },
                    { LookupParameters.Id, id.Value }
                };

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                    {
                        licenseType = ds.Get(LicenseTypeConverter);
                    }
                }
            }
            return licenseType;
        }
        
        private static Lookup GetLookupById(string tableName, int? id, string schema = null)
        {
            var lookups = new Lookup();

            if (id.HasValue && id > 0)
            {
                var Parameters = new Dictionary<string, object> {
                    { LookupParameters.Table, tableName },
                    { LookupParameters.Id, id.Value }
                };

                if (schema != null)
                {
                    Parameters.Add(LookupParameters.Schema, schema);
                }

                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                    {
                        lookups = ds.Get(Converter);
                    }
                }
            }

            return lookups;
        }

        private static IEnumerable<Lookup> GetLookup(string tableName, bool HasDefault = false, string schema = null)
        {
            var lookups = new List<Lookup>();

            var Parameters = new Dictionary<string, object> {
                { LookupParameters.Table, tableName }
            };

            if (schema != null)
            {
                Parameters.Add(LookupParameters.Schema, schema);
            }

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    lookups = ds.GetList(Converter);
                }
            }

            if (HasDefault)
            {
                lookups.Insert(0, new Lookup() { ID = 0, Description = "N/A" });
            }

            return lookups;
        }

        public static Dictionary<bool, string> GetGender()
        {
            return new Dictionary<bool, string> { { true, "Male" }, { false, "Female" } };
        }

        public static string GetGender(bool? b)
        {
            return b.HasValue ? (b.Value ? "Male" : "Female") : null;
        }

        public static IEnumerable<Lookup> GetCivilStatus(bool HasDefault = false)
        {
            return GetLookup(Table.CivilStatus, HasDefault);
        }

        public static Lookup GetCivilStatus(int? id)
        {
            return GetLookupById(Table.CivilStatus, id);
        }

        public static IEnumerable<Lookup> GetReligion(bool HasDefault = false)
        {
            return GetLookup(Table.Religion, HasDefault);
        }

        public static Lookup GetReligion(int? id)
        {
            return GetLookupById(Table.Religion, id);
        }


        public static IEnumerable<Lookup> GetCurrency(bool HasDefault = false)
        {
            return GetLookup(Table.Currency, HasDefault);
        }

        public static Lookup GetCurrency(int? id)
        {
            return GetLookupById(Table.Currency, id);
        }

        public static IEnumerable<Lookup> GetContactNoType(bool HasDefault = false)
        {
            return GetLookup(Table.ContactNoType, HasDefault);
        }

        public static Lookup GetContactNoType(int? id)
        {
            return GetLookupById(Table.ContactNoType, id);
        }

        public static IEnumerable<Lookup> GetEmployementStatus(bool HasDefault = false)
        {
            return GetLookup(Table.EmploymentStatus, HasDefault);
        }

        public static Lookup GetEmployementStatus(int? id)
        {
            return GetLookupById(Table.EmploymentStatus, id);
        }

        public static IEnumerable<Lookup> GetEducationalLevel(bool HasDefault = false)
        {
            return GetLookup(Table.EducationalLevel, HasDefault);
        }

        public static Lookup GetEducationalLevel(int? id)
        {
            return GetLookupById(Table.EducationalLevel, id);
        }

        public static IEnumerable<Lookup> GetRelationship(bool HasDefault = false)
        {
            return GetLookup(Table.Relationship, HasDefault);
        }

        public static Lookup GetRelationship(int? id)
        {
            return GetLookupById(Table.Relationship, id);
        }

        public static IEnumerable<Lookup> GetLegislationStatus(bool HasDefault = false)
        {
            return GetLookup(Table.LegislationStatus, HasDefault);
        }

        public static Lookup GetLegislationStatus(int? id)
        {
            return GetLookupById(Table.LegislationStatus, id);
        }

        public static IEnumerable<Lookup> GetVaccineType(bool HasDefault = false)
        {
            return GetLookup(Table.VaccineType, HasDefault);
        }

        public static Lookup GetVaccineType(int? id)
        {
            return GetLookupById(Table.VaccineType, id);
        }

        public static IEnumerable<Lookup> GetTrainingType(bool HasDefault = false)
        {
            return GetLookup(Table.TrainingType, HasDefault);
        }

        public static Lookup GetTrainingType(int? id)
        {
            return GetLookupById(Table.TrainingType, id);
        }

        public static IEnumerable<Lookup> GetTaxType(bool HasDefault = false)
        {
            return GetLookup(Table.TaxType, HasDefault);
        }

        public static Lookup GetTaxType(int? id)
        {
            return GetLookupById(Table.TaxType, id);
        }

        public static IEnumerable<Lookup> GetNationality(bool HasDefault = false)
        {
            return GetLookup(Table.Nationality, HasDefault);
        }

        public static Lookup GetNationality(int? id)
        {
            return GetLookupById(Table.Nationality, id);
        }

        public static IEnumerable<Lookup> GetMemoType(bool HasDefault = false)
        {
            return GetLookup(Table.MemoType, HasDefault);
        }

        public static Lookup GetMemoType(int? id)
        {
            return GetLookupById(Table.MemoType, id);
        }

        public static IEnumerable<Lookup> GetVesselMovementType(bool HasDefault = false)
        {
            return GetLookup(Table.VesselMovementType, HasDefault);
        }

        public static Lookup GetVesselMovementType(int? id)
        {
            return GetLookupById(Table.VesselMovementType, id);
        }

        public static IEnumerable<PayrollType> GetPayrollType(bool HasDefault = false)
        {
           var p =  GetLookups<PayrollType>(PayrollTypeConverter, Table.PayrollType).ToList();
            if(HasDefault)

            p.Insert(0, new PayrollType { ID = 0, Description = "N/A" });

            return p;
        }

        public static PayrollType GetPayrollType(int? id)
        {
            return GetLookup(PayrollTypeConverter, Table.PayrollType, id);
        }

        #endregion
        #region new codes
        
        protected static Compensation CompensationConverter(DataRow dr)
        {
            return new Compensation
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                Taxable = dr["Taxable"].ToNullableBoolean(),
                SupplementarySalary = dr["Supplemental Salary"].ToNullableBoolean(),
                Has_Approval = dr["Has Approval"].ToNullableBoolean(),
                ComputationType = dr["Computation Type"].ToComputationType()

            };
        }
        protected static Deduction DeductionConverter(DataRow dr)
        {
            return new Deduction
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                WhenToDeduct = dr["When to Deduct"].ToNullableInt(),
                ComputedThruSalary = dr["Computed thru Salary"].ToNullableBoolean(),
                GovernmentDeduction = dr["Government Deduction"].ToNullableBoolean()
            };
        }
        protected static Loan LoanConverter(DataRow dr)
        {
            return new Loan
            {
                ID = dr["ID"].ToInt(),
                Description = dr["Description"].ToString(),
                isPersonal = dr["is Personal"].ToNullableBoolean(),
                GovernmentLoan = dr["Government Loan"].ToNullableBoolean()
            };
        }
        private static T GetLookup<T>(Func<DataRow, T> converter, string tableName, object id, string schema = null, string proc = LookupProcedures.GetLookup, Dictionary<string, object> parameters = null, bool withparams = true) where T : new()
        {
            var lookups = new T();

            if (id != null)
            {
                if (withparams && parameters == null)
                {
                    parameters = new Dictionary<string, object> {
                        { LookupParameters.Table, tableName },
                        { LookupParameters.Id, id }
                    };

                    if (schema != null)
                    {
                        parameters.Add(LookupParameters.Schema, schema);
                    }
                }
                using (var db = new DBTools())
                {
                    using (var ds = db.ExecuteReader(proc, parameters))
                    {
                        lookups = ds.Get(converter);
                    }
                }
            }

            return lookups;
        }

        private static IEnumerable<T> GetLookups<T>( Func<DataRow, T> converter, string tableName = null, string schema = null, string proc = LookupProcedures.GetLookup, Dictionary<string, object> parameters = null, bool withparams = true) where T : new()
        {
            var lookups = new List<T>();
            if (withparams && parameters == null)
            {
                parameters = new Dictionary<string, object> {
                    { LookupParameters.Table, tableName }
                };
                if (schema != null)
                {
                    parameters.Add(LookupParameters.Schema, schema);
                }
            }

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(proc, parameters))
                {
                    lookups = ds.GetList(converter);
                }
            }

            return lookups;
        }
        
        public static IEnumerable<Compensation> GetCompensations(bool HasDefault = false)
        {
            var c = GetLookups(CompensationConverter, Table.Compensation).ToList();
            if(HasDefault)
            {
                c.Insert(0, new Compensation { ID = 0, Description = "N/A" });
            }

            return c;
        }

        public static Compensation GetCompensation(int? id)
        {
            return GetLookup(CompensationConverter, Table.Compensation, id);
        }
        #endregion
        
        public static IEnumerable<Lookup> GetPayrollStatus(bool HasDefault = false)
        {
            return GetLookup(Table.PayrollStatus, HasDefault);
        }

        public static Lookup GetPayrollStatus(int? id)
        {
            return GetLookupById(Table.PayrollStatus, id);
        }

        public static Loan GetLoan(int? id)
        {
            return GetLookup(LoanConverter, Table.Loan, id);
        }

        public static IEnumerable<Loan> GetLoans(bool HasDefault = false)
        {
            var c = GetLookups(LoanConverter, Table.Loan, schema: "lookup").ToList();
            if (HasDefault)
            {
                c.Insert(0, new Loan { ID = 0, Description = "N/A" });
            }

            return c;
        }

        public static IEnumerable<EmploymentType> GetEmploymentType(bool HasDefault = false)
        {
            return GetLookups(EmploymentTypeConverter, Table.EmploymentType);
        }

        public static EmploymentType GetEmploymentType(int? id)
        {
            return GetLookup(EmploymentTypeConverter, Table.EmploymentType, id);
        }

        public static IEnumerable<Deduction> GetDeduction(bool HasDefault = false, bool withGovernmentDeduction = true)
        {
            var d = GetLookups(DeductionConverter, Table.Deduction).ToList();
            if(!withGovernmentDeduction)
            {
                d = d.Where(x => !x.GovernmentDeduction ?? false).ToList();
            }
            if (HasDefault)
            {
                d.Insert(0, new Deduction { ID = 0, Description = "N/A" });
            }
            return d;
        }

        public static Deduction GetDeduction(int? id)
        {
            return GetLookup(DeductionConverter, Table.Deduction, id);
        }

        public static Lookup GetMemoStatus(int? id)
        {
            return GetLookupById(Table.MemoStatus, id);
        }
        public static List<YearsOfService> GetYearsOfServices()
        {
            using(var db = new DBTools())
            {
                using(var ds = db.ExecuteReader("lookup.GetYearsOfService"))
                {
                    return ds.GetList((dr) => {
                        return new YearsOfService
                        {
                            StartYear = dr["Start Year"].ToInt(),
                            EndYear = dr["End Year"].ToInt(),
                            Amount = dr["Amount"].ToDecimal()
                        };
                    });
                }
            }
        }
        #region Maintenance

        public static List<string> _GetLookupTables()
        {
            List<string> LookupTables = new List<string>();

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookupTables, new Dictionary<string, object>()))
                {
                    LookupTables = ds.Tables[0].AsEnumerable().Select(r => r.Field<string>(0)).ToList();
                }
            }

            return LookupTables;
        }

        public static List<Lookup> _GetLookup(string TableName)
        {
            if (String.IsNullOrEmpty(TableName))
            {
                throw new Exception("Target table is invalid");
            }

            List<Lookup> Lookup = new List<Lookup>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, TableName }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookup, Parameters))
                {
                    Lookup = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                }
            }

            return Lookup;
        }

        public static List<Lookup> _GetLookupPage(string TableName, string Filter, int Page, int GridCount, out int Count)
        {
            List<Lookup> Lookup = new List<Lookup>();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, TableName },
                { LookupParameters.Filter, Filter },
                { LookupParameters.PageNumber, Page },
                { LookupParameters.GridCount, GridCount }
            };

            using (DBTools db = new DBTools())
            {
                using (var ds = db.ExecuteReader(LookupProcedures.GetLookupPage, Parameters))
                {
                    Lookup = ds.Tables[0].AsEnumerable().Select(r => Converter(r)).ToList();
                    Count = ds.Tables[1].AsEnumerable().Select(r => r.Field<int>("Count")).FirstOrDefault();
                }
            }

            return Lookup;
        }

        public static Lookup _GetLookupByID(string TableName, int ID)
        {
            if (String.IsNullOrEmpty(TableName))
            {
                throw new Exception("Target table is invalid");
            }

            Lookup Lookup = new Lookup();
            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Id, ID },
                { LookupParameters.Table, TableName }
            };

            using (DBTools db = new DBTools())
            {
                var dt = db.ExecuteReader(LookupProcedures.GetLookup, Parameters).Tables[0];

                if (dt.Rows.Count > 0)
                    Lookup = Converter(dt.Rows[0]);
                else
                    throw new Exception("Selected Item not found");
            }

            return Lookup;
        }

        public static Lookup _CreateOrUpdateLookup(string TableName, Lookup Lookup, int UserID)
        {
            if (String.IsNullOrEmpty(TableName))
            {
                throw new Exception("Target table is invalid");
            }

            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, TableName },
                { LookupParameters.Id, Lookup.ID },
                { LookupParameters.Description, Lookup.Description },
                { LookupParameters.UserId, UserID }
            };

            using (DBTools db = new DBTools())
            {
                object ret = db.ExecuteScalar(LookupProcedures.CreateOrUpdateLookup, Parameters);
                if (ret is null)
                    throw new Exception();
                else
                    Lookup.ID = ret.ToInt();
            }

            return Lookup;
        }

        public static void _DeleteLookup(string TableName, int ID, int UserID)
        {
            if (String.IsNullOrEmpty(TableName))
            {
                throw new Exception("Target table is invalid");
            }

            Dictionary<string, object> Parameters = new Dictionary<string, object>()
            {
                { LookupParameters.Table, TableName },
                { LookupParameters.Id, ID },
                { LookupParameters.UserId, UserID }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(LookupProcedures.DeleteLookup, Parameters);
            }
        }

        #endregion
        public static string GetUser(int? id)
        {
            string s = "";
            using (var db = new DBTools())
            {
                s = db.ExecuteScalar("System.GetUserName", new Dictionary<string, object> { { "@UserId", id } })?.ToString();
            }
            return s;
        }

        public static Dictionary<int, string> GetWhenToDeduct()
        {
            return new Dictionary<int, string>
            {
                { 1, "1st Cutoff" },
                { 2, "2nd Cutoff" },
                { 3, "Both" }
            };
        }
    }
}
