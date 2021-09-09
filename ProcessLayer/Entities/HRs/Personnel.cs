using ProcessLayer.Entities.HR;
using ProcessLayer.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class Personnel : LogDetails
    {
        public long ID { get; set; }
        [DisplayName("Employee No.")]
	    public string EmployeeNo { get; set; }
        public string FullName {
            get {
                return String.Format("{0}{1}{2}{3}{4}", LastName
                                                      , (string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName) ? "" : ", ")
                                                      , FirstName
                                                      , (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(MiddleName) ? "" : " ")
                                                      , MiddleName);
            }
        }
        [DisplayName("First Name")]
	    public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [DisplayName("Maiden Middle Name")]
        public string MaidenMiddleName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
	    public bool? Gender { get; set; }
        [DisplayName("Date Hired")]
        public DateTime? DateHired { get; set; }
        [DisplayName("Nationality")]
        public int? NationalityID { get; set; }
        [DisplayName("Employment Status")]
        public int? EmploymentStatusId { get; set; }
        [DisplayName("Resignation Date")]
	    public DateTime? ResignationDate { get; set; }
        [DisplayName("Resignation Remarks")]
	    public string ResignationRemarks { get; set; }
        [DisplayName("Birth Date")]
	    public DateTime? BirthDate { get; set; }
        [DisplayName("Birth Place")]
	    public string BirthPlace { get; set; }
	    public string Address { get; set; }
        [DisplayName("Telephone No.")]
	    public string TelephoneNo { get; set; }
        [DisplayName("Mobile No.")]
        public string MobileNo { get; set; }
        [DisplayName("Civil Status")]
	    public int? CivilStatusID { get; set; }
        [DisplayName("Religion")]
	    public int? ReligionID { get; set; }
        [DisplayName("Personnel Type")]
	    public int? PersonnelTypeID { get; set; }
        [DisplayName("Hiring Location")]
	    public int? HiringLocationID { get; set; }
        [DisplayName("Tax Type")]
        public byte? TaxTypeID { get; set; }
        public string SSS { get; set; }
        [DisplayName("PhilHealth")]
	    public string Philhealth { get; set; }
        [DisplayName("Pag-IBIG")]
	    public string PAGIBIG { get; set; }
	    public string TIN { get; set; }
        [DisplayName("Name")]
        public string EmergencyContactPerson { get; set; }
        [DisplayName("Address")]
        public string EmergencyContactAddress { get; set; }
        [DisplayName("Contact No.")]
        public string EmergencyContactNumber { get; set; }
        public string Email { get; set; }
        [DisplayName("Referred By")]
        public string ReferredBy { get; set; }
        [DisplayName("Contact No.")]
        public string ReferenceContactNo { get; set; }
        [DisplayName("Walk-In")]
        public bool? WalkIn { get; set; }
        public string ImagePath { get; set; }
        public string Image { get; set; }

        [DisplayName("Payroll Type")]
        public byte? PayrollTypeID { get; set; }
        [DisplayName("Fixed Salary")]
        public bool? FixedSalary { get; set; }
        public long? LinkedPersonnelID { get; set; }

        [DisplayName("Automatic Overtime")]
        public bool AutoOT { get; set; }
        [DisplayName("Additional Hazard Rate")]
        public decimal AdditionalHazardRate { get; set; }
        [DisplayName("Biomatrics ID")]
        public int BiometricsID { get; set; }
        public bool? Approved { get; set; }
        public bool? Cancelled { get; set; }

        public List<EducationalBackground> _EducationalBackground { get; set; } = new List<EducationalBackground>();
        public List<WorkExperience> _WorkExperience { get; set; } = new List<WorkExperience>();
        public List<PersonnelCompensation> _Compensations { get; set; } = new List<PersonnelCompensation>();
        //public List<PersonnelDeduction> _Deductions { get; set; } = new List<PersonnelDeduction>();
        public List<AssumedPersonnelDeduction> _AssumedDeductions { get; set; } = new List<AssumedPersonnelDeduction>();
        public List<PersonnelDependent> _Dependents { get; set; } = new List<PersonnelDependent>();
        public List<PersonnelLegislation> _Legislations { get; set; } = new List<PersonnelLegislation>();
        public List<PersonnelLicense> _Licenses { get; set; } = new List<PersonnelLicense>();
        public List<MemoArchives> _Memos { get; set; } = new List<MemoArchives>();
        public List<PersonnelVaccine> _Vaccines { get; set; } = new List<PersonnelVaccine>();
        public List<PersonnelTraining> _Trainings { get; set; } = new List<PersonnelTraining>();
        public List<PersonnelDepartment> _Departments { get; set; } = new List<PersonnelDepartment>();
        public List<PersonnelPosition> _Positions { get; set; } = new List<PersonnelPosition>();
        public List<PersonnelAssignedLocation> _AssignedLocation { get; set; } = new List<PersonnelAssignedLocation>();
        public List<PersonnelEmploymentType> _EmploymentType { get; set; } = new List<PersonnelEmploymentType>();
        public List<PersonnelLeaveCredit> _LeaveCredits { get; set; } = new List<PersonnelLeaveCredit>();
        public List<ContactNumber> _ContactNumbers { get; set; } = new List<ContactNumber>();
        public string _Gender { get; set; }
        public Lookup _EmploymentStatus { get; set; } = new Lookup();
        public Lookup _CivilStatus { get; set; } = new Lookup();
        public Lookup _Religion { get; set; } = new Lookup();
        public PersonnelType _PersonnelType { get; set; } = new PersonnelType();
        public Location _HiringLocation { get; set; } = new Location();
        public ScheduleType _ScheduleType { get; set; } = new ScheduleType();
        public PayrollType _PayrollType { get; set; } = new PayrollType();
        public List<PersonnelSchedule> _Schedules { get; set; } = new List<PersonnelSchedule>();

        public bool HasFailedEmail { get; set; } = false;
        public bool HasAdditionalLoan { get; set; } = false;
        
        public Personnel _LinkedPersonnel { get; set; }
        public int Years 
        { 
            get 
            {
                DateTime dateHired = (DateHired ?? DateTime.Now);
                DateTime dateEnd = ResignationDate ?? DateTime.Now;
                int year = dateEnd.Year - dateHired.Year;
                year += dateHired.Month >= dateEnd.Month ? (dateHired.Month == dateEnd.Month ? (dateHired.Day > dateEnd.Day ? -1 : 0) : -1) : 0;
                return year;
            } 
        }

        public int Months
        {
            get
            {
                DateTime dateHired = DateHired ?? DateTime.Now;
                DateTime dateEnd = ResignationDate ?? DateTime.Now;
                int months = dateEnd.Month - dateHired.Month;
                months += (dateHired.Day > dateEnd.Day ? -1 : 0);
                if (months < 0)
                    months += (dateHired.Year != dateEnd.Year ? 12 : 0);
                return months;
            }
        }
    }
}
