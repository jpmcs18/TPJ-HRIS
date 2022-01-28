namespace ProcessLayer.Entities
{
    public class Deduction : Lookup<int>
    {
        public int? WhenToDeduct { get; set; }
        public bool? ComputedThruSalary { get; set; }
        public bool? GovernmentDeduction { get; set; }
        public bool? AutoCompute { get; set; }
        public WhenToDeduct Deduct { get; set; }
    }
}
