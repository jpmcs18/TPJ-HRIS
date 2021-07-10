using ProcessLayer.Helpers.Enumerable;

namespace ProcessLayer.Entities
{
    public class Compensation : Lookup<int>
    {
        public bool? Taxable { get; set; }
        public bool? SupplementarySalary { get; set; }
        public bool? Has_Approval { get; set; }
        public ComputationType ComputationType { get; set; }
    }
}
