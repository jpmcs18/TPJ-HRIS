namespace ProcessLayer.Entities
{
    public class PersonnelDepartment : PersonnelHistoryBase
    {
        public int? DepartmentID { get; set; }

        public Department _Department { get; set; } = new Department();
    }
}
