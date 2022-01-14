using ProcessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProcessLayer.Helpers;
using DBUtilities;
using ProcessLayer.Helpers.ObjectParameter;
using ProcessLayer.Helpers.ObjectParameter.Personnel;
using System.Configuration;
using System.IO;
using ProcessLayer.Helpers.ObjectParameter.MemoArchive;
using ProcessLayer.Processes.CnB;
using ProcessLayer.Helpers.Enumerable;
using ProcessLayer.Processes.HRs;
using ProcessLayer.Processes.HR;
using ProcessLayer.Processes.Lookups;

namespace ProcessLayer.Processes
{
    public class PersonnelProcess
    {
        internal static bool PersonnelOnly { get; set; } = false;

        internal static Personnel Converter(DataRow dr)
        {
            var p = new Personnel
            {
                ID = dr[PersonnelFields.ID].ToLong(),
                EmployeeNo = dr[PersonnelFields.EmployeeNo].ToString(),
                FirstName = dr[PersonnelFields.FirstName].ToString(),
                MiddleName = dr[PersonnelFields.MiddleName].ToString(),
                MaidenMiddleName = dr[PersonnelFields.MaidenMiddleName].ToString(),
                LastName = dr[PersonnelFields.LastName].ToString(),
                Gender = dr[PersonnelFields.Gender].ToNullableBoolean(),
                DateHired = dr[PersonnelFields.DateHired].ToNullableDateTime(),
                NationalityID = dr[PersonnelFields.NationalityID].ToNullableInt(),
                ResignationDate = dr[PersonnelFields.ResignationDate].ToNullableDateTime(),
                ResignationRemarks = dr[PersonnelFields.ResignationRemarks].ToString(),
                BirthDate = dr[PersonnelFields.BirthDate].ToNullableDateTime(),
                BirthPlace = dr[PersonnelFields.BirthPlace].ToString(),
                Address = dr[PersonnelFields.Address].ToString(),
                MobileNo = dr[PersonnelFields.MobileNo].ToString(),
                TelephoneNo = dr[PersonnelFields.TelephoneNo].ToString(),
                CivilStatusID = dr[PersonnelFields.CivilStatusID].ToNullableInt(),
                ReligionID = dr[PersonnelFields.ReligionID].ToNullableInt(),
                PersonnelTypeID = dr[PersonnelFields.PersonnelTypeID].ToNullableInt(),
                HiringLocationID = dr[PersonnelFields.HiringLocationID].ToNullableInt(),
                SSS = dr[PersonnelFields.SSS].ToString(),
                Philhealth = dr[PersonnelFields.Philhealth].ToString(),
                PAGIBIG = dr[PersonnelFields.PAGIBIG].ToString(),
                TIN = dr[PersonnelFields.TIN].ToString(),
                Email = dr[PersonnelFields.Email].ToString(),
                EmergencyContactAddress = dr[PersonnelFields.EmergencyContactAddress].ToString(),
                EmergencyContactNumber = dr[PersonnelFields.EmergencyContactNumber].ToString(),
                EmergencyContactPerson = dr[PersonnelFields.EmergencyContactPerson].ToString(),
                EmergencyRelationshipID = dr[PersonnelFields.EmergencyRelationshipID].ToNullableInt(),
                EmploymentStatusId = dr[PersonnelFields.EmploymentStatusId].ToNullableInt(),
                ReferenceContactNo = dr[PersonnelFields.ReferenceContactNo].ToString(),
                ReferredBy = dr[PersonnelFields.ReferredBy].ToString(),
                ImagePath = dr[PersonnelFields.ImagePath] != DBNull.Value ? Path.Combine(ConfigurationManager.AppSettings["ImageFolder"], dr[PersonnelFields.ImagePath].ToString()) : "",
                Image = dr[PersonnelFields.ImagePath].ToString(),
                TaxTypeID = dr[PersonnelFields.TaxTypeID].ToNullableInt(),
                WalkIn = dr[PersonnelFields.WalkIn].ToNullableBoolean(),
                PayrollTypeID = dr[PersonnelFields.PayrollTypeID].ToNullableInt(),
                FixedSalary = dr[PersonnelFields.FixedSalary].ToNullableBoolean(),
                AutoOT = dr["Auto OT"].ToNullableBoolean() ?? false,
                AdditionalHazardRate = dr["Additional Hazard Rate"].ToNullableDecimal() ?? 0,
                BiometricsID = dr["Biometrics ID"].ToNullableInt() ?? 0,
                CreatedBy = dr[LogDetailsFields.CreatedBy].ToNullableInt(),
                CreatedOn = dr[LogDetailsFields.CreatedOn].ToNullableDateTime(),
                ModifiedBy = dr[LogDetailsFields.ModifiedBy].ToNullableInt(),
                ModifiedOn = dr[LogDetailsFields.ModifiedOn].ToNullableDateTime(),
                Approved = dr["Approved"].ToNullableBoolean(),
                Cancelled = dr["Cancelled"].ToNullableBoolean(),

            };
            p._Creator = LookupProcess.GetUser(p.CreatedBy);
            p._Modifier = LookupProcess.GetUser(p.ModifiedBy);

            try { 
                p.LinkedPersonnelID = dr[PersonnelFields.PersonnelLinkedID].ToNullableLong();
                p._LinkedPersonnel = Get(p.LinkedPersonnelID ?? 0, true);
            }
            catch { }

            p._PayrollType = LookupProcess.GetPayrollType(p.PayrollTypeID);
            p._Gender = LookupProcess.GetGender(p.Gender);
            p._CivilStatus = LookupProcess.GetCivilStatus(p.CivilStatusID);
            p._PersonnelType = PersonnelTypeProcess.Instance.Value.Get(p.PersonnelTypeID);
            p._Religion = LookupProcess.GetReligion(p.ReligionID);
            p._EmploymentStatus = LookupProcess.GetEmployementStatus(p.EmploymentStatusId);
            p._HiringLocation = LocationProcess.Instance.Value.Get(p.HiringLocationID);
            p._Schedules = PersonnelScheduleProcess.Instance.Value.GetList(p.ID);
            p._ContactNumbers = ContactNumberProcess.GetList(p.ID);
            p._AssignedLocation = PersonnelAssignedLocationProcess.GetList(p.ID);
            p._Relationship = LookupProcess.GetRelationship(p.EmergencyRelationshipID);

            if (!PersonnelOnly)
            {
                p._Vaccines = PersonnelVaccineProcess.GetByPersonnelID(p.ID);
                p._Dependents = PersonnelDependentProcess.GetByPersonnelID(p.ID);
                p._Licenses = PersonnelLicenseProcess.GetByPersonnelID(p.ID, true);
                p._Departments = PersonnelDepartmentProcess.GetList(p.ID);
                p._Positions = PersonnelPositionProcess.GetList(p.ID);
                p._EmploymentType = PersonnelEmploymentTypeProcess.GetList(p.ID);
                p._LeaveCredits = PersonnelLeaveCreditProcess.Instance.Value.GetByPersonnelID(p.ID);
                p._AssumedDeductions = PersonnelDeductionProcess.GetAssumed(p.ID);
            }
            
            try
            {
                p.HasFailedEmail = dr[PersonnelFields.HasFailedEmail].ToBoolean();
                p.HasAdditionalLoan = AdditionalLoanProcess.Instance.Value.HasAdditionalLoan(p.ID);
            }
            catch { }

            if (!string.IsNullOrEmpty(p.ImagePath))
            {
                p.ImagePath = Path.Combine(ConfigurationManager.AppSettings["ImageFolder"], p.ImagePath);
            }

            return p;
        }

