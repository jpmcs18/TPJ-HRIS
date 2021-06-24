namespace ProcessLayer.Entities
{
    public class PersonnelEmploymentType : PersonnelHistoryBase
    {
        public int? EmploymentTypeID { get; set; }

        public EmploymentType _EmploymentType { get; set; } = new EmploymentType();
    }

}
