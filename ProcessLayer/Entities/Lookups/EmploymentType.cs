namespace ProcessLayer.Entities
{
    public class EmploymentType : Lookup<int>
    {
        public bool WithGovtDeduction { get; set; }
    }
}