        public static IEnumerable<Personnel> GetList(string filter,
                                                     int? employmentStatusID,
                                                     int? departmentID,
                                                     int? personnelTypeID,
                                                     int? locationID,
                                                     int page,
                                                     int gridCount,
                                                     out int PageCount,
                                                     bool? isCrew = null)
        {
            var emp = new List<Personnel>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
                { PersonnelParameters.EmploymentStatusId, employmentStatusID },
                { PersonnelParameters.DepartmentID, departmentID },
                { PersonnelParameters.PersonnelTypeID, personnelTypeID },
                { PersonnelParameters.HiringLocationID, locationID },
                { "@IsCrew", isCrew},
                { FilterParameters.PageNumber, page },
                { FilterParameters.GridCount, gridCount }
            };

            var outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.FilterPersonnel, ref outParameters, Parameters))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                    PageCount = outParameters.Get(FilterParameters.PageCount).ToInt();
                }
            }

            return emp;
        }

        public static List<Personnel> GetList(bool personnelOnly = false)
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.GetPersonnel))
                {
                    PersonnelOnly = personnelOnly;
                    emp = ds.GetList(Converter);
                    personnelOnly = false;
                }
            }

            return emp;
        }

        public static List<Personnel> SearchResignedList(string key, bool personnelOnly = false)
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { FilterParameters.Filter, key }
                };

                using (var ds = db.ExecuteReader(PersonnelProcedures.SearchResigned, Parameters))
                {
                    PersonnelOnly = personnelOnly;
                    emp = ds.GetList(Converter);
                    personnelOnly = false;
                }
            }

            return emp;
        }

        public static List<Personnel> GetList(string filter, bool personnelOnly = false)
        {
            var emp = new List<Personnel>();

            var Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.SearchPersonnel, Parameters))
                {
                    PersonnelOnly = personnelOnly;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }
        public static List<Personnel> GetListByDepartment(DateTime date, long? personnelId, int? departmentId)
        {
            var Parameters = new Dictionary<string, object>
            {
                { "@Date", date},
                { "@PersonnelID", personnelId },
                { "@DepartmentID", departmentId }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.GetListByDepartment, Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public static List<Personnel> GetForPayroll(DateTime cutoffstartdate, DateTime cutoffenddate, PayrollSheet payrollSheet)
        {
            var emp = new List<Personnel>();

            var Parameters = new Dictionary<string, object>
            {
                { "@CutOffStartDate", cutoffstartdate },
                { "@CutOffEndDate", cutoffenddate },
                { "@Type", payrollSheet }
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("hr.GetPersonnelForPayroll", Parameters))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static Personnel Get(long Id, bool personnelOnly = false)
        {
            var emp = new Personnel();

            if (Id == 0)
            {
                emp.EmployeeNo = GetEmployeeNumber();
            }
            else
            {
                var Parameters = new Dictionary<string, object>
                {
                    { PersonnelParameters.ID, Id }
                };

                using (var db = new DBTools())
                {
                    PersonnelOnly = personnelOnly;
                    using (var ds = db.ExecuteReader(PersonnelProcedures.GetPersonnel, Parameters))
                    {
                        emp = ds.Get(Converter);
                    }
                    PersonnelOnly = false;
                }
            }

            return emp;
        }
        public static Personnel GetByBiometricId(int bioId)
        {
            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader("GetPersonnelByBiometricID", new Dictionary<string, object> { {"@BiometricID", bioId }}))
                {
                    PersonnelOnly = true;
                    return ds.Get(Converter);
                }
            }
        }
        public static Personnel GetByUserId(int userid, bool personnelOnly = false)
        {
            var emp = new Personnel();

            
            var Parameters = new Dictionary<string, object>
            {
                { "@UserId", userid}
            };

            using (var db = new DBTools())
            {
                PersonnelOnly = personnelOnly;
                using (var ds = db.ExecuteReader(PersonnelProcedures.GetByUserId, Parameters))
                {
                    emp = ds.Get(Converter);
                }
                PersonnelOnly = false;
            }

            return emp;
        }

        public static List<Personnel> GetMemoPersonnel(long MemoId)
        {
            var emp = new List<Personnel>();

            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.ID, MemoId },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.GetMemoPersonnel, Parameters))
                {
                    emp = ds.GetList(Converter);
                }
            }

            return emp;
        }
        public static List<Personnel> GetPersonnelEligibleToApprove(int? deptId, string filter)
        {
            using(var db = new DBTools())
            {
                var Parameters = new Dictionary<string, object>
                {
                    { "@DepartmentID", deptId },
                    { "@Filter", filter },
                };
                using(var ds = db.ExecuteReader("hr.GetPersonnelForApprover", Parameters))
                {
                    return ds.GetList(Converter);
                }
            }
        }
        public static List<Personnel> GetMemoArchivesPerson(long MemoId, bool personnelOnly = false)
        {
            var emp = new List<Personnel>();

            var Parameters = new Dictionary<string, object>
            {
                { MemoArchivesParameters.ID, MemoId },
            };

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelProcedures.GetMemoPerson, Parameters))
                {
                    PersonnelOnly = personnelOnly;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }
        
        public static Personnel CreateOrUpdatePersonalInfo(Personnel personnel, int userid)
        {
            using (var db = new DBTools())
            {
                personnel = CreateOrUpdatePersonalInfo(db, personnel, userid);

                if (personnel._ContactNumbers?.Any() ?? false)
                {
                    foreach (var cn in personnel._ContactNumbers)
                    {
                        cn.PersonnelID = personnel.ID;
                        if(cn.IsDeleted)
                        {
                            ContactNumberProcess.Delete(db, cn.ID, userid);
                        }
                        else
                        {
                            ContactNumberProcess.CreateOrUpdate(db, cn, userid);
                        }
                    }
                }
            }
            return personnel;
        }

        public static Personnel CreateOrUpdatePersonalInfo(DBTools db, Personnel personnel, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.EmployeeNo, personnel.EmployeeNo },
                { PersonnelParameters.FirstName, personnel.FirstName },
                { PersonnelParameters.MiddleName, personnel.MiddleName },
                { PersonnelParameters.MaidenMiddleName, personnel.MaidenMiddleName },
                { PersonnelParameters.LastName, personnel.LastName },
                { PersonnelParameters.Gender, personnel.Gender },
                { PersonnelParameters.NationalityID, personnel.NationalityID },
                { PersonnelParameters.BirthDate, personnel.BirthDate },
                { PersonnelParameters.BirthPlace, personnel.BirthPlace },
                { PersonnelParameters.Address, personnel.Address },
                { PersonnelParameters.TelephoneNo, personnel.TelephoneNo },
                { PersonnelParameters.MobileNo, personnel.MobileNo },
                { PersonnelParameters.CivilStatusID, personnel.CivilStatusID },
                { PersonnelParameters.ReligionID, personnel.ReligionID },
                { PersonnelParameters.SSS, personnel.SSS },
                { PersonnelParameters.Philhealth, personnel.Philhealth },
                { PersonnelParameters.PAGIBIG, personnel.PAGIBIG },
                { PersonnelParameters.TIN, personnel.TIN },
                { PersonnelParameters.TaxTypeID, personnel.TaxTypeID },
                { PersonnelParameters.Email, personnel.Email },
                { PersonnelParameters.EmergencyContactAddress, personnel.EmergencyContactAddress },
                { PersonnelParameters.EmergencyContactNumber, personnel.EmergencyContactNumber },
                { PersonnelParameters.EmergencyContactPerson, personnel.EmergencyContactPerson },
                { PersonnelParameters.EmergencyRelationshipID, personnel.EmergencyRelationshipID },
                { PersonnelParameters.ImagePath, personnel.ImagePath },
                { PersonnelParameters.PersonnelTypeID, personnel.PersonnelTypeID },
                { PersonnelParameters.HiringLocationID, personnel.HiringLocationID },
                { PersonnelParameters.DateHired, personnel.DateHired },
                { PersonnelParameters.ResignationDate, personnel.ResignationDate },
                { PersonnelParameters.ResignationRemarks, personnel.ResignationRemarks },
                { PersonnelParameters.EmploymentStatusId, personnel.EmploymentStatusId },
                { PersonnelParameters.AutoOT, personnel.AutoOT },
                { PersonnelParameters.AdditionalHazardRate, personnel.AdditionalHazardRate },
                { PersonnelParameters.BiometricsID, personnel.BiometricsID },
                { CredentialParameters.LogBy, userid }
            };

            var outParameters = new List<OutParameters>
            {
                { PersonnelParameters.ID, SqlDbType.BigInt, personnel.ID }
            };

            db.ExecuteNonQuery(PersonnelProcedures.CreateOrUpdate, ref outParameters, Parameters);
            personnel.ID = outParameters.Get(PersonnelParameters.ID).ToLong();

            return personnel;
        }

        public static Personnel UpdateEmploymentInfo(Personnel personnel, int userid)
        {
            using (var db = new DBTools())
            {
                db.StartTransaction();
                try
                {
                    UpdateEmploymentInfo(db, personnel, userid);
                    db.CommitTransaction();
                }
                catch(Exception)
                { db.RollBackTransaction(); throw; }

                return personnel;
            }
        }

        public static void UpdateEmploymentInfo(DBTools db, Personnel personnel, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.ID, personnel.ID },
                { PersonnelParameters.ReferredBy, personnel.ReferredBy },
                { PersonnelParameters.ReferenceContactNo, personnel.ReferenceContactNo },
                { PersonnelParameters.WalkIn, personnel.WalkIn },
                { PersonnelParameters.PayrollTypeID, personnel.PayrollTypeID },
                { CredentialParameters.LogBy, userid }
            };

            db.ExecuteNonQuery(PersonnelProcedures.UpdatePersonnelEmploymentInfo, Parameters);
        }

        public static void Delete(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelProcedures.DeletePersonnel, Parameters);
            }
        }

        public static string GetEmployeeNumber()
        {
            var number = "";

            using (var db = new DBTools())
            {
                number = db.ExecuteScalar(PersonnelProcedures.GetEmployeeNumber)?.ToString();
            }

            return number;
        }

        public static IEnumerable<Personnel> GetBirthdayCelebrantsThisDay()
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetBirthdayCelebrantsProcedures.GetBirthdayCelebrantsThisDay))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static IEnumerable<Personnel> GetBirthdayCelebrantsThisWeek()
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetBirthdayCelebrantsProcedures.GetBirthdayCelebrantsThisWeek))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static IEnumerable<Personnel> GetBirthdayCelebrantsThisMonth()
        {
            var emp = new List<Personnel>();  

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetBirthdayCelebrantsProcedures.GetBirthdayCelebrantsThisMonth))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static IEnumerable<Personnel> GetBirthdayCelebrantsThisMonthRecent()
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetBirthdayCelebrantsProcedures.GetBirthdayCelebrantsThisMonthRecent))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static IEnumerable<Personnel> GetBirthdayCelebrantsThisMonthUpcoming()
        {
            var emp = new List<Personnel>();

            using (var db = new DBTools())
            {
                using (var ds = db.ExecuteReader(PersonnelGetBirthdayCelebrantsProcedures.GetBirthdayCelebrantsThisMonthUpcoming))
                {
                    PersonnelOnly = true;
                    emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                }
            }

            return emp;
        }

        public static List<Personnel> GetApprovedPersonnel(string filter, int pagenumber, int gridcount, out int count)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
                { FilterParameters.PageNumber, pagenumber },
                { FilterParameters.GridCount, gridcount }
            };
            List<OutParameters> outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader(PersonnelProcedures.FilterApprovedPersonnel, ref outParameters, Parameters))
                {
                    PersonnelOnly = true;
                    List<Personnel> emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                    count = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return emp;
                }
            }
        }

        public static List<Personnel> GetApprovingPersonnel(string filter, int pagenumber, int gridcount, out int count)
        {
            Dictionary<string, object> Parameters = new Dictionary<string, object>
            {
                { FilterParameters.Filter, filter },
                { FilterParameters.PageNumber, pagenumber },
                { FilterParameters.GridCount, gridcount }
            };
            List<OutParameters> outParameters = new List<OutParameters>
            {
                { FilterParameters.PageCount, SqlDbType.Int }
            };

            using (DBTools db = new DBTools())
            {
                using (DataSet ds = db.ExecuteReader(PersonnelProcedures.FilterApprovingPersonnel, ref outParameters, Parameters))
                {
                    PersonnelOnly = true;
                    List<Personnel> emp = ds.GetList(Converter);
                    PersonnelOnly = false;
                    count = outParameters.Get(FilterParameters.PageCount).ToInt();
                    return emp;
                }
            }
        }

        public static void Approved(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelProcedures.Approved, Parameters);
            }
        }

        public static void Cancelled(long Id, int userid)
        {
            var Parameters = new Dictionary<string, object>
            {
                { PersonnelParameters.ID, Id },
                { CredentialParameters.LogBy, userid }
            };

            using (var db = new DBTools())
            {
                db.ExecuteNonQuery(PersonnelProcedures.Cancelled, Parameters);
            }
        }
    }
}
