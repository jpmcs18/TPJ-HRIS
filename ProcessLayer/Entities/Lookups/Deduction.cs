namespace ProcessLayer.Entities
{
    public class Deduction : Lookup<int>
    {
        public byte? WhenToDeduct { get; set; }
        public bool? ComputedThruSalary { get; set; }
        public bool? GovernmentDeduction { get; set; }
        public bool? AutoCompute { get; set; }
        public WhenToDeduct Deduct { get; set; }
    }
}
