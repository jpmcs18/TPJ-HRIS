using ProcessLayer.Helpers.Enumerable;

namespace ProcessLayer.Entities
{
    public class Compensation : Lookup<int>
    {
        public bool? Taxable { get; set; } = false;
        public bool? SupplementarySalary { get; set; } = false;
        public bool? Has_Approval { get; set; } = false;
        public ComputationType ComputationType { get; set; }
    }
}
